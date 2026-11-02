using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.CreateClienteResponsableLetra
{
    public class CreateClienteResponsableLetraCommandHandler : IRequestHandler<CreateClienteResponsableLetraCommand, ClienteResponsableLetraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateClienteResponsableLetraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponsableLetraResponse> Handle(CreateClienteResponsableLetraCommand request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim();

            var exists = await _uow.Facturacion.Maestros.Clientes.Query().AsNoTracking()
                .AnyAsync(c => c.Code == clienteCode, cancellationToken);
            if (!exists) throw new KeyNotFoundException($"Cliente {clienteCode} no encontrado.");

            ClienteResponsableLetraResponse? result = null;

            await _uow.ExecuteInTransactionAsync(async ct =>
            {
                await _uow.AcquireAppLockAsync($"aval-cliente-{clienteCode}", 15000, ct);

                var last = await _uow.Facturacion.Maestros.ClientesResponsablesLetra.Query().AsNoTracking()
                    .Where(a => a.ClienteCode == clienteCode)
                    .OrderByDescending(a => a.Sequence)
                    .Select(a => a.Sequence)
                    .FirstOrDefaultAsync(ct);

                var next = int.TryParse(last?.Trim(), out var n) ? n + 1 : 1;
                if (next > 999) throw new InvalidOperationException("El cliente ya tiene el máximo de avales (999).");

                var aval = new ClienteResponsableLetra { ClienteCode = clienteCode, Sequence = next.ToString().PadLeft(3, '0') };
                ClienteResponsableLetraRules.Apply(aval, request);

                await _uow.Facturacion.Maestros.ClientesResponsablesLetra.AddAsync(aval, ct);
                await _uow.SaveChangesAsync(ct);

                result = ClienteResponsableLetraRules.ToResponse(aval);
                return true;
            }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);

            _uow.ClearTracking();
            return result!;
        }
    }
}
