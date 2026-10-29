using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.UpdateTipoVale
{
    public class UpdateTipoValeCommandHandler : IRequestHandler<UpdateTipoValeCommand, TipoValeResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoValeCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValeResponse> Handle(UpdateTipoValeCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.TiposVale.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de vale {request.Code} no encontrado.");
            }

            var tipoVale = new TipoVale
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.TiposVale.Update(tipoVale);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de vale fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoValeResponse
            {
                Code = tipoVale.Code,
                Name = tipoVale.Name,
                IsActive = tipoVale.IsActive,
                CreatedAt = tipoVale.CreatedAt,
                UpdatedAt = tipoVale.UpdatedAt,
                CreatedBy = tipoVale.CreatedBy,
                UpdatedBy = tipoVale.UpdatedBy,
                RowVersion = tipoVale.RowVersion
            };
        }
    }
}