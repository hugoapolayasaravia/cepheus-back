using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.UpdateSubCentroEjecutor
{
    public class UpdateSubCentroEjecutorCommandHandler
        : IRequestHandler<UpdateSubCentroEjecutorCommand, SubCentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubCentroEjecutorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroEjecutorResponse> Handle(UpdateSubCentroEjecutorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Subcentro ejecutor {request.Code} no encontrado.");
            }

            var subCentroEjecutor = new SubCentroEjecutor
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                CentroEjecutorCode = request.CentroEjecutorCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Update(subCentroEjecutor);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El subcentro ejecutor fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateSubCentroEjecutor.CreateSubCentroEjecutorCommandHandler.Map(subCentroEjecutor);
        }
    }
}
