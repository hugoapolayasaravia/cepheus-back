using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.CreateTrabajadorDocumento
{
    public class CreateTrabajadorDocumentoCommandHandler
        : IRequestHandler<CreateTrabajadorDocumentoCommand, TrabajadorDocumentoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorDocumentoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDocumentoResponse> Handle(CreateTrabajadorDocumentoCommand request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            if (request.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                    .Where(d => d.TrabajadorCode == trabajadorCode && d.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            var documento = new TrabajadorDocumento
            {
                TrabajadorCode = trabajadorCode,
                TipoDocumentoCode = request.TipoDocumentoCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                IsPrimary = request.IsPrimary
            };

            await _uow.Rrhh.Maestros.TrabajadorDocumentos.AddAsync(documento, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(documento);
        }

        internal static TrabajadorDocumentoResponse Map(TrabajadorDocumento documento) => new()
        {
            Id = documento.Id,
            TrabajadorCode = documento.TrabajadorCode,
            TipoDocumentoCode = documento.TipoDocumentoCode,
            DocumentNumber = documento.DocumentNumber,
            IsPrimary = documento.IsPrimary,
            CreatedAt = documento.CreatedAt,
            UpdatedAt = documento.UpdatedAt
        };
    }
}