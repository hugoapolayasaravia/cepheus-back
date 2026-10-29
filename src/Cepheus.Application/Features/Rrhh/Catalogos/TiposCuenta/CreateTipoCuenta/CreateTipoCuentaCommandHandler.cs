using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.CreateTipoCuenta
{
    public class CreateTipoCuentaCommandHandler : IRequestHandler<CreateTipoCuentaCommand, TipoCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCuentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaResponse> Handle(CreateTipoCuentaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposCuenta.Query().Select(t => t.Code), length: 3, entityLabel: "TiposCuenta", cancellationToken);

            var tipoCuenta = new TipoCuenta
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposCuenta.AddAsync(tipoCuenta, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoCuenta);
        }

        internal static TipoCuentaResponse Map(TipoCuenta tipoCuenta) => new()
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