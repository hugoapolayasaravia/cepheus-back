using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetAvalesCliente
{
    public class GetAvalesClienteQueryHandler : IRequestHandler<GetAvalesClienteQuery, List<AvalClienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAvalesClienteQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<AvalClienteResponse>> Handle(GetAvalesClienteQuery request, CancellationToken cancellationToken)
        {
            var cliente = request.ClienteCode.Trim();

            return await _uow.Facturacion.Maestros.ClientesResponsablesLetra.Query()
                .AsNoTracking()
                .Where(a => a.ClienteCode == cliente)
                .OrderBy(a => a.Sequence)
                .Select(a => new AvalClienteResponse
                {
                    ClienteCode = a.ClienteCode,
                    Sequence = a.Sequence,
                    PaternalSurname = a.PaternalSurname,
                    MaternalSurname = a.MaternalSurname,
                    FirstNames = a.FirstNames,
                    DocumentTypeCode = a.DocumentTypeCode,
                    DocumentNumber = a.DocumentNumber,
                    Address = a.Address,
                    Phone = a.Phone
                })
                .ToListAsync(cancellationToken);
        }
    }
}
