using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.UpdateCentroCosto
{
    public class UpdateCentroCostoCommandHandler : IRequestHandler<UpdateCentroCostoCommand, CentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCentroCostoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroCostoResponse> Handle(UpdateCentroCostoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.CentrosCosto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Centro de costo {request.Code} no encontrado.");
            }

            var centro = new CentroCosto
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.CentrosCosto.Update(centro);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El centro de costo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new CentroCostoResponse
            {
                Code = centro.Code,
                Name = centro.Name,
                PlantaCode = centro.PlantaCode,
                IsActive = centro.IsActive,
                CreatedAt = centro.CreatedAt,
                UpdatedAt = centro.UpdatedAt,
                RowVersion = centro.RowVersion
            };
        }
    }
}
