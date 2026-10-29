using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.AnulateVale;

public sealed class AnulateValeCommandHandler
    : IRequestHandler<AnulateValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AnulateValeCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ValeResponse> Handle(
        AnulateValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (vale.Estado == EstadoVale.Anulado)
        {
            throw new InvalidOperationException("El Vale de Salida ya está anulado.");
        }

        if (vale.Estado is not (EstadoVale.Pendiente or EstadoVale.Aprobado or EstadoVale.AprobacionProvisional))
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se anula en Pendiente, Aprobado o Aprobación Provisional.");
        }

        foreach (var linea in vale.Detalles.Where(d => d.Estado == EstadoValeDetalle.Pendiente))
        {
            await ValeOrdenTrabajo.ReleaseAsync(_uow, vale, linea, ct);
            linea.Estado = EstadoValeDetalle.Anulado;
        }

        vale.Estado = EstadoVale.Anulado;
        vale.Preparado = false;
        vale.AnuladoPor = _currentUser.FullName;
        vale.FechaAnulacion = DateTime.Now;

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
