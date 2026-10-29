using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable
{
    public class CreateCuentaContableCommandHandler : IRequestHandler<CreateCuentaContableCommand, CuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaContableResponse> Handle(CreateCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var entity = new CuentaContable
            {
                Code = request.Code.Trim(),
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
                IsActive = true
            };

            await _uow.Contabilidad.Maestros.CuentasContables.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static CuentaContableResponse Map(CuentaContable e) => new()
        {
            Code = e.Code.TrimEnd(),
            Name = e.Name.TrimEnd(),
            TipoCuentaCode = e.TipoCuentaCode.TrimEnd(),
            TipoAnexoCode = e.TipoAnexoCode?.TrimEnd(),
            LinkAccount1Code = e.LinkAccount1Code.TrimEnd(),
            LinkAccount2Code = e.LinkAccount2Code.TrimEnd(),
            CurrencyCode = e.CurrencyCode.TrimEnd(),
            AcmFlag = e.AcmFlag?.TrimEnd(),
            PilotFlag = e.PilotFlag?.TrimEnd(),
            GainsLossesFlag = e.GainsLossesFlag?.TrimEnd(),
            ClosingAccountCode = e.ClosingAccountCode.TrimEnd(),
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
