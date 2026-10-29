using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiaRemisionVentaByNumber;

public sealed record GetGuiaRemisionVentaByNumberQuery(string Number) : IRequest<GuiaRemisionVentaResponse>;

public sealed class GetGuiaRemisionVentaByNumberQueryHandler
    : IRequestHandler<GetGuiaRemisionVentaByNumberQuery, GuiaRemisionVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetGuiaRemisionVentaByNumberQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<GuiaRemisionVentaResponse> Handle(GetGuiaRemisionVentaByNumberQuery request, CancellationToken ct)
        => GuiaRemisionVentaReader.GetAsync(_uow, GuiaRemisionVentaRules.NormalizeNumber(request.Number), ct);
}
