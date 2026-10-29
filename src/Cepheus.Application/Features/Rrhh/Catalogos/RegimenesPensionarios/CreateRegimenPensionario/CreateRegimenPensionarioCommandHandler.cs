using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.CreateRegimenPensionario
{
    public class CreateRegimenPensionarioCommandHandler
        : IRequestHandler<CreateRegimenPensionarioCommand, RegimenPensionarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateRegimenPensionarioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenPensionarioResponse> Handle(CreateRegimenPensionarioCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.RegimenesPensionarios.Query().Select(r => r.Code), length: 3, entityLabel: "RegimenesPensionarios", cancellationToken);

            var regimenPensionario = new RegimenPensionario
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.RegimenesPensionarios.AddAsync(regimenPensionario, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(regimenPensionario);
        }

        internal static RegimenPensionarioResponse Map(RegimenPensionario regimenPensionario) => new()
        {
            Code = regimenPensionario.Code,
            Name = regimenPensionario.Name,
            IsActive = regimenPensionario.IsActive,
            CreatedAt = regimenPensionario.CreatedAt,
            UpdatedAt = regimenPensionario.UpdatedAt,
            RowVersion = regimenPensionario.RowVersion
        };
    }
}