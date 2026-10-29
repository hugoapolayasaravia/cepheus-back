using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.CreateSexo
{
    public class CreateSexoCommandHandler : IRequestHandler<CreateSexoCommand, SexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SexoResponse> Handle(CreateSexoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Sexos.Query().Select(s => s.Code), length: 3, entityLabel: "Sexos", cancellationToken);

            var sexo = new Sexo
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Sexos.AddAsync(sexo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(sexo);
        }

        internal static SexoResponse Map(Sexo sexo) => new()
        {
            Code = sexo.Code,
            Name = sexo.Name,
            IsActive = sexo.IsActive,
            CreatedAt = sexo.CreatedAt,
            UpdatedAt = sexo.UpdatedAt,
            RowVersion = sexo.RowVersion
        };
    }
}