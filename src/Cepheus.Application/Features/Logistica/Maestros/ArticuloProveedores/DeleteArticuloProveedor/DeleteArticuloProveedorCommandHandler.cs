using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.DeleteArticuloProveedor
{
    public class DeleteArticuloProveedorCommandHandler : IRequestHandler<DeleteArticuloProveedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteArticuloProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteArticuloProveedorCommand request, CancellationToken cancellationToken)
        {
            var relacion = await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .FirstOrDefaultAsync(x =>
                    x.PlantaCode == request.PlantaCode &&
                    x.ArticuloCode == request.ArticuloCode &&
                    x.ProveedorCode == request.ProveedorCode, cancellationToken);

            if (relacion is null)
            {
                throw new KeyNotFoundException(
                    $"No existe relación para planta {request.PlantaCode}, artículo {request.ArticuloCode}, proveedor {request.ProveedorCode}.");
            }

            _uow.Logistica.Maestros.ArticuloProveedores.Remove(relacion);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
