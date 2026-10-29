using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentosByTrabajador
{
    public class GetTrabajadorDocumentosByTrabajadorQueryHandler
        : IRequestHandler<GetTrabajadorDocumentosByTrabajadorQuery, List<TrabajadorDocumentoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorDocumentosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorDocumentoResponse>> Handle(
            GetTrabajadorDocumentosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            return await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                .AsNoTracking()
                .Where(d => d.TrabajadorCode == trabajadorCode)
                .OrderByDescending(d => d.IsPrimary)
                .ThenBy(d => d.Id)
                .Select(d => new TrabajadorDocumentoResponse
                {
                    Id = d.Id,
                    TrabajadorCode = d.TrabajadorCode,
                    TipoDocumentoCode = d.TipoDocumentoCode,
                    DocumentNumber = d.DocumentNumber,
                    IsPrimary = d.IsPrimary,
                    CreatedAt = d.CreatedAt,
                    CreatedBy = d.CreatedBy,
                    UpdatedBy = d.UpdatedBy,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}