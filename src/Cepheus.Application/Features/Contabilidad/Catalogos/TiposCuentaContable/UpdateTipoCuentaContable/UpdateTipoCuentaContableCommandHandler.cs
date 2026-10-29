using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.UpdateTipoCuentaContable
{
    public class UpdateTipoCuentaContableCommandHandler : IRequestHandler<UpdateTipoCuentaContableCommand, TipoCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaContableResponse> Handle(UpdateTipoCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Catalogos.TiposCuentaContable.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta contable {request.Code} no encontrado.");
            }

            var entity = new TipoCuentaContable
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Catalogos.TiposCuentaContable.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoCuentaContable.CreateTipoCuentaContableCommandHandler.Map(entity);
        }
    }
}
