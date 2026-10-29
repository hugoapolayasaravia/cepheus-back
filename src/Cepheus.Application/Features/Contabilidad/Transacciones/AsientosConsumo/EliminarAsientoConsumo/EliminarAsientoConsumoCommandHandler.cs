using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.EliminarAsientoConsumo
{
    /// <summary>
    /// Elimina el asiento de consumo (legacy: evento ue_eliminaasiento de w_con_asientosconsumo): libera las líneas de
    /// vale y los vales que tenían el asiento y lo anula (período abierto, no tipo 04), en una sola transacción.
    /// </summary>
    public class EliminarAsientoConsumoCommandHandler
        : IRequestHandler<EliminarAsientoConsumoCommand, AsientoConsumoResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public EliminarAsientoConsumoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoConsumoResultResponse> Handle(
            EliminarAsientoConsumoCommand request, CancellationToken cancellationToken)
            => _uow.ExecuteInTransactionAsync(
                token => EliminarAsync(request.AsientoCodigo, token),
                IsolationLevel.ReadCommitted,
                cancellationToken);

        private async Task<AsientoConsumoResultResponse> EliminarAsync(string asientoCodigo, CancellationToken ct)
        {
            var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(asientoCodigo);
            var codigo = anio + mes + tipo + numero;

            var lineas = await _uow.Logistica.Transacciones.ValeDetalles.Query()
                .CountAsync(d => d.AsientoContable == codigo, ct);

            var vales = await _uow.Logistica.Transacciones.Vales.Query()
                .CountAsync(v => v.AsientoContable == codigo, ct);

            if (lineas == 0 && vales == 0)
            {
                throw new InvalidOperationException(
                    $"El asiento {codigo} no corresponde a un consumo de vales (ningún vale lo referencia).");
            }

            // La anulación también libera las líneas de vale y los vales.
            await AsientoAnulacionService.AnularAsync(_uow, codigo, ct);

            await _uow.SaveChangesAsync(ct);

            return new AsientoConsumoResultResponse
            {
                PlantaCode = string.Empty,
                Success = true,
                AsientoCodigo = codigo,
                Lineas = lineas,
                Vales = vales,
                Message = $"Asiento {codigo} anulado."
            };
        }
    }
}
