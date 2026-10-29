using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.UpdateEstadoCivil
{
    public class UpdateEstadoCivilCommandHandler : IRequestHandler<UpdateEstadoCivilCommand, EstadoCivilResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEstadoCivilCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoCivilResponse> Handle(UpdateEstadoCivilCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.EstadosCiviles.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Estado civil {request.Code} no encontrado.");
            }

            var estadoCivil = new EstadoCivil
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.EstadosCiviles.Update(estadoCivil);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El estado civil fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new EstadoCivilResponse
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
}