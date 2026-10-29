using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.UpdateArticuloProveedor
{
    public class UpdateArticuloProveedorCommandHandler
        : IRequestHandler<UpdateArticuloProveedorCommand, ArticuloProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloProveedorResponse> Handle(UpdateArticuloProveedorCommand request, CancellationToken cancellationToken)
        {
            var relacion = await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .FirstOrDefaultAsync(x =>
                    x.PlantaCode == request.PlantaCode &&
                    x.ArticuloCode == request.ArticuloCode &&
                    x.ProveedorCode == request.ProveedorCode, cancellationToken);

            if (relacion is null)
            {
                throw new KeyNotFoundException(
                    $"No existe relación para planta {request.PlantaCode}, artículo {request.ArticuloCode}, proveedor {request.ProveedorCode}.");
            }

            relacion.IsAgreement = request.IsAgreement;
            relacion.AgreementPrice = request.AgreementPrice;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateArticuloProveedor.CreateArticuloProveedorCommandHandler.Map(relacion);
        }
    }
}
