using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.GetElementoConcretoByCode
{
    public class GetElementoConcretoByCodeQueryHandler : IRequestHandler<GetElementoConcretoByCodeQuery, ElementoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetElementoConcretoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ElementoConcretoResponse> Handle(GetElementoConcretoByCodeQuery request, CancellationToken cancellationToken)
        {
            var elementoConcreto = await _uow.Facturacion.Catalogos.ElementosConcreto.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new ElementoConcretoResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    UsesBalance = t.UsesBalance,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (elementoConcreto is null)
            {
                throw new KeyNotFoundException($"Elemento de concreto {request.Code} no encontrado.");
            }

            return elementoConcreto;
        }
    }
}
