using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.DeleteResponsableObra
{
    public class DeleteResponsableObraCommandHandler : IRequestHandler<DeleteResponsableObraCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteResponsableObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteResponsableObraCommand request, CancellationToken cancellationToken)
        {
            var responsable = await _uow.Facturacion.Catalogos.ResponsablesObra.Query()
                .FirstOrDefaultAsync(r =>
                    r.ClienteCode == request.ClienteCode &&
                    r.ObraCode == request.ObraCode &&
                    r.Code == request.Code, cancellationToken);

            if (responsable is null)
            {
                throw new KeyNotFoundException("Responsable de obra no encontrado para la combinación indicada.");
            }

            _uow.Facturacion.Catalogos.ResponsablesObra.Remove(responsable);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
