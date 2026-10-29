using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetComprobanteVenta;

public sealed record GetComprobanteVentaQuery(string TypeCode, string Number) : IRequest<ComprobanteVentaResponse>;

public sealed class GetComprobanteVentaQueryHandler : IRequestHandler<GetComprobanteVentaQuery, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetComprobanteVentaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<ComprobanteVentaResponse> Handle(GetComprobanteVentaQuery request, CancellationToken ct)
        => ComprobanteVentaReader.GetAsync(
            _uow,
            ComprobanteVentaRules.NormalizeType(request.TypeCode),
            ComprobanteVentaRules.NormalizeNumber(request.Number),
            ct);
}
