using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedor
{
    public class GetArticuloProveedorQueryHandler : IRequestHandler<GetArticuloProveedorQuery, ArticuloProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloProveedorResponse> Handle(GetArticuloProveedorQuery request, CancellationToken cancellationToken)
        {
            var relacion = await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .AsNoTracking()
                .Where(x =>
                    x.PlantaCode == request.PlantaCode &&
                    x.ArticuloCode == request.ArticuloCode &&
                    x.ProveedorCode == request.ProveedorCode)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (relacion is null)
            {
                throw new KeyNotFoundException(
                    $"No existe relación para planta {request.PlantaCode}, artículo {request.ArticuloCode}, proveedor {request.ProveedorCode}.");
            }

            return relacion;
        }
    }
}
