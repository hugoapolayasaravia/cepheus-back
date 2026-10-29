using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccion
{
    public class UpdateDetraccionCommandHandler : IRequestHandler<UpdateDetraccionCommand, DetraccionResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IDetraccionSettings _settings;

        public UpdateDetraccionCommandHandler(IUnitOfWork uow, IDetraccionSettings settings)
        {
            _uow = uow;
            _settings = settings;
        }

        public async Task<DetraccionResponse> Handle(UpdateDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);
            var start = request.StartDate.Date;
            var end = request.EndDate.Date;

            List<string> warnings;

            try
            {
                warnings = await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                        .Include(d => d.Items)
                        .ThenInclude(i => i.Documents)
                        .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, ct);

                    if (detraccion is null)
                    {
                        throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
                    }

                    // El legacy permitía regenerar un lote ya emitido y lo dejaba otra vez Pendiente
                    // perdiendo la fecha de depósito: ahora solo se modifica un lote Pendiente.
                    DetraccionRules.EnsurePendiente(detraccion, "modificar");

                    if (detraccion.StartDate.Date == start && detraccion.EndDate.Date == end)
                    {
                        return new List<string>();
                    }

                    // Primero se eliminan los ítems actuales (sus documentos quedan libres), luego se
                    // genera de nuevo: las claves nuevas pueden coincidir con las anteriores.
                    foreach (var item in detraccion.Items.ToList())
                    {
                        foreach (var document in item.Documents.ToList())
                        {
                            _uow.Contabilidad.Transacciones.DetraccionDocumentos.Remove(document);
                        }

                        _uow.Contabilidad.Transacciones.DetraccionItems.Remove(item);
                    }

                    await _uow.SaveChangesAsync(ct);

                    var generated = await DetraccionGenerator.GenerateAsync(
                        _uow, _settings, year, period, lot, start, end, ct);

                    detraccion.StartDate = start;
                    detraccion.EndDate = end;

                    foreach (var item in generated.Items)
                    {
                        detraccion.Items.Add(item);
                    }

                    DetraccionRules.Recalculate(detraccion);

                    await _uow.SaveChangesAsync(ct);

                    return generated.Warnings;
                },
                System.Data.IsolationLevel.ReadCommitted,
                cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "La detracción fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }
            catch
            {
                _uow.ClearTracking();
                throw;
            }

            _uow.ClearTracking();

            var response = (await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken))!;
            response.Warnings = warnings;
            return response;
        }
    }
}
