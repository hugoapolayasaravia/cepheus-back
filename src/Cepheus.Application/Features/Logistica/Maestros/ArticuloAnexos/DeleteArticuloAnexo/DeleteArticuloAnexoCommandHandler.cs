using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.DeleteArticuloAnexo
{
    public class DeleteArticuloAnexoCommandHandler : IRequestHandler<DeleteArticuloAnexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteArticuloAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteArticuloAnexoCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var art = request.ArticuloCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ArticuloAnexos.Query()
                .FirstOrDefaultAsync(a => a.SubCentroCostoCode == scc && a.ArticuloCode == art, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No existe el enlace de anexo del subcentro {scc} y el artículo {art}.");
            }

            _uow.Logistica.Maestros.ArticuloAnexos.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
