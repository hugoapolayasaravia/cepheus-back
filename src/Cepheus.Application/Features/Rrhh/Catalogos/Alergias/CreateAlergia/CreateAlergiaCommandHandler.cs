using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.CreateAlergia
{
    public class CreateAlergiaCommandHandler : IRequestHandler<CreateAlergiaCommand, AlergiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAlergiaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlergiaResponse> Handle(CreateAlergiaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Alergias.Query().Select(a => a.Code), length: 3, entityLabel: "Alergias", cancellationToken);

            var alergia = new Alergia
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Alergias.AddAsync(alergia, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(alergia);
        }

        internal static AlergiaResponse Map(Alergia alergia) => new()
        {
            Code = alergia.Code,
            Name = alergia.Name,
            IsActive = alergia.IsActive,
            CreatedAt = alergia.CreatedAt,
            UpdatedAt = alergia.UpdatedAt,
            RowVersion = alergia.RowVersion
        };
    }
}