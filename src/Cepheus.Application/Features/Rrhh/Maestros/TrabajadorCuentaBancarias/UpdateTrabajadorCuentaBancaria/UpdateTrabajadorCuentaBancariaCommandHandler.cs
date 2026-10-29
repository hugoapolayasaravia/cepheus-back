using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.UpdateTrabajadorCuentaBancaria
{
    public class UpdateTrabajadorCuentaBancariaCommandHandler : IRequestHandler<UpdateTrabajadorCuentaBancariaCommand, TrabajadorCuentaBancariaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorCuentaBancariaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorCuentaBancariaResponse> Handle(UpdateTrabajadorCuentaBancariaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorCuentaBancaria {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorCuentaBancaria
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                TipoCuentaCode = request.TipoCuentaCode,
                BancoCode = string.IsNullOrWhiteSpace(request.BancoCode) ? null : request.BancoCode!.Trim(),
                MonedaCode = string.IsNullOrWhiteSpace(request.MonedaCode) ? null : request.MonedaCode!.Trim(),
                NumeroCuenta = string.IsNullOrWhiteSpace(request.NumeroCuenta) ? null : request.NumeroCuenta!.Trim(),
                TipoOperacion = request.TipoOperacion.Trim(),
                Principal = request.Principal,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorCuentaBancaria.CreateTrabajadorCuentaBancariaCommandHandler.Map(entidad);
        }
    }
}
