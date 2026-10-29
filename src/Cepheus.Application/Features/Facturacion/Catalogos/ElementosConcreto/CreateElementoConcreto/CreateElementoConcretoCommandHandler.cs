using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.CreateElementoConcreto
{
    public class CreateElementoConcretoCommandHandler : IRequestHandler<CreateElementoConcretoCommand, ElementoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateElementoConcretoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ElementoConcretoResponse> Handle(CreateElementoConcretoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.ElementosConcreto.Query().Select(t => t.Code), length: 2, entityLabel: "Elementos de Concreto", cancellationToken);

            var elementoConcreto = new ElementoConcreto
            {
                Code = code,
                Name = request.Name.Trim(),
                UsesBalance = request.UsesBalance,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.ElementosConcreto.AddAsync(elementoConcreto, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(elementoConcreto);
        }

        internal static ElementoConcretoResponse Map(ElementoConcreto elementoConcreto) => new()
        {
            Code = elementoConcreto.Code,
            Name = elementoConcreto.Name,
            UsesBalance = elementoConcreto.UsesBalance,
            IsActive = elementoConcreto.IsActive,
            CreatedAt = elementoConcreto.CreatedAt,
            UpdatedAt = elementoConcreto.UpdatedAt,
            CreatedBy = elementoConcreto.CreatedBy,
            UpdatedBy = elementoConcreto.UpdatedBy,
            RowVersion = elementoConcreto.RowVersion
        };
    }
}
