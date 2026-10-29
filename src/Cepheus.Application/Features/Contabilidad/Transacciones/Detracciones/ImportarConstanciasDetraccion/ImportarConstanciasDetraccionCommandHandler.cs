using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.ImportarConstanciasDetraccion
{
    /// <summary>
    /// Legacy: botón "Importa" + SP com_actualiza_constancia_doc_prv. Por cada constancia del archivo se
    /// busca el proveedor por RUC y se actualizan los documentos del lote de ese proveedor y tipo de bien.
    ///
    /// Cambios frente al legacy: solo se actualizan los documentos marcados para exportar (los demás se
    /// registran a mano), y los RUC no encontrados o sin documentos se informan en lugar de ignorarse.
    /// </summary>
    public class ImportarConstanciasDetraccionCommandHandler
        : IRequestHandler<ImportarConstanciasDetraccionCommand, ImportarConstanciasDetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public ImportarConstanciasDetraccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportarConstanciasDetraccionResponse> Handle(
            ImportarConstanciasDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var status = await _uow.Contabilidad.Transacciones.Detracciones.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot)
                .Select(d => (EstadoDetraccion?)d.Status)
                .FirstOrDefaultAsync(cancellationToken);

            if (status is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            if (status == EstadoDetraccion.Pendiente)
            {
                throw new InvalidOperationException(
                    "El lote aún no fue emitido: genere el archivo de depósito antes de importar constancias.");
            }

            var records = DetraccionConstanciaParser.Parse(request.Content);

            if (records.Count == 0)
            {
                throw new InvalidOperationException(
                    "No se encontraron constancias en el archivo. Verifique que sea el archivo de constancias del banco.");
            }

            var rucs = records.Select(r => r.Ruc).Distinct().ToList();

            var providers = await _uow.Logistica.Maestros.Proveedores.Query().AsNoTracking()
                .Where(p => rucs.Contains(p.DocumentNumber))
                .Select(p => new { p.Code, p.DocumentNumber })
                .ToListAsync(cancellationToken);

            var response = new ImportarConstanciasDetraccionResponse { Records = records.Count };

            foreach (var record in records)
            {
                var provider = providers.FirstOrDefault(p => p.DocumentNumber.Trim() == record.Ruc);

                if (provider is null)
                {
                    response.Errors.Add($"RUC {record.Ruc}: no existe un proveedor con ese RUC.");
                    continue;
                }

                if (record.Operacion.Length > DetraccionRules.OperacionMaxLength
                    || record.Constancia.Length > DetraccionRules.ConstanciaMaxLength)
                {
                    response.Errors.Add(
                        $"RUC {record.Ruc}: la operación '{record.Operacion}' o la constancia '{record.Constancia}' " +
                        "exceden los 10 caracteres.");
                    continue;
                }

                var documents = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query()
                    .Include(d => d.DocumentoCompra)
                    .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot
                                && d.ProveedorCode == provider.Code
                                && d.IncludeInExport
                                && d.Item.TipoBienCode == record.Bien)
                    .ToListAsync(cancellationToken);

                if (documents.Count == 0)
                {
                    response.Errors.Add(
                        $"RUC {record.Ruc}, bien o servicio {record.Bien}: el lote no tiene documentos exportados para esa combinación.");
                    continue;
                }

                foreach (var document in documents)
                {
                    document.DocumentoCompra.OperacionDetraccion = record.Operacion;
                    document.DocumentoCompra.ConstanciaDetraccion = record.Constancia;
                    document.DocumentoCompra.FechaConstanciaDetraccion = record.Fecha?.Date;
                }

                response.UpdatedDocuments += documents.Count;
            }

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "Algún documento fue modificado por otro proceso. Intente importar nuevamente.");
            }

            _uow.ClearTracking();

            return response;
        }
    }
}
