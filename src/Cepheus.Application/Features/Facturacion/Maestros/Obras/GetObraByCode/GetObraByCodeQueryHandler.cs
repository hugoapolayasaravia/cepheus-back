using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObraByCode
{
    public class GetObraByCodeQueryHandler : IRequestHandler<GetObraByCodeQuery, ObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetObraByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObraResponse> Handle(GetObraByCodeQuery request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var obra = await _uow.Facturacion.Maestros.Obras.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == code, cancellationToken);

            if (obra is null)
            {
                throw new KeyNotFoundException($"Obra {clienteCode}/{code} no encontrada.");
            }

            return Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra.CreateObraCommandHandler.Map(obra);
        }
    }
}
