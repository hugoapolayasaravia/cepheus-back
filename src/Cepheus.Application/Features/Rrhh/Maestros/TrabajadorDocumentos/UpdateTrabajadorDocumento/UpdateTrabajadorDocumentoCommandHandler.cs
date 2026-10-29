using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.UpdateTrabajadorDocumento
{
    public class UpdateTrabajadorDocumentoCommandHandler
        : IRequestHandler<UpdateTrabajadorDocumentoCommand, TrabajadorDocumentoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorDocumentoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDocumentoResponse> Handle(UpdateTrabajadorDocumentoCommand request, CancellationToken cancellationToken)
        {
            var documento = await _uow.Rrhh.Maestros.TrabajadorDocumentos.GetByIdAsync(request.Id, cancellationToken);

            if (documento is null)
            {
                throw new KeyNotFoundException($"Documento {request.Id} no encontrado.");
            }

            if (request.IsPrimary && !documento.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                    .Where(d => d.TrabajadorCode == documento.TrabajadorCode && d.IsPrimary && d.Id != documento.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            documento.TipoDocumentoCode = request.TipoDocumentoCode.Trim().ToUpperInvariant();
            documento.DocumentNumber = request.DocumentNumber.Trim();
            documento.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateTrabajadorDocumento.CreateTrabajadorDocumentoCommandHandler.Map(documento);
        }
    }
}