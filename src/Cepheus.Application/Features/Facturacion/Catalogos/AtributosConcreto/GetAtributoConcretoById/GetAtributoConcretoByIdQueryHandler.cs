using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributoConcretoById
{
    public class GetAtributoConcretoByIdQueryHandler : IRequestHandler<GetAtributoConcretoByIdQuery, AtributoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAtributoConcretoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AtributoConcretoResponse> Handle(GetAtributoConcretoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.AtributosConcreto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Atributo de concreto {request.Code} no encontrado.");
            }

            return CreateAtributoConcretoCommandHandler.Map(entity);
        }
    }
}
