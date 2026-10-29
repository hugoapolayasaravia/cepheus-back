// Cepheus.Application/Features/Logistica/Transacciones/Guias/CreateGuia/CreateGuiaCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.CreateGuia
{
    /// <summary>
    /// La guía NO mueve stock. El correlativo se toma de Planta.GuiaNum y se
    /// actualiza en la MISMA llamada a SaveChanges que inserta la guía (una
    /// sola transacción): si la guía falla, el número no se consume, y si dos
    /// usuarios emiten a la vez en la misma planta, Planta.RowVersion hace
    /// fallar al segundo con un conflicto de concurrencia (no hay duplicados
    /// ni saltos de numeración).
    /// </summary>
    public class CreateGuiaCommandHandler : IRequestHandler<CreateGuiaCommand, GuiaResponse>
    {
        private const int MaxPuntoPartidaLength = 70;

        private readonly IUnitOfWork _uow;

        public CreateGuiaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(CreateGuiaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            // Planta con tracking: se le actualiza GuiaNum junto con la guía.
            var planta = await _uow.Comunes.Plantas.Query()
                .FirstOrDefaultAsync(p => p.Code == plantaCode, cancellationToken);

            if (planta is null)
            {
                throw new KeyNotFoundException($"Planta {plantaCode} no encontrada.");
            }

            var code = GuiaNumberGenerator.Next(plantaCode, planta.GuiaNum);

            var puntoPartida = string.IsNullOrWhiteSpace(request.PuntoPartida)
                ? planta.Address
                : request.PuntoPartida.Trim();

            if (puntoPartida.Length > MaxPuntoPartidaLength)
            {
                puntoPartida = puntoPartida.Substring(0, MaxPuntoPartidaLength);
            }

            var guia = new Guia
            {
                PlantaCode = plantaCode,
                Code = code,
                FechaEmision = request.FechaEmision.Date,
                Hora = request.Hora.Trim(),
                ProveedorCode = Normalize(request.ProveedorCode),
                MotivoCode = Normalize(request.MotivoCode),
                Direccion = request.Direccion.Trim(),
                PuntoPartida = puntoPartida,
                TransportistaCode = Normalize(request.TransportistaCode),
                ConductorCode = Normalize(request.ConductorCode),
                VehiculoCode = Normalize(request.VehiculoCode),
                Observaciones = request.Observaciones?.Trim() ?? string.Empty,
                Estado = EstadoGuia.Pendiente
            };

            var item = 1;
            foreach (var line in request.Detalles)
            {
                guia.Detalles.Add(new GuiaDetalle
                {
                    PlantaCode = plantaCode,
                    GuiaCode = code,
                    ArticuloCode = Normalize(line.ArticuloCode),
                    ItemNumber = item++,
                    Cantidad = line.Cantidad,
                    IsVerified = line.IsVerified,
                    Estado = EstadoGuiaDetalle.Pendiente
                });
            }

            planta.GuiaNum = code;

            await _uow.Logistica.Transacciones.Guias.AddAsync(guia, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    $"Otro usuario emitió una guía en la planta {plantaCode} al mismo tiempo. Intente nuevamente.");
            }

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, code, cancellationToken);
        }

        private static string Normalize(string code) => code.Trim().ToUpperInvariant();
    }
}
