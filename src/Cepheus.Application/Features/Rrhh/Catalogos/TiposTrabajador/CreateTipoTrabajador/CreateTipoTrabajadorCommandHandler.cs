using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.CreateTipoTrabajador
{
    public class CreateTipoTrabajadorCommandHandler : IRequestHandler<CreateTipoTrabajadorCommand, TipoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTrabajadorResponse> Handle(CreateTipoTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposTrabajador.Query().Select(t => t.Code), length: 3, entityLabel: "TiposTrabajador", cancellationToken);

            var tipoTrabajador = new TipoTrabajador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposTrabajador.AddAsync(tipoTrabajador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoTrabajador);
        }

        internal static TipoTrabajadorResponse Map(TipoTrabajador tipoTrabajador) => new()
        {
            Code = tipoTrabajador.Code,
            Name = tipoTrabajador.Name,
            IsActive = tipoTrabajador.IsActive,
            CreatedAt = tipoTrabajador.CreatedAt,
            UpdatedAt = tipoTrabajador.UpdatedAt,
            RowVersion = tipoTrabajador.RowVersion
        };
    }
}