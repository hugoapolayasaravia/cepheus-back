using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.GetTrabajadorCuentaBancariasByTrabajador
{
    public class GetTrabajadorCuentaBancariasByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorCuentaBancariasByTrabajadorQuery, List<TrabajadorCuentaBancariaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorCuentaBancariasByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorCuentaBancariaResponse>> Handle(GetTrabajadorCuentaBancariasByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorCuentaBancariaResponse Map(TrabajadorCuentaBancaria e) => new()
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
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
