using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.GetVendedorByCode
{
    public class GetVendedorByCodeQueryHandler : IRequestHandler<GetVendedorByCodeQuery, VendedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetVendedorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VendedorResponse> Handle(GetVendedorByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Maestros.Vendedores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Vendedor {request.Code} no encontrado.");
            }

            return CreateVendedorCommandHandler.Map(entity);
        }
    }
}
