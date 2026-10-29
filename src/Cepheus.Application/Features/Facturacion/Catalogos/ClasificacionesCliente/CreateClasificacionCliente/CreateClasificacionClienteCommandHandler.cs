using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.CreateClasificacionCliente
{
    public class CreateClasificacionClienteCommandHandler : IRequestHandler<CreateClasificacionClienteCommand, ClasificacionClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateClasificacionClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClasificacionClienteResponse> Handle(CreateClasificacionClienteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.ClasificacionesCliente.Query().Select(t => t.Code), length: 1, entityLabel: "Clasificaciones de Cliente", cancellationToken);

            var clasificacionCliente = new ClasificacionCliente
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.ClasificacionesCliente.AddAsync(clasificacionCliente, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(clasificacionCliente);
        }

        internal static ClasificacionClienteResponse Map(ClasificacionCliente clasificacionCliente) => new()
        {
            Code = clasificacionCliente.Code,
            Name = clasificacionCliente.Name,
            IsActive = clasificacionCliente.IsActive,
            CreatedAt = clasificacionCliente.CreatedAt,
            UpdatedAt = clasificacionCliente.UpdatedAt,
            RowVersion = clasificacionCliente.RowVersion
        };
    }
}
