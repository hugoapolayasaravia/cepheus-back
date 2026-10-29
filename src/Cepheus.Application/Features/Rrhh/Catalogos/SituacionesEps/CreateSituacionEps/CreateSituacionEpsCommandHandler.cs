using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.CreateSituacionEps
{
    public class CreateSituacionEpsCommandHandler : IRequestHandler<CreateSituacionEpsCommand, SituacionEpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSituacionEpsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SituacionEpsResponse> Handle(CreateSituacionEpsCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.SituacionesEps.Query().Select(s => s.Code), length: 3, entityLabel: "SituacionesEps", cancellationToken);

            var situacionEps = new SituacionEps
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.SituacionesEps.AddAsync(situacionEps, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(situacionEps);
        }

        internal static SituacionEpsResponse Map(SituacionEps situacionEps) => new()
        {
            Code = situacionEps.Code,
            Name = situacionEps.Name,
            IsActive = situacionEps.IsActive,
            CreatedAt = situacionEps.CreatedAt,
            UpdatedAt = situacionEps.UpdatedAt,
            RowVersion = situacionEps.RowVersion
        };
    }
}