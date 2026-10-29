using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.CreateTipoCliente
{
    public class CreateTipoClienteCommandHandler : IRequestHandler<CreateTipoClienteCommand, TipoClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoClienteResponse> Handle(CreateTipoClienteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.TiposCliente.Query().Select(t => t.Code), length: 1, entityLabel: "Tipos de Cliente", cancellationToken);

            var tipoCliente = new TipoCliente
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposCliente.AddAsync(tipoCliente, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoCliente);
        }

        internal static TipoClienteResponse Map(TipoCliente tipoCliente) => new()
        {
            Code = tipoCliente.Code,
            Name = tipoCliente.Name,
            IsActive = tipoCliente.IsActive,
            CreatedAt = tipoCliente.CreatedAt,
            UpdatedAt = tipoCliente.UpdatedAt,
            RowVersion = tipoCliente.RowVersion
        };
    }
}
