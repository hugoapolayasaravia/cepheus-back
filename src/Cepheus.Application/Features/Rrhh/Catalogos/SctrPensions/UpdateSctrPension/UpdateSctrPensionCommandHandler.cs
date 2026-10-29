using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.UpdateSctrPension
{
    public class UpdateSctrPensionCommandHandler : IRequestHandler<UpdateSctrPensionCommand, SctrPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSctrPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrPensionResponse> Handle(UpdateSctrPensionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.SctrsPension.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cobertura de pensión SCTR {request.Code} no encontrada.");
            }

            var sctrPension = new SctrPension
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.SctrsPension.Update(sctrPension);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cobertura de pensión SCTR fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SctrPensionResponse
            {
                Code = sctrPension.Code,
                Name = sctrPension.Name,
                IsActive = sctrPension.IsActive,
                CreatedAt = sctrPension.CreatedAt,
                UpdatedAt = sctrPension.UpdatedAt,
                RowVersion = sctrPension.RowVersion
            };
        }
    }
}