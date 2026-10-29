using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.CreateMotivoDevolucion
{
    public class CreateMotivoDevolucionCommandHandler : IRequestHandler<CreateMotivoDevolucionCommand, MotivoDevolucionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateMotivoDevolucionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoDevolucionResponse> Handle(CreateMotivoDevolucionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
               _uow.Comunes.MotivosDevolucion.Query().Select(b => b.Code), length: 2, entityLabel: "Motivos", cancellationToken);

            var motivo = new MotivoDevolucion
            {
                Code = code,
                Name = request.Name.Trim(),
                AffectsStock = request.AffectsStock,
                EsVenta = request.EsVenta,
                IsActive = true
            };

            await _uow.Comunes.MotivosDevolucion.AddAsync(motivo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(motivo);
        }

        internal static MotivoDevolucionResponse Map(MotivoDevolucion motivo) => new()
        {
            Code = motivo.Code,
            Name = motivo.Name,
            AffectsStock = motivo.AffectsStock,
            IsActive = motivo.IsActive,
            CreatedAt = motivo.CreatedAt,
            UpdatedAt = motivo.UpdatedAt,
            CreatedBy = motivo.CreatedBy,
            UpdatedBy = motivo.UpdatedBy,
            RowVersion = motivo.RowVersion
        };
    }
}
