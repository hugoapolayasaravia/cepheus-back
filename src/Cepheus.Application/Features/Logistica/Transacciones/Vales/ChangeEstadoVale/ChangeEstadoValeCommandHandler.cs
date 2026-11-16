using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ChangeEstadoVale;

public sealed class ChangeEstadoValeCommandHandler
    : IRequestHandler<ChangeEstadoValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IAprobacionService _aprobacion;
    private readonly ICurrentUserService _currentUser;

    public ChangeEstadoValeCommandHandler(
        IUnitOfWork uow,
        IAprobacionService aprobacion,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _aprobacion = aprobacion;
        _currentUser = currentUser;
    }

    public async Task<ValeResponse> Handle(
        ChangeEstadoValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        if (!ValeRules.TryParseEstado(request.NuevoEstado, out var nuevoEstado))
        {
            throw new ArgumentException(
                $"Estado '{request.NuevoEstado}' no es válido.");
        }

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (ValeRules.EsTransferencia(vale))
        {
            throw new InvalidOperationException(
                "Un vale de tipo Transferencia no se aprueba desde el almacén origen.");
        }

        switch (nuevoEstado)
        {
            case EstadoVale.Aprobado:
                await AprobarAsync(vale, ct);
                break;

            case EstadoVale.Pendiente:
                Desaprobar(vale);
                break;

            default:
                throw new InvalidOperationException(
                    $"No se puede pasar de '{vale.Estado}' a '{nuevoEstado}' con este comando. " +
                    "Use procesar, devolver o anular según corresponda.");
        }

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }

    private async Task AprobarAsync(Vale vale, CancellationToken ct)
    {
        if (vale.Estado != EstadoVale.Pendiente)
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se aprueba desde Pendiente.");
        }

        if (!vale.Detalles.Any(d => d.Estado == EstadoValeDetalle.Pendiente))
        {
            throw new InvalidOperationException(
                "El Vale de Salida no tiene líneas para aprobar.");
        }

        await ValidarAprobadorAsync(vale, ct);

        vale.Estado = EstadoVale.Aprobado;
        vale.AprobadoPor = _currentUser.Login;
        vale.FechaAprobacion = DateTime.Now;
    }

    /// <summary>
    /// Un vale Aprobado solo vuelve a Pendiente por quien lo aprobó (PB: dw_8, estado 09 -> 01).
    /// </summary>
    private void Desaprobar(Vale vale)
    {
        if (vale.Estado != EstadoVale.Aprobado)
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se devuelve a Pendiente desde Aprobado.");
        }

        if (!string.Equals(vale.AprobadoPor, _currentUser.Login, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Solo el usuario que aprobó el vale puede devolverlo a Pendiente.");
        }

        vale.Estado = EstadoVale.Pendiente;
        vale.AprobadoPor = null;
        vale.FechaAprobacion = null;
        vale.Preparado = false;
    }

    /// <summary>
    /// Regla única de aprobación (nivel asignado + límites diario/mensual por FechaAprobacion).
    /// Tipo de transacción VS / VC según el tipo de vale; los vales se valorizan en soles.
    /// </summary>
    private Task ValidarAprobadorAsync(Vale vale, CancellationToken ct)
    {
        return _aprobacion.AsegurarAprobadorActualAsync(
            new DocumentoAprobable(
                ValeRules.TipoTransaccion(vale.TipoValeCode),
                vale.PlantaCode,
                vale.Code,
                vale.UnidadNegocioCode,
                ValeRules.MonedaSoles,
                vale.Neto),
            ct);
    }
}
