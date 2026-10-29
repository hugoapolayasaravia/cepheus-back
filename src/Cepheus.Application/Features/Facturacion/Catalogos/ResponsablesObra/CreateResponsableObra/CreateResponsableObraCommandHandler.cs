using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.CreateResponsableObra
{
    /// <summary>
    /// Codigo_Res es un correlativo POR OBRA (PK compuesta Cliente + Obra +
    /// Responsable). Se genera igual que Obra.Code por cliente: se busca el
    /// máximo Code existente para la obra y se reintenta ante colisión por
    /// concurrencia.
    /// </summary>
    public class CreateResponsableObraCommandHandler : IRequestHandler<CreateResponsableObraCommand, ResponsableObraResponse>
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly IUnitOfWork _uow;

        public CreateResponsableObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResponsableObraResponse> Handle(CreateResponsableObraCommand request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim().ToUpperInvariant();
            var obraCode = request.ObraCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(clienteCode, obraCode, cancellationToken);

                var responsable = new ResponsableObra
                {
                    ClienteCode = clienteCode,
                    ObraCode = obraCode,
                    Code = nextCode,
                    Name = request.Name.Trim(),
                    Phone = request.Phone.Trim(),
                    Position = request.Position.Trim()
                };

                await _uow.Facturacion.Catalogos.ResponsablesObra.AddAsync(responsable, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return Map(responsable);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    // Colisión de correlativo por creación simultánea para la
                    // misma obra: se reintenta generando el siguiente código.
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de responsable para la obra {clienteCode}-{obraCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<int> NextCodeAsync(string clienteCode, string obraCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Facturacion.Catalogos.ResponsablesObra.Query()
                .AsNoTracking()
                .Where(r => r.ClienteCode == clienteCode && r.ObraCode == obraCode)
                .OrderByDescending(r => r.Code)
                .Select(r => (int?)r.Code)
                .FirstOrDefaultAsync(cancellationToken);

            return (lastCode ?? 0) + 1;
        }

        internal static ResponsableObraResponse Map(ResponsableObra r) => new()
        {
            ClienteCode = r.ClienteCode,
            ObraCode = r.ObraCode,
            Code = r.Code,
            Name = r.Name,
            Phone = r.Phone,
            Position = r.Position,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            RowVersion = r.RowVersion
        };
    }
}
