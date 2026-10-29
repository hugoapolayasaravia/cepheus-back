using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CreateDetraccion
{
    public class CreateDetraccionCommandHandler : IRequestHandler<CreateDetraccionCommand, DetraccionResponse>
    {
        private const int MaxAttempts = 3;

        private readonly IUnitOfWork _uow;
        private readonly IDetraccionSettings _settings;

        public CreateDetraccionCommandHandler(IUnitOfWork uow, IDetraccionSettings settings)
        {
            _uow = uow;
            _settings = settings;
        }

        public async Task<DetraccionResponse> Handle(CreateDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);

            for (var attempt = 1; ; attempt++)
            {
                var lot = await SequentialCodeGenerator.NextAsync(
                    _uow.Contabilidad.Transacciones.Detracciones.Query()
                        .Where(d => d.Year == year && d.Period == period)
                        .Select(d => d.LotNumber),
                    length: DetraccionRules.LotLength,
                    entityLabel: "Detracciones del periodo",
                    cancellationToken);

                var generated = await DetraccionGenerator.GenerateAsync(
                    _uow, _settings, year, period, lot, request.StartDate, request.EndDate, cancellationToken);

                var detraccion = new Detraccion
                {
                    Year = year,
                    Period = period,
                    LotNumber = lot,
                    StartDate = request.StartDate.Date,
                    EndDate = request.EndDate.Date,
                    Status = EstadoDetraccion.Pendiente,
                    Items = generated.Items
                };

                DetraccionRules.Recalculate(detraccion);

                await _uow.Contabilidad.Transacciones.Detracciones.AddAsync(detraccion, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxAttempts)
                {
                    // Otro usuario tomó el mismo lote o los mismos documentos: se vuelve a generar.
                    _uow.ClearTracking();
                    continue;
                }

                _uow.ClearTracking();

                var response = (await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken))!;
                response.Warnings = generated.Warnings;
                return response;
            }
        }
    }
}
