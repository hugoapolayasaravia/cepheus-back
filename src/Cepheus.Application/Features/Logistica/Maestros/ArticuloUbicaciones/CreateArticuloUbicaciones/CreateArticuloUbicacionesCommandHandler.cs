using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.CreateArticuloUbicaciones
{
    public class CreateArticuloUbicacionesCommandHandler
        : IRequestHandler<CreateArticuloUbicacionesCommand, ArticuloUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloUbicacionesResponse> Handle(
            CreateArticuloUbicacionesCommand request,
            CancellationToken cancellationToken)
        {

            var code = await SequentialCodeGenerator.NextAsync(
               _uow.Logistica.Maestros.ArticuloUbicaciones.Query().Select(c => c.Code), length: 4, entityLabel: "Ubicacion de Articulos", cancellationToken);

            var ubicacion = new ArticuloUbicacion
            {
                Code = code,
                Name = request.Name.Trim(),
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                Zone = request.Zone?.Trim(),
                Rack = request.Rack?.Trim(),
                Shelf = request.Shelf?.Trim(),
                Locker = request.Locker?.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.ArticuloUbicaciones.AddAsync(ubicacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(ubicacion);
        }

        internal static ArticuloUbicacionesResponse Map(ArticuloUbicacion ubicacion) => new()
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
            UpdatedAt = ubicacion.UpdatedAt
        };
    }
}