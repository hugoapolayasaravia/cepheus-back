using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.DeleteArticuloUbicaciones
{
    public class DeleteArticuloUbicacionesCommandHandler : IRequestHandler<DeleteArticuloUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteArticuloUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteArticuloUbicacionesCommand request, CancellationToken cancellationToken)
        {
            var codeClean = request.Code.Trim();

            var ubicacion = await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .FirstOrDefaultAsync(x => x.Code == codeClean, cancellationToken);

            if (ubicacion is null)
            {
                throw new KeyNotFoundException($"No existe la ubicación con código '{request.Code}'.");
            }

            _uow.Logistica.Maestros.ArticuloUbicaciones.Remove(ubicacion);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}