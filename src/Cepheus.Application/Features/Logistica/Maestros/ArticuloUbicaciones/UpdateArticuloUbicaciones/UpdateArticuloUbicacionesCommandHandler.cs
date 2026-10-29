using System.Collections.Generic;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.UpdateArticuloUbicaciones
{
    public class UpdateArticuloUbicacionesCommandHandler
        : IRequestHandler<UpdateArticuloUbicacionesCommand, ArticuloUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloUbicacionesResponse> Handle(
            UpdateArticuloUbicacionesCommand request,
            CancellationToken cancellationToken)
        {
            var codeClean = request.Code.Trim();

            var ubicacion = await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .FirstOrDefaultAsync(x => x.Code == codeClean, cancellationToken);

            if (ubicacion is null)
            {
                throw new KeyNotFoundException($"No existe la ubicación con código '{request.Code}'.");
            }

            // Actualización de campos
            ubicacion.Name = request.Name.Trim();
            ubicacion.PlantaCode = request.PlantaCode.Trim();
            ubicacion.Zone = request.Zone?.Trim();
            ubicacion.Rack = request.Rack?.Trim();
            ubicacion.Shelf = request.Shelf?.Trim();
            ubicacion.Locker = request.Locker?.Trim();
            ubicacion.IsActive = request.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return new ArticuloUbicacionesResponse
            {
                Code = ubicacion.Code,
                Name = ubicacion.Name,
                PlantaCode = ubicacion.PlantaCode,
                Zone = ubicacion.Zone,
                Rack = ubicacion.Rack,
                Shelf = ubicacion.Shelf,
                Locker = ubicacion.Locker,
                IsActive = ubicacion.IsActive,
                CreatedAt = ubicacion.CreatedAt,
                CreatedBy = ubicacion.CreatedBy,
                UpdatedAt = ubicacion.UpdatedAt,
                UpdatedBy = ubicacion.UpdatedBy
            };
        }
    }
}