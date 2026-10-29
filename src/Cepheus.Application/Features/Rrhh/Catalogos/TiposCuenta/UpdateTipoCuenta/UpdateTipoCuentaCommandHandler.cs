using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.UpdateTipoCuenta
{
    public class UpdateTipoCuentaCommandHandler : IRequestHandler<UpdateTipoCuentaCommand, TipoCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoCuentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaResponse> Handle(UpdateTipoCuentaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposCuenta.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta {request.Code} no encontrado.");
            }

            var tipoCuenta = new TipoCuenta
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposCuenta.Update(tipoCuenta);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de cuenta fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoCuentaResponse
            {
                Code = tipoCuenta.Code,
                Name = tipoCuenta.Name,
                IsActive = tipoCuenta.IsActive,
                CreatedAt = tipoCuenta.CreatedAt,
                UpdatedAt = tipoCuenta.UpdatedAt,
                RowVersion = tipoCuenta.RowVersion
            };
        }
    }
}