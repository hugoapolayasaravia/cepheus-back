using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.GetLugarEnvioByCode
{
    public class GetLugarEnvioByCodeQueryHandler : IRequestHandler<GetLugarEnvioByCodeQuery, LugarEnvioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetLugarEnvioByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LugarEnvioResponse> Handle(GetLugarEnvioByCodeQuery request, CancellationToken cancellationToken)
        {
            var lugar = await _uow.Logistica.Catalogos.LugaresEnvio.Query()
                .AsNoTracking()
                .Where(l => l.Code == request.Code)
                .Select(l => new LugarEnvioResponse
                {
                    Code = l.Code,
                    Name = l.Name,
                    Address = l.Address,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    CreatedBy = l.CreatedBy,
                    UpdatedBy = l.UpdatedBy,
                    RowVersion = l.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (lugar is null)
            {
                throw new KeyNotFoundException($"Lugar de envío {request.Code} no encontrado.");
            }

            return lugar;
        }
    }
}