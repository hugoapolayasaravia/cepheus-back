using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.CreateFamilia
{
    public class CreateFamiliaCommandHandler : IRequestHandler<CreateFamiliaCommand, FamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFamiliaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FamiliaResponse> Handle(CreateFamiliaCommand request, CancellationToken cancellationToken)
        {
            //var code = await SequentialCodeGenerator.NextAsync(
            //    _uow.Logistica.Catalogos.Familias.Query().Select(f => f.Code), length: 2, entityLabel: "Familias", cancellationToken);

            var familia = new Familia
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.Familias.AddAsync(familia, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(familia);
        }

        internal static FamiliaResponse Map(Familia familia) => new()
        {
            Code = familia.Code,
            Name = familia.Name,
            IsActive = familia.IsActive,
            CreatedAt = familia.CreatedAt,
            UpdatedAt = familia.UpdatedAt,
            CreatedBy = familia.CreatedBy,
            UpdatedBy = familia.UpdatedBy,
            RowVersion = familia.RowVersion
        };
    }
}