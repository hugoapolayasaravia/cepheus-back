using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivoDevolucionArticuloById
{
    public class GetMotivoDevolucionArticuloByIdQueryHandler : IRequestHandler<GetMotivoDevolucionArticuloByIdQuery, MotivoDevolucionArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivoDevolucionArticuloByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoDevolucionArticuloResponse> Handle(GetMotivoDevolucionArticuloByIdQuery request, CancellationToken cancellationToken)
        {
            var motivo = await _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Query()
                .AsNoTracking()
                .Where(m => m.Code == request.Code)
                .Select(m => new MotivoDevolucionArticuloResponse
                {
                    Code = m.Code,
                    Name = m.Name,
                    AffectsStock = m.AffectsStock,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
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
