using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.UpdateTrabajadorDependiente
{
    public class UpdateTrabajadorDependienteCommandHandler : IRequestHandler<UpdateTrabajadorDependienteCommand, TrabajadorDependienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorDependienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDependienteResponse> Handle(UpdateTrabajadorDependienteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorDependientes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorDependiente {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorDependiente
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                Nombre = request.Nombre.Trim(),
                ParentescoCode = request.ParentescoCode,
                FechaNacimiento = request.FechaNacimiento,
                Documento = string.IsNullOrWhiteSpace(request.Documento) ? null : request.Documento!.Trim(),
                Asegurado = request.Asegurado,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorDependientes.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorDependiente.CreateTrabajadorDependienteCommandHandler.Map(entidad);
        }
    }
}
