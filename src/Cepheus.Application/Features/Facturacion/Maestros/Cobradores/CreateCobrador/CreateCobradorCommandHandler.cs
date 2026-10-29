using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.CreateCobrador
{
    public class CreateCobradorCommandHandler : IRequestHandler<CreateCobradorCommand, CobradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCobradorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobradorResponse> Handle(CreateCobradorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Cobradores.Query().Select(x => x.Code), length: 4, entityLabel: "Cobradores", cancellationToken);

            var entity = new Cobrador
            {
                Code = code,
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                UserId = request.UserId,
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Cobradores.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static CobradorResponse Map(Cobrador e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            Address = e.Address,
            Phone = e.Phone,
            Email = e.Email,
            UserId = e.UserId,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
