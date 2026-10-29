using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescoByCode
{
    public class GetParentescoByCodeQueryHandler : IRequestHandler<GetParentescoByCodeQuery, ParentescoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetParentescoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ParentescoResponse> Handle(GetParentescoByCodeQuery request, CancellationToken cancellationToken)
        {
            var parentesco = await _uow.Rrhh.Catalogos.Parentescos.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new ParentescoResponse
                {
                    Code = p.Code,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (parentesco is null)
            {
                throw new KeyNotFoundException($"Parentesco {request.Code} no encontrado.");
            }

            return parentesco;
        }
    }
}