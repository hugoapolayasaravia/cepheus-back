using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentoById
{
    public class GetTrabajadorDocumentoByIdQueryHandler
        : IRequestHandler<GetTrabajadorDocumentoByIdQuery, TrabajadorDocumentoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorDocumentoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDocumentoResponse> Handle(GetTrabajadorDocumentoByIdQuery request, CancellationToken cancellationToken)
        {
            var documento = await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                .AsNoTracking()
                .Where(d => d.Id == request.Id)
                .Select(d => new TrabajadorDocumentoResponse
                {
                    Id = d.Id,
                    TrabajadorCode = d.TrabajadorCode,
                    TipoDocumentoCode = d.TipoDocumentoCode,
                    DocumentNumber = d.DocumentNumber,
                    IsPrimary = d.IsPrimary,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (documento is null)
            {
                throw new KeyNotFoundException($"Documento {request.Id} no encontrado.");
            }

            return documento;
        }
    }
}