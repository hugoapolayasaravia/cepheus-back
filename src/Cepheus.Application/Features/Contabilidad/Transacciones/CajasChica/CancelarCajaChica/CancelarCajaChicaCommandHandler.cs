using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CancelarCajaChica
{
    public class CancelarCajaChicaCommandHandler : IRequestHandler<CancelarCajaChicaCommand, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CancelarCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(CancelarCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

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

                    if (caja.Status != EstadoCajaChica.Procesado || string.IsNullOrWhiteSpace(caja.ProvisionAsientoCode))
                    {
                        throw new InvalidOperationException(
                            "Solo se cancela una caja chica Procesada con asiento de provisión. Provisione la caja primero.");
                    }

                    if (!string.IsNullOrWhiteSpace(caja.ReversionAsientoCode))
                    {
                        throw new InvalidOperationException(
                            $"La caja chica ya tiene el asiento de reversión {caja.ReversionAsientoCode.Trim()}.");
                    }

                    var requiereReversion = caja.Type != TipoCajaChica.Gastos;

                    if (requiereReversion)
                    {
                        if (string.IsNullOrWhiteSpace(request.ReversionYear) || string.IsNullOrWhiteSpace(request.ReversionMonth))
                        {
                            throw new InvalidOperationException(
                                $"Una caja de tipo {caja.Type} requiere el año y mes del asiento de reversión.");
                        }

                        var anio = request.ReversionYear.Trim();
                        var mes = int.Parse(request.ReversionMonth.Trim()).ToString("00");

                        var emision = caja.IssueDate.Date;
                        var periodoEmision = emision.Year.ToString("0000") + emision.Month.ToString("00");

                        if (string.CompareOrdinal(anio + mes, periodoEmision) < 0)
                        {
                            throw new InvalidOperationException(
                                "El período de la reversión no puede ser anterior al de la emisión de la caja.");
                        }

                        // Solo se exige período abierto para la reversión (el de la provisión puede estar cerrado).
                        await PeriodoContableRules.EnsureOpenAsync(_uow, anio, mes, ct);

                        var tipoCambio = await TipoCambioRules.GetVentaAsync(_uow, TipoCambioRules.MonedaReferencia, emision, ct);

                        if (!tipoCambio.HasValue || tipoCambio.Value <= 0)
                        {
                            throw new InvalidOperationException(
                                $"Tipo de cambio de la caja chica {code} no registrado para el {emision:dd/MM/yyyy}.");
                        }

                        // Misma emisión: la fecha del asiento es la de la caja; mes posterior: primer día del mes.
                        var fechaAsiento = anio + mes == periodoEmision
                            ? emision
                            : new DateTime(int.Parse(anio), int.Parse(mes), 1);

                        var plantilla = await CajaChicaAsientoService.GetPlantillaAsync(_uow, ProcesoAsiento.ReversionCajaChica, ct);
                        var proveedores = await CajaChicaAsientoService.LoadProveedoresAsync(_uow, caja.Details, ct);
                        var fuentes = await CajaChicaAsientoService.BuildFuentesAsync(_uow, caja, plantilla, proveedores, ct);

                        var asiento = await CajaChicaAsientoService.BuildAsientoAsync(
                            _uow, plantilla, fuentes, anio, mes, fechaAsiento, $"CANCELACION CAJA CHICA {code}",
                            tipoCambio.Value, aplicarDestinos: false, ct);

                        caja.ReversionAsientoCode = asiento.Codigo;
                    }

                    // Cancelación de los documentos de compra de la caja
                    var hoy = DateTime.UtcNow.Date;

                    foreach (var d in caja.Details.Where(x => x.ComprobantePagoCode != CajaChicaRules.ComprobanteSinDocumento))
                    {
                        var documento = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                            .FirstOrDefaultAsync(x => x.ComprobantePagoCode == d.ComprobantePagoCode
                                                      && x.NumeroDocumento == d.NumeroDocumento
                                                      && x.ProveedorCode == d.ProveedorCode
                                                      && x.Origen == OrigenDocumentoCompra.CajaChica, ct)
                            ?? throw new InvalidOperationException(
                                $"No se encontró el documento de compra {d.ComprobantePagoCode} {d.NumeroDocumento} de la caja.");

                        documento.PagadoSol = documento.TotalSol;
                        documento.Estado = EstadoDocumentoCompra.Cancelado;
                        documento.FechaCancelacion = hoy;
                    }

                    caja.Status = EstadoCajaChica.Cancelado;

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
