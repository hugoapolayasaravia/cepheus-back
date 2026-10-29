using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.UpdateCajaChica
{
    public class UpdateCajaChicaCommandHandler : IRequestHandler<UpdateCajaChicaCommand, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(UpdateCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();
            var planta = CajaChicaRules.Normalize(request.PlantaCode);
            var tipo = CajaChicaRules.ParseTipo(request.Type);
            var inputs = request.Details ?? new List<CajaChicaDetalleInput>();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var caja = await _uow.Contabilidad.Transacciones.CajasChica.Query()
                        .Include(c => c.Details)
                        .FirstOrDefaultAsync(c => c.Code == code, ct);

                    if (caja is null)
                    {
                        throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
                    }

                    if (!caja.RowVersion.SequenceEqual(request.RowVersion))
                    {
                        throw new InvalidOperationException(
                            "La caja chica fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
                    }

                    if (caja.Status != EstadoCajaChica.Pendiente || CajaChicaRules.HasAsientos(caja))
                    {
                        throw new InvalidOperationException(
                            "Solo se puede modificar una caja chica Pendiente y sin asientos contables.");
                    }

                    // Reemplazo de líneas: primero se eliminan, luego se insertan.
                    foreach (var detail in caja.Details.ToList())
                    {
                        _uow.Contabilidad.Transacciones.CajasChicaDetalle.Remove(detail);
                    }

                    await _uow.SaveChangesAsync(ct);

                    var lines = await CajaChicaLineBuilder.BuildAsync(_uow, code, planta, inputs, ct);
                    var (monto, total) = CajaChicaRules.ComputeTotals(lines, TipoCambioRules.MonedaBase);

                    caja.PlantaCode = planta;
                    caja.TrabajadorCode = request.TrabajadorCode.Trim();
                    caja.IssueDate = request.IssueDate;
                    caja.Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim();
                    caja.Type = tipo;
                    caja.MontoSol = monto;
                    caja.TotalSol = total;

                    foreach (var line in lines)
                    {
                        await _uow.Contabilidad.Transacciones.CajasChicaDetalle.AddAsync(line, ct);
                    }

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La caja chica fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
