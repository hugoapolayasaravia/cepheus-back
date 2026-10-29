using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.DeleteTrabajadorDocumento
{
    public class DeleteTrabajadorDocumentoCommandHandler : IRequestHandler<DeleteTrabajadorDocumentoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorDocumentoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorDocumentoCommand request, CancellationToken cancellationToken)
        {
            var documento = await _uow.Rrhh.Maestros.TrabajadorDocumentos.GetByIdAsync(request.Id, cancellationToken);

            if (documento is null)
            {
                throw new KeyNotFoundException($"Documento {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorDocumentos.Remove(documento);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}