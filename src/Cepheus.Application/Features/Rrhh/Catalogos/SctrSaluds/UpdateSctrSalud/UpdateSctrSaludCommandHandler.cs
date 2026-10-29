using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.UpdateSctrSalud
{
    public class UpdateSctrSaludCommandHandler : IRequestHandler<UpdateSctrSaludCommand, SctrSaludResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSctrSaludCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrSaludResponse> Handle(UpdateSctrSaludCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.SctrsSalud.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cobertura de salud SCTR {request.Code} no encontrada.");
            }

            var sctrSalud = new SctrSalud
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.SctrsSalud.Update(sctrSalud);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cobertura de salud SCTR fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SctrSaludResponse
            {
                Code = sctrSalud.Code,
                Name = sctrSalud.Name,
                IsActive = sctrSalud.IsActive,
                CreatedAt = sctrSalud.CreatedAt,
                UpdatedAt = sctrSalud.UpdatedAt,
                RowVersion = sctrSalud.RowVersion
            };
        }
    }
}