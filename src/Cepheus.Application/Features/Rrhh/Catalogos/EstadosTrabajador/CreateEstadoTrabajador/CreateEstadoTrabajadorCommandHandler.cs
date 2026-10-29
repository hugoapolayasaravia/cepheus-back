using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.CreateEstadoTrabajador
{
    public class CreateEstadoTrabajadorCommandHandler : IRequestHandler<CreateEstadoTrabajadorCommand, EstadoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEstadoTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoTrabajadorResponse> Handle(CreateEstadoTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.EstadosTrabajador.Query().Select(e => e.Code), length: 3, entityLabel: "EstadosTrabajador", cancellationToken);

            var estadoTrabajador = new EstadoTrabajador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.EstadosTrabajador.AddAsync(estadoTrabajador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(estadoTrabajador);
        }

        internal static EstadoTrabajadorResponse Map(EstadoTrabajador estadoTrabajador) => new()
        {
            Code = estadoTrabajador.Code,
            Name = estadoTrabajador.Name,
            IsActive = estadoTrabajador.IsActive,
            CreatedAt = estadoTrabajador.CreatedAt,
            UpdatedAt = estadoTrabajador.UpdatedAt,
            RowVersion = estadoTrabajador.RowVersion
        };
    }
}