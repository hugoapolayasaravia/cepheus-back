using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.UpdateNivel
{
    public class UpdateNivelCommandHandler : IRequestHandler<UpdateNivelCommand, NivelResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNivelCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelResponse> Handle(UpdateNivelCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.Niveles.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            var nivel = new Nivel
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                UpdatedAt = current.UpdatedAt,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.Niveles.Update(nivel);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El nivel fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateNivel.CreateNivelCommandHandler.Map(nivel);
        }
    }
}
