using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.CreateSubFamilia
{
    public class CreateSubFamiliaCommandHandler : IRequestHandler<CreateSubFamiliaCommand, SubFamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubFamiliaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubFamiliaResponse> Handle(CreateSubFamiliaCommand request, CancellationToken cancellationToken)
        {
            var familiaCode = request.FamiliaCode.Trim().ToUpperInvariant();

            var code = await SequentialCodeGenerator.NextChildAsync(
                _uow.Logistica.Catalogos.SubFamilias.Query().Select(s => s.Code),
                prefix: familiaCode,
                suffixLength: 2,
                entityLabel: "SubFamilias",
                cancellationToken);

            var subFamilia = new SubFamilia
            {
                Code = code,
                FamiliaCode = familiaCode,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.SubFamilias.AddAsync(subFamilia, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(subFamilia);
        }

        internal static SubFamiliaResponse Map(SubFamilia subFamilia) => new()
        {
            Code = subFamilia.Code,
            FamiliaCode = subFamilia.FamiliaCode,
            Name = subFamilia.Name,
            IsActive = subFamilia.IsActive,
            CreatedAt = subFamilia.CreatedAt,
            UpdatedAt = subFamilia.UpdatedAt,
            CreatedBy = subFamilia.CreatedBy,
            UpdatedBy = subFamilia.UpdatedBy,
            RowVersion = subFamilia.RowVersion
        };
    }
}