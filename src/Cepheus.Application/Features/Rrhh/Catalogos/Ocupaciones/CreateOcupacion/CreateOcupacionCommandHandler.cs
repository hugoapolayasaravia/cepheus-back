using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.CreateOcupacion
{
    public class CreateOcupacionCommandHandler : IRequestHandler<CreateOcupacionCommand, OcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOcupacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OcupacionResponse> Handle(CreateOcupacionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Ocupaciones.Query().Select(o => o.Code), length: 3, entityLabel: "Ocupaciones", cancellationToken);

            var ocupacion = new Ocupacion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Ocupaciones.AddAsync(ocupacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(ocupacion);
        }

        internal static OcupacionResponse Map(Ocupacion ocupacion) => new()
        {
            Code = ocupacion.Code,
            Name = ocupacion.Name,
            IsActive = ocupacion.IsActive,
            CreatedAt = ocupacion.CreatedAt,
            UpdatedAt = ocupacion.UpdatedAt,
            RowVersion = ocupacion.RowVersion
        };
    }
}