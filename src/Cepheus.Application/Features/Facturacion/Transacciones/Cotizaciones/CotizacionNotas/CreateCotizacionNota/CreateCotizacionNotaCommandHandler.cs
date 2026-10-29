using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.CreateCotizacionNota
{
    public class CreateCotizacionNotaCommandHandler : IRequestHandler<CreateCotizacionNotaCommand, CotizacionNotaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionNotaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionNotaResponse> Handle(CreateCotizacionNotaCommand request, CancellationToken cancellationToken)
        {
            var nota = new CotizacionNota
            {
                NegocioCode = request.NegocioCode.Trim().ToUpperInvariant(),
                Year = request.Year,
                Month = request.Month,
                Code = request.Code.Trim().ToUpperInvariant(),
                Description = request.Description.Trim(),
                Option = request.Option
            };

            await _uow.Facturacion.Transacciones.CotizacionesNotas.AddAsync(nota, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nota);
        }

        internal static CotizacionNotaResponse Map(CotizacionNota n) => new()
        {
            NegocioCode = n.NegocioCode,
            Year = n.Year,
            Month = n.Month,
            Code = n.Code,
            Sequence = n.Sequence,
            Description = n.Description,
            Option = n.Option,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt,
            RowVersion = n.RowVersion
        };
    }
}
