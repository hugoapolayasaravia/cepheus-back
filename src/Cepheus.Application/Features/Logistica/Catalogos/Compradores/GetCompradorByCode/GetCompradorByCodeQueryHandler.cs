using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.GetCompradorByCode
{
    public class GetCompradorByCodeQueryHandler : IRequestHandler<GetCompradorByCodeQuery, CompradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCompradorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CompradorResponse> Handle(GetCompradorByCodeQuery request, CancellationToken cancellationToken)
        {
            var comprador = await _uow.Logistica.Catalogos.Compradores.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new CompradorResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (comprador is null)
            {
                throw new KeyNotFoundException($"Comprador {request.Code} no encontrado.");
            }

            return comprador;
        }
    }
}