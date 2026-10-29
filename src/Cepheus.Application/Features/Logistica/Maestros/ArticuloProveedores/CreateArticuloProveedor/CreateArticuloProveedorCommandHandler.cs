using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.CreateArticuloProveedor
{
    public class CreateArticuloProveedorCommandHandler
         : IRequestHandler<CreateArticuloProveedorCommand, ArticuloProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloProveedorResponse> Handle(CreateArticuloProveedorCommand request, CancellationToken cancellationToken)
        {
            var relacion = new ArticuloProveedor
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                ArticuloCode = request.ArticuloCode.Trim().ToUpperInvariant(),
                ProveedorCode = request.ProveedorCode.Trim().ToUpperInvariant(),
                IsAgreement = request.IsAgreement,
                AgreementPrice = request.AgreementPrice
            };

            await _uow.Logistica.Maestros.ArticuloProveedores.AddAsync(relacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(relacion);
        }

        internal static ArticuloProveedorResponse Map(ArticuloProveedor relacion) => new()
        {
            PlantaCode = relacion.PlantaCode,
            ArticuloCode = relacion.ArticuloCode,
            ProveedorCode = relacion.ProveedorCode,
            IsAgreement = relacion.IsAgreement,
            AgreementPrice = relacion.AgreementPrice,
            CreatedAt = relacion.CreatedAt,
            UpdatedAt = relacion.UpdatedAt
        };
    }
}
