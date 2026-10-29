using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion;
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
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ChangeEstadoValeCommandHandler(
        IUnitOfWork uow,
        ISender sender,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _sender = sender;
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
        vale.AprobadoPor = _currentUser.FullName;
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

        if (!string.Equals(vale.AprobadoPor, _currentUser.FullName, StringComparison.OrdinalIgnoreCase))
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
    /// Mismo esquema de aprobación que la Orden de Compra: rango por tipo de transacción (VS / VC),
    /// unidad de negocio, moneda (soles) y monto neto; el usuario debe ser aprobador, suplente o superior
    /// del nivel que cubre el monto.
    /// </summary>
    private async Task ValidarAprobadorAsync(Vale vale, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
        {
            throw new InvalidOperationException(
                "No se pudo identificar al usuario autenticado.");
        }

        var trabajadorCode = await _uow.Administracion.Users
            .Query()
            .AsNoTracking()
            .Where(u => u.Id == _currentUser.UserId.Value)
            .Select(u => u.TrabajadorCode)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(trabajadorCode))
        {
            throw new InvalidOperationException(
                "Tu usuario no tiene un Trabajador de RRHH vinculado — no puedes aprobar Vales de Salida.");
        }

        var tipoTransaccion = ValeRules.TipoTransaccion(vale.TipoValeCode);

        var rango = await _sender.Send(
            new ResolverRangoAprobacionQuery(
                tipoTransaccion,
                vale.UnidadNegocioCode,
                ValeRules.MonedaSoles,
                vale.Neto),
            ct);

        if (rango.RangoAplicable is null)
        {
            throw new InvalidOperationException(
                $"No existe un rango de aprobación configurado que cubra S/ {vale.Neto:N2} " +
                $"para la unidad de negocio {vale.UnidadNegocioCode} ({tipoTransaccion}).");
        }

        var aprobadores = await _sender.Send(
            new GetAprobadoresPorCombinacionQuery(
                rango.RangoAplicable.NivelCode,
                tipoTransaccion,
                rango.UnidadNegocioEfectivaCode,
                ValeRules.MonedaSoles),
            ct);

        var autorizado = aprobadores.Any(a =>
            a.TrabajadorCode == trabajadorCode ||
            a.SuplenteTrabajadorCode == trabajadorCode ||
            a.SuperiorTrabajadorCode == trabajadorCode);

        if (!autorizado)
        {
            throw new InvalidOperationException(
                $"No estás autorizado para aprobar este Vale de Salida (nivel {rango.RangoAplicable.NivelCode}, " +
                $"monto S/ {vale.Neto:N2}).");
        }
    }
}
