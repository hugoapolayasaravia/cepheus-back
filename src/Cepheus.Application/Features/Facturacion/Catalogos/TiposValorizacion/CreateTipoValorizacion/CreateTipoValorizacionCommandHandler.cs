using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.CreateTipoValorizacion
{
    public class CreateTipoValorizacionCommandHandler : IRequestHandler<CreateTipoValorizacionCommand, TipoValorizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoValorizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValorizacionResponse> Handle(CreateTipoValorizacionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.TiposValorizacion.Query().Select(t => t.Code), length: 1, entityLabel: "Tipos de Valorización", cancellationToken);

            var tipoValorizacion = new TipoValorizacion
            {
                Code = code,
                Name = request.Name.Trim(),
                Days = request.Days,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposValorizacion.AddAsync(tipoValorizacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoValorizacion);
        }

        internal static TipoValorizacionResponse Map(TipoValorizacion tipoValorizacion) => new()
        {
            Code = tipoValorizacion.Code,
            Name = tipoValorizacion.Name,
            Days = tipoValorizacion.Days,
            IsActive = tipoValorizacion.IsActive,
            CreatedAt = tipoValorizacion.CreatedAt,
            UpdatedAt = tipoValorizacion.UpdatedAt,
            RowVersion = tipoValorizacion.RowVersion
        };
    }
}
