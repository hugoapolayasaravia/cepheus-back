using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.CreateFlete
{
    public class CreateFleteCommandHandler : IRequestHandler<CreateFleteCommand, FleteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFleteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FleteResponse> Handle(CreateFleteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.Fletes.Query().Select(t => t.Code), length: 2, entityLabel: "Fletes", cancellationToken);

            var flete = new Flete
            {
                Code = code,
                Name = request.Name.Trim(),
                Amount = request.Amount,
                IsDefault = request.IsDefault,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.Fletes.AddAsync(flete, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(flete);
        }

        internal static FleteResponse Map(Flete flete) => new()
        {
            Code = flete.Code,
            Name = flete.Name,
            Amount = flete.Amount,
            IsDefault = flete.IsDefault,
            IsActive = flete.IsActive,
            CreatedAt = flete.CreatedAt,
            UpdatedAt = flete.UpdatedAt,
            RowVersion = flete.RowVersion
        };
    }
}
