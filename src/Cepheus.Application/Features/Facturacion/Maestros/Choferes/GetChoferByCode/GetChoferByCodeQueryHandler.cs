using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferByCode
{
    public class GetChoferByCodeQueryHandler : IRequestHandler<GetChoferByCodeQuery, ChoferResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetChoferByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChoferResponse> Handle(GetChoferByCodeQuery request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var chofer = await _uow.Facturacion.Maestros.Choferes.Query()
                .AsNoTracking()
                .Where(c => c.TransportistaCode == transportistaCode && c.Code == code)
                .Select(c => new ChoferResponse
                {
                    TransportistaCode = c.TransportistaCode,
                    Code = c.Code,
                    FullName = c.FullName,
                    DriverLicenseNumber = c.DriverLicenseNumber,
                    Observations = c.Observations,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (chofer is null)
            {
                throw new KeyNotFoundException($"Chofer {transportistaCode}/{code} no encontrado.");
            }

            return chofer;
        }
    }
}
