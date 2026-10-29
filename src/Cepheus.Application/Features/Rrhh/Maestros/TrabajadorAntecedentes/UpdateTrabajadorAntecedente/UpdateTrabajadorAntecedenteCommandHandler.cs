using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.UpdateTrabajadorAntecedente
{
    public class UpdateTrabajadorAntecedenteCommandHandler : IRequestHandler<UpdateTrabajadorAntecedenteCommand, TrabajadorAntecedenteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorAntecedenteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorAntecedenteResponse> Handle(UpdateTrabajadorAntecedenteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorAntecedentes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorAntecedente {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorAntecedente
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                TieneAntecedentes = request.TieneAntecedentes,
                Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion!.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorAntecedentes.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorAntecedente.CreateTrabajadorAntecedenteCommandHandler.Map(entidad);
        }
    }
}
