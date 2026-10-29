using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.CreateRegimenLaboral
{
    public class CreateRegimenLaboralCommandHandler : IRequestHandler<CreateRegimenLaboralCommand, RegimenLaboralResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateRegimenLaboralCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenLaboralResponse> Handle(CreateRegimenLaboralCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.RegimenesLaborales.Query().Select(r => r.Code), length: 3, entityLabel: "RegimenesLaborales", cancellationToken);

            var regimenLaboral = new RegimenLaboral
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.RegimenesLaborales.AddAsync(regimenLaboral, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(regimenLaboral);
        }

        internal static RegimenLaboralResponse Map(RegimenLaboral regimenLaboral) => new()
        {
            Code = regimenLaboral.Code,
            Name = regimenLaboral.Name,
            IsActive = regimenLaboral.IsActive,
            CreatedAt = regimenLaboral.CreatedAt,
            UpdatedAt = regimenLaboral.UpdatedAt,
            RowVersion = regimenLaboral.RowVersion
        };
    }
}