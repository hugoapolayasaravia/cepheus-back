using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByArticulo
{
    public class GetArticuloProveedoresByArticuloQueryHandler
        : IRequestHandler<GetArticuloProveedoresByArticuloQuery, List<ArticuloProveedorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloProveedoresByArticuloQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArticuloProveedorResponse>> Handle(
            GetArticuloProveedoresByArticuloQuery request, CancellationToken cancellationToken)
        {
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .AsNoTracking()
                .Where(x => x.ArticuloCode == articuloCode)
                .Select(x => new ArticuloProveedorResponse
                {
                    PlantaCode = x.PlantaCode,
                    ArticuloCode = x.ArticuloCode,
                    ProveedorCode = x.ProveedorCode,
                    IsAgreement = x.IsAgreement,
                    AgreementPrice = x.AgreementPrice,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
