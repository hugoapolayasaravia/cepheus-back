using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivoDevolucionById
{
    public class GetMotivoDevolucionByIdQueryHandler : IRequestHandler<GetMotivoDevolucionByIdQuery, MotivoDevolucionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivoDevolucionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoDevolucionResponse> Handle(GetMotivoDevolucionByIdQuery request, CancellationToken cancellationToken)
        {
            var motivo = await _uow.Comunes.MotivosDevolucion.Query()
                .AsNoTracking()
                .Where(m => m.Code == request.Code)
                .Select(m => new MotivoDevolucionResponse
                {
                    Code = m.Code,
                    Name = m.Name,
                    AffectsStock = m.AffectsStock,
                    EsVenta = m.EsVenta,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    CreatedBy = m.CreatedBy,
                    UpdatedBy = m.UpdatedBy,
                    RowVersion = m.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (motivo is null)
            {
                throw new KeyNotFoundException($"Motivo de devolución {request.Code} no encontrado.");
            }

            return motivo;
        }
    }
}
