using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.CreateModoPago
{
    public class CreateModoPagoCommandHandler : IRequestHandler<CreateModoPagoCommand, ModoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateModoPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModoPagoResponse> Handle(CreateModoPagoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.ModosPago.Query().Select(m => m.Code), length: 3, entityLabel: "ModosPago", cancellationToken);

            var modoPago = new ModoPago
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.ModosPago.AddAsync(modoPago, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(modoPago);
        }

        internal static ModoPagoResponse Map(ModoPago modoPago) => new()
        {
            Code = modoPago.Code,
            Name = modoPago.Name,
            IsActive = modoPago.IsActive,
            CreatedAt = modoPago.CreatedAt,
            UpdatedAt = modoPago.UpdatedAt,
            RowVersion = modoPago.RowVersion
        };
    }
}