using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.UpdateTipoTrabajador
{
    public class UpdateTipoTrabajadorCommandHandler : IRequestHandler<UpdateTipoTrabajadorCommand, TipoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTrabajadorResponse> Handle(UpdateTipoTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposTrabajador.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de trabajador {request.Code} no encontrado.");
            }

            var tipoTrabajador = new TipoTrabajador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposTrabajador.Update(tipoTrabajador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de trabajador fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoTrabajadorResponse
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
}