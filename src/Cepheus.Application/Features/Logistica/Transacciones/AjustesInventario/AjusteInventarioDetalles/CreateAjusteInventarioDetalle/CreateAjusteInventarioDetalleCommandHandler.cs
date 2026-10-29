using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.CreateAjusteInventarioDetalle;

public sealed class CreateAjusteInventarioDetalleCommandHandler
    : IRequestHandler<CreateAjusteInventarioDetalleCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateAjusteInventarioDetalleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AjusteInventarioResponse> Handle(
        CreateAjusteInventarioDetalleCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.AjusteCode);
        var articulo = AjusteInventarioRules.Normalize(request.ArticuloCode);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        AjusteInventarioRules.EnsureLineasAgregables(ajuste);

        if (ajuste.Detalles.Any(d => d.ArticuloCode == articulo))
        {
            throw new InvalidOperationException(
                $"El artículo {articulo} ya está registrado en el Ajuste de Inventario.");
        }

        var resuelta = await AjusteInventarioLineas.ResolverAsync(
            _uow,
            planta,
            articulo,
            request.Cantidad,
            null,
            ct);

        ajuste.Detalles.Add(new AjusteInventarioDetalle
        {
            PlantaCode = planta,
            AjusteCode = code,
            ArticuloCode = articulo,
            Tipo = request.Tipo,
            Cantidad = request.Cantidad,
            Precio = resuelta.Precio,
            Total = AjusteInventarioRules.LineTotal(request.Cantidad, resuelta.Precio),
            Estado = EstadoAjusteInventarioDetalle.Pendiente
        });

        AjusteInventarioLineas.Renumerar(ajuste);
        await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);

        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }
}
