using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.CreateSubOcupacion
{
    public class CreateSubOcupacionCommandHandler : IRequestHandler<CreateSubOcupacionCommand, SubOcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubOcupacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubOcupacionResponse> Handle(CreateSubOcupacionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.SubOcupaciones.Query().Select(s => s.Code), length: 3, entityLabel: "SubOcupaciones", cancellationToken);

            var subOcupacion = new SubOcupacion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.SubOcupaciones.AddAsync(subOcupacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(subOcupacion);
        }

        internal static SubOcupacionResponse Map(SubOcupacion subOcupacion) => new()
        {
            Code = subOcupacion.Code,
            Name = subOcupacion.Name,
            IsActive = subOcupacion.IsActive,
            CreatedAt = subOcupacion.CreatedAt,
            UpdatedAt = subOcupacion.UpdatedAt,
            RowVersion = subOcupacion.RowVersion
        };
    }
}