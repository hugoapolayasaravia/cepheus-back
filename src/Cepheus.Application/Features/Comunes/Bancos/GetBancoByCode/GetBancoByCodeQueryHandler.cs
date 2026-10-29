using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.GetBancoByCode
{
    public class GetBancoByCodeQueryHandler : IRequestHandler<GetBancoByCodeQuery, BancoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetBancoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BancoResponse> Handle(GetBancoByCodeQuery request, CancellationToken cancellationToken)
        {
            var banco = await _uow.Comunes.Bancos.Query()
                .AsNoTracking()
                .Where(b => b.Code == request.Code)
                .Select(b => new BancoResponse
                {
                    Code = b.Code,
                    Name = b.Name,
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    CreatedBy = b.CreatedBy,
                    UpdatedBy = b.UpdatedBy,
                    RowVersion = b.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (banco is null)
            {
                throw new KeyNotFoundException($"Banco {request.Code} no encontrado.");
            }

            return banco;
        }
    }
}
