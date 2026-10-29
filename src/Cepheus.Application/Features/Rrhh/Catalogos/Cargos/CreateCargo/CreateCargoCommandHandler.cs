using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.CreateCargo
{
    public class CreateCargoCommandHandler : IRequestHandler<CreateCargoCommand, CargoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCargoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CargoResponse> Handle(CreateCargoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Cargos.Query().Select(c => c.Code), length: 3, entityLabel: "Cargos", cancellationToken);

            var cargo = new Cargo
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Cargos.AddAsync(cargo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(cargo);
        }

        internal static CargoResponse Map(Cargo cargo) => new()
        {
            Code = cargo.Code,
            Name = cargo.Name,
            IsActive = cargo.IsActive,
            CreatedAt = cargo.CreatedAt,
            UpdatedAt = cargo.UpdatedAt,
            RowVersion = cargo.RowVersion
        };
    }
}