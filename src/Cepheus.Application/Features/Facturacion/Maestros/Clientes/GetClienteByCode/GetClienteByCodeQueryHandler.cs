using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClienteByCode
{
    public class GetClienteByCodeQueryHandler : IRequestHandler<GetClienteByCodeQuery, ClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetClienteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponse> Handle(GetClienteByCodeQuery request, CancellationToken cancellationToken)
        {
            var cliente = await _uow.Facturacion.Maestros.Clientes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (cliente is null)
            {
                throw new KeyNotFoundException($"Cliente {request.Code} no encontrado.");
            }

            return Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente.CreateClienteCommandHandler.Map(cliente);
        }
    }
}
