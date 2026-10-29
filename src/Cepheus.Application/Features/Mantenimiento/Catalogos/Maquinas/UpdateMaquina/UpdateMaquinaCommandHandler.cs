using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.UpdateMaquina
{
    public class UpdateMaquinaCommandHandler : IRequestHandler<UpdateMaquinaCommand, MaquinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMaquinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MaquinaResponse> Handle(UpdateMaquinaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Máquina {request.Code} no encontrada.");
            }

            var maquina = new Maquina
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Maquinas.Update(maquina);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La máquina fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new MaquinaResponse
            {
                Code = maquina.Code,
                Name = maquina.Name,
                IsActive = maquina.IsActive,
                CreatedAt = maquina.CreatedAt,
                UpdatedAt = maquina.UpdatedAt,
                RowVersion = maquina.RowVersion
            };
        }
    }
}
