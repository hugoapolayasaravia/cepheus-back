// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/CreateGuiaDetalle/CreateGuiaDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.CreateGuiaDetalle
{
    public class CreateGuiaDetalleCommandHandler : IRequestHandler<CreateGuiaDetalleCommand, GuiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateGuiaDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(CreateGuiaDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var guiaCode = request.GuiaCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var guia = await _uow.Logistica.Transacciones.Guias.Query()
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.PlantaCode == plantaCode && g.Code == guiaCode, cancellationToken);

            if (guia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{guiaCode} no encontrada.");
            }

            if (guia.Estado != EstadoGuia.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Guía está en estado '{guia.Estado}' y ya no admite agregar líneas.");
            }

            // Legacy: un artículo no puede repetirse dentro de la misma guía.
            if (guia.Detalles.Any(d => d.ArticuloCode == articuloCode))
            {
                throw new InvalidOperationException(
                    $"El artículo {articuloCode} ya está registrado en la guía {guiaCode}.");
            }

            var nextItem = guia.Detalles.Count == 0 ? 1 : guia.Detalles.Max(d => d.ItemNumber) + 1;

            var linea = new GuiaDetalle
            {
                PlantaCode = plantaCode,
                GuiaCode = guiaCode,
                ArticuloCode = articuloCode,
                ItemNumber = nextItem,
                Cantidad = request.Cantidad,
                IsVerified = request.IsVerified,
                Estado = EstadoGuiaDetalle.Pendiente
            };

            guia.Detalles.Add(linea);
            await _uow.Logistica.Transacciones.GuiaDetalles.AddAsync(linea, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, guiaCode, cancellationToken);
        }
    }
}
