using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.UpdateNivelTrabajador
{
    public class UpdateNivelTrabajadorCommandHandler
        : IRequestHandler<UpdateNivelTrabajadorCommand, NivelTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNivelTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelTrabajadorResponse> Handle(UpdateNivelTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.NivelesTrabajador.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            var nivelTrabajador = new NivelTrabajador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.NivelesTrabajador.Update(nivelTrabajador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El nivel fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NivelTrabajadorResponse
            {
                Code = nivelTrabajador.Code,
                Name = nivelTrabajador.Name,
                IsActive = nivelTrabajador.IsActive,
                CreatedAt = nivelTrabajador.CreatedAt,
                UpdatedAt = nivelTrabajador.UpdatedAt,
                RowVersion = nivelTrabajador.RowVersion
            };
        }
    }
}