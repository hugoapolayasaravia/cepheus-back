using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.CreateTipoHojaCorte
{
    public class CreateTipoHojaCorteCommandHandler : IRequestHandler<CreateTipoHojaCorteCommand, TipoHojaCorteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoHojaCorteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoHojaCorteResponse> Handle(CreateTipoHojaCorteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.TiposHojaCorte.Query().Select(t => t.Code), length: 2, entityLabel: "Tipos de Hoja de Corte", cancellationToken);

            var tipoHojaCorte = new TipoHojaCorte
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposHojaCorte.AddAsync(tipoHojaCorte, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoHojaCorte);
        }

        internal static TipoHojaCorteResponse Map(TipoHojaCorte tipoHojaCorte) => new()
        {
            Code = tipoHojaCorte.Code,
            Name = tipoHojaCorte.Name,
            IsActive = tipoHojaCorte.IsActive,
            CreatedAt = tipoHojaCorte.CreatedAt,
            UpdatedAt = tipoHojaCorte.UpdatedAt,
            RowVersion = tipoHojaCorte.RowVersion
        };
    }
}
