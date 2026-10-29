using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.UpdateTipoCentroFormacion
{
    public class UpdateTipoCentroFormacionCommandHandler
        : IRequestHandler<UpdateTipoCentroFormacionCommand, TipoCentroFormacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoCentroFormacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCentroFormacionResponse> Handle(UpdateTipoCentroFormacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposCentroFormacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de centro de formación {request.Code} no encontrado.");
            }

            var tipoCentroFormacion = new TipoCentroFormacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposCentroFormacion.Update(tipoCentroFormacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de centro de formación fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoCentroFormacionResponse
            {
                Code = tipoCentroFormacion.Code,
                Name = tipoCentroFormacion.Name,
                IsActive = tipoCentroFormacion.IsActive,
                CreatedAt = tipoCentroFormacion.CreatedAt,
                UpdatedAt = tipoCentroFormacion.UpdatedAt,
                RowVersion = tipoCentroFormacion.RowVersion
            };
        }
    }
}