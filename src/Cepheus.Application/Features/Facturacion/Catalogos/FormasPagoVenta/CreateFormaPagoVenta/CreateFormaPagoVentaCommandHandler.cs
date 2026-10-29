using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.CreateFormaPagoVenta
{
    public class CreateFormaPagoVentaCommandHandler : IRequestHandler<CreateFormaPagoVentaCommand, FormaPagoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFormaPagoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoVentaResponse> Handle(CreateFormaPagoVentaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.FormasPagoVenta.Query().Select(t => t.Code), length: 2, entityLabel: "Formas de Pago de Ventas", cancellationToken);

            var formaPagoVenta = new FormaPagoVenta
            {
                Code = code,
                Name = request.Name.Trim(),
                Days = request.Days,
                IsCredit = request.IsCredit,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.FormasPagoVenta.AddAsync(formaPagoVenta, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(formaPagoVenta);
        }

        internal static FormaPagoVentaResponse Map(FormaPagoVenta formaPagoVenta) => new()
        {
            Code = formaPagoVenta.Code,
            Name = formaPagoVenta.Name,
            Days = formaPagoVenta.Days,
            IsCredit = formaPagoVenta.IsCredit,
            IsActive = formaPagoVenta.IsActive,
            CreatedAt = formaPagoVenta.CreatedAt,
            UpdatedAt = formaPagoVenta.UpdatedAt,
            RowVersion = formaPagoVenta.RowVersion
        };
    }
}
