using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.ToggleProductoStatus
{
    public class ToggleProductoStatusCommandHandler : IRequestHandler<ToggleProductoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProductoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProductoStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var producto = await _uow.Facturacion.Maestros.Productos.Query()
                .FirstOrDefaultAsync(p => p.TipoProductoCode == tipoProductoCode && p.Code == code, cancellationToken);

            if (producto is null)
            {
                throw new KeyNotFoundException($"Producto {tipoProductoCode}{code} no encontrado.");
            }

            producto.IsActive = !producto.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return producto.IsActive;
        }
    }
}
