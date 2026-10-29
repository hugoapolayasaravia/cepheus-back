using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientoContableByCode
{
    public class GetAsientoContableByCodeQueryHandler
        : IRequestHandler<GetAsientoContableByCodeQuery, AsientoContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAsientoContableByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AsientoContableResponse> Handle(
            GetAsientoContableByCodeQuery request, CancellationToken cancellationToken)
        {
            var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(request.Codigo);

            var asiento = await _uow.Contabilidad.Transacciones.AsientosContables.Query()
                .AsNoTracking()
                .Include(a => a.TipoAsiento)
                .Include(a => a.Detalles)
                .FirstOrDefaultAsync(
                    a => a.Anio == anio && a.Mes == mes && a.TipoAsientoCode == tipo && a.Numero == numero,
                    cancellationToken);

            if (asiento is null)
            {
                throw new KeyNotFoundException($"Asiento {request.Codigo.Trim()} no encontrado.");
            }

            return AsientoContableMapper.Map(asiento);
        }
    }
}
