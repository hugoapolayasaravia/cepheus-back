using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.UpdateTipoVia
{
    public class UpdateTipoViaCommandHandler : IRequestHandler<UpdateTipoViaCommand, TipoViaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoViaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoViaResponse> Handle(UpdateTipoViaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposVia.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de vía {request.Code} no encontrado.");
            }

            var tipoVia = new TipoVia
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Abbreviation = string.IsNullOrWhiteSpace(request.Abbreviation) ? null : request.Abbreviation.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposVia.Update(tipoVia);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de vía fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoViaResponse
            {
                Code = tipoVia.Code,
                Name = tipoVia.Name,
                Abbreviation = tipoVia.Abbreviation,
                IsActive = tipoVia.IsActive,
                CreatedAt = tipoVia.CreatedAt,
                UpdatedAt = tipoVia.UpdatedAt,
                RowVersion = tipoVia.RowVersion
            };
        }
    }
}