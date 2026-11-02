using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.GetClientesResponsablesLetra
{
    public class GetClientesResponsablesLetraQueryHandler : IRequestHandler<GetClientesResponsablesLetraQuery, List<ClienteResponsableLetraResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetClientesResponsablesLetraQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ClienteResponsableLetraResponse>> Handle(GetClientesResponsablesLetraQuery request, CancellationToken cancellationToken)
        {
            var cliente = request.ClienteCode.Trim();

            return await _uow.Facturacion.Maestros.ClientesResponsablesLetra.Query()
                .AsNoTracking()
                .Where(a => a.ClienteCode == cliente)
                .OrderBy(a => a.Sequence)
                .Select(a => new ClienteResponsableLetraResponse
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
