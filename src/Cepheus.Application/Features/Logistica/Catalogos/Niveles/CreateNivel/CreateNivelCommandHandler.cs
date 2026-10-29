using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.CreateNivel
{
    public class CreateNivelCommandHandler : IRequestHandler<CreateNivelCommand, NivelResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNivelCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelResponse> Handle(CreateNivelCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.Niveles.Query().Select(n => n.Code), length: 2, entityLabel: "Niveles", cancellationToken);

            var nivel = new Nivel
            {
                Code = code,
                Name = request.Name.Trim(),
                FechaInicio = request.FechaInicio ?? DateTime.UtcNow,
                FechaFin = request.FechaFin,
                IsActive = true
            };

            await _uow.Logistica.Catalogos.Niveles.AddAsync(nivel, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nivel);
        }

        internal static NivelResponse Map(Nivel nivel) => new()
        {
            Code = nivel.Code,
            Name = nivel.Name,
            FechaInicio = nivel.FechaInicio,
            FechaFin = nivel.FechaFin,
            IsActive = nivel.IsActive,
            CreatedAt = nivel.CreatedAt,
            UpdatedAt = nivel.UpdatedAt,
            CreatedBy = nivel.CreatedBy,
            UpdatedBy = nivel.UpdatedBy,
            RowVersion = nivel.RowVersion
        };
    }
}
