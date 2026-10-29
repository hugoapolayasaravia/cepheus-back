using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor
{
    public class CreateVendedorCommandHandler : IRequestHandler<CreateVendedorCommand, VendedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateVendedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VendedorResponse> Handle(CreateVendedorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Vendedores.Query().Select(x => x.Code), length: 4, entityLabel: "Vendedores", cancellationToken);

            var entity = new Vendedor
            {
                Code = code,
                Name = request.Name.Trim(),
                Abbreviation = string.IsNullOrWhiteSpace(request.Abbreviation) ? null : request.Abbreviation.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim(),
                UserId = request.UserId,
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Vendedores.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static VendedorResponse Map(Vendedor e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            Abbreviation = e.Abbreviation,
            Address = e.Address,
            Phone = e.Phone,
            Email = e.Email,
            Title = e.Title,
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
