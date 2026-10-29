using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.UpdateCuentaContable
{
    public class UpdateCuentaContableCommandHandler : IRequestHandler<UpdateCuentaContableCommand, CuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaContableResponse> Handle(UpdateCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Maestros.CuentasContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cuenta contable {request.Code} no encontrado.");
            }

            var entity = new CuentaContable
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                TipoCuentaCode = request.TipoCuentaCode.Trim(),
                TipoAnexoCode = request.TipoAnexoCode?.Trim(),
                LinkAccount1Code = (request.LinkAccount1Code?.Trim() ?? string.Empty),
                LinkAccount2Code = (request.LinkAccount2Code?.Trim() ?? string.Empty),
                CurrencyCode = request.CurrencyCode.Trim().ToUpper(),
                AcmFlag = request.AcmFlag?.Trim().ToUpper(),
                PilotFlag = request.PilotFlag?.Trim().ToUpper(),
                GainsLossesFlag = request.GainsLossesFlag?.Trim().ToUpper(),
                ClosingAccountCode = (request.ClosingAccountCode?.Trim() ?? string.Empty),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Maestros.CuentasContables.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateCuentaContable.CreateCuentaContableCommandHandler.Map(entity);
        }
    }
}
