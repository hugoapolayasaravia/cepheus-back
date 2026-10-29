using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.CreateTrabajadorCuentaBancaria
{
    public class CreateTrabajadorCuentaBancariaCommandHandler : IRequestHandler<CreateTrabajadorCuentaBancariaCommand, TrabajadorCuentaBancariaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorCuentaBancariaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorCuentaBancariaResponse> Handle(CreateTrabajadorCuentaBancariaCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorCuentaBancaria
            {
                TrabajadorCode = request.TrabajadorCode,
                TipoCuentaCode = request.TipoCuentaCode,
                BancoCode = string.IsNullOrWhiteSpace(request.BancoCode) ? null : request.BancoCode!.Trim(),
                MonedaCode = string.IsNullOrWhiteSpace(request.MonedaCode) ? null : request.MonedaCode!.Trim(),
                NumeroCuenta = string.IsNullOrWhiteSpace(request.NumeroCuenta) ? null : request.NumeroCuenta!.Trim(),
                TipoOperacion = request.TipoOperacion.Trim(),
                Principal = request.Principal,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorCuentaBancariaResponse Map(TrabajadorCuentaBancaria e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoCuentaCode = e.TipoCuentaCode,
            BancoCode = e.BancoCode,
            MonedaCode = e.MonedaCode,
            NumeroCuenta = e.NumeroCuenta,
            TipoOperacion = e.TipoOperacion,
            Principal = e.Principal,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
