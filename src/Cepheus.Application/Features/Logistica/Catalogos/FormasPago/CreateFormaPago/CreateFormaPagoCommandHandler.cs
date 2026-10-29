using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.CreateFormaPago
{
    public class CreateFormaPagoCommandHandler : IRequestHandler<CreateFormaPagoCommand, FormaPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFormaPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoResponse> Handle(CreateFormaPagoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.FormasPago.Query().Select(f => f.Code), length: 2, entityLabel: "Formas de Pago", cancellationToken);

            var formaPago = new FormaPago
            {
                Code = code,
                Name = request.Name.Trim(),
                Days = request.Days,
                IsCredit = request.IsCredit,
                IsActive = true
            };

            await _uow.Logistica.Catalogos.FormasPago.AddAsync(formaPago, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(formaPago);
        }

        internal static FormaPagoResponse Map(FormaPago formaPago) => new()
        {
            Code = formaPago.Code,
            Name = formaPago.Name,
            Days = formaPago.Days,
            IsCredit = formaPago.IsCredit,
            IsActive = formaPago.IsActive,
            CreatedAt = formaPago.CreatedAt,
            UpdatedAt = formaPago.UpdatedAt,
            CreatedBy = formaPago.CreatedBy,
            UpdatedBy = formaPago.UpdatedBy,
            RowVersion = formaPago.RowVersion
        };
    }
}