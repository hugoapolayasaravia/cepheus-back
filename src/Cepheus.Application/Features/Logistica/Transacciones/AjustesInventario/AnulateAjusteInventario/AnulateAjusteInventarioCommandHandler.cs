using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AnulateAjusteInventario;

public sealed class AnulateAjusteInventarioCommandHandler
    : IRequestHandler<AnulateAjusteInventarioCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public AnulateAjusteInventarioCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AjusteInventarioResponse> Handle(
        AnulateAjusteInventarioCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.Code);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (ajuste.Estado == EstadoAjusteInventario.Anulado)
        {
            throw new InvalidOperationException("El Ajuste de Inventario ya está anulado.");
        }

        if (ajuste.Estado != EstadoAjusteInventario.Pendiente)
        {
            throw new InvalidOperationException(
                $"El Ajuste de Inventario se encuentra {ajuste.Estado}; solo se anula en estado Pendiente.");
        }

        foreach (var linea in ajuste.Detalles.Where(d => d.Estado == EstadoAjusteInventarioDetalle.Pendiente))
        {
            linea.Estado = EstadoAjusteInventarioDetalle.Anulado;
        }

        ajuste.Estado = EstadoAjusteInventario.Anulado;

        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }
}
