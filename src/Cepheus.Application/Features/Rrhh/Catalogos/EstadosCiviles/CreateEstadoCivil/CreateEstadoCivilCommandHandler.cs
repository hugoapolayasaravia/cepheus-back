using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.CreateEstadoCivil
{
    public class CreateEstadoCivilCommandHandler : IRequestHandler<CreateEstadoCivilCommand, EstadoCivilResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEstadoCivilCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoCivilResponse> Handle(CreateEstadoCivilCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.EstadosCiviles.Query().Select(e => e.Code), length: 3, entityLabel: "EstadosCiviles", cancellationToken);

            var estadoCivil = new EstadoCivil
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.EstadosCiviles.AddAsync(estadoCivil, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(estadoCivil);
        }

        internal static EstadoCivilResponse Map(EstadoCivil estadoCivil) => new()
        {
            Code = estadoCivil.Code,
            Name = estadoCivil.Name,
            IsActive = estadoCivil.IsActive,
            CreatedAt = estadoCivil.CreatedAt,
            UpdatedAt = estadoCivil.UpdatedAt,
            RowVersion = estadoCivil.RowVersion
        };
    }
}