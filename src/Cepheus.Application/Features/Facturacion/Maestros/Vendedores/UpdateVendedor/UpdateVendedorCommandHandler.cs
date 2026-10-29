using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.UpdateVendedor
{
    public class UpdateVendedorCommandHandler : IRequestHandler<UpdateVendedorCommand, VendedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateVendedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VendedorResponse> Handle(UpdateVendedorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Maestros.Vendedores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Vendedor {request.Code} no encontrado.");
            }

            var entity = new Vendedor
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Abbreviation = string.IsNullOrWhiteSpace(request.Abbreviation) ? null : request.Abbreviation.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim(),
                UserId = request.UserId,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Vendedores.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateVendedor.CreateVendedorCommandHandler.Map(entity);
        }
    }
}
