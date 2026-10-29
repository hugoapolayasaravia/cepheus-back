using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.UpdateTipoSctr
{
    public class UpdateTipoSctrCommandHandler : IRequestHandler<UpdateTipoSctrCommand, TipoSctrResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoSctrCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSctrResponse> Handle(UpdateTipoSctrCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposSctr.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de SCTR {request.Code} no encontrado.");
            }

            var tipoSctr = new TipoSctr
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposSctr.Update(tipoSctr);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de SCTR fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoSctrResponse
            {
                Code = tipoSctr.Code,
                Name = tipoSctr.Name,
                IsActive = tipoSctr.IsActive,
                CreatedAt = tipoSctr.CreatedAt,
                UpdatedAt = tipoSctr.UpdatedAt,
                RowVersion = tipoSctr.RowVersion
            };
        }
    }
}