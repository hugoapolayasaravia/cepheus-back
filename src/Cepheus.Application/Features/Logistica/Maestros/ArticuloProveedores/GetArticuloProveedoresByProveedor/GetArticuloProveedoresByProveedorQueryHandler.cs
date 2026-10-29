using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByProveedor
{
    public class GetArticuloProveedoresByProveedorQueryHandler
        : IRequestHandler<GetArticuloProveedoresByProveedorQuery, List<ArticuloProveedorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloProveedoresByProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArticuloProveedorResponse>> Handle(
            GetArticuloProveedoresByProveedorQuery request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .AsNoTracking()
                .Where(x => x.ProveedorCode == proveedorCode)
                .Select(x => new ArticuloProveedorResponse
                {
                    PlantaCode = x.PlantaCode,
                    ArticuloCode = x.ArticuloCode,
                    ProveedorCode = x.ProveedorCode,
                    IsAgreement = x.IsAgreement,
                    AgreementPrice = x.AgreementPrice,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
