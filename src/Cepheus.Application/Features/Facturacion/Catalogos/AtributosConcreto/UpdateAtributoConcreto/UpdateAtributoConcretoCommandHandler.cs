using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.UpdateAtributoConcreto
{
    public class UpdateAtributoConcretoCommandHandler : IRequestHandler<UpdateAtributoConcretoCommand, AtributoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAtributoConcretoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AtributoConcretoResponse> Handle(UpdateAtributoConcretoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.AtributosConcreto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Atributo de concreto {request.Code} no encontrado.");
            }

            var entity = new AtributoConcreto
            {
                Code = request.Code,
                AttributeType = System.Enum.Parse<TipoAtributoConcreto>(request.AttributeType.Trim(), ignoreCase: true),
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.AtributosConcreto.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateAtributoConcreto.CreateAtributoConcretoCommandHandler.Map(entity);
        }
    }
}
