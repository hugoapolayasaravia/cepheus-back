using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.CreateProveedorCondicion
{
    public class CreateProveedorCondicionCommandHandler
        : IRequestHandler<CreateProveedorCondicionCommand, ProveedorCondicionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorCondicionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCondicionResponse> Handle(CreateProveedorCondicionCommand request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            if (request.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorCondiciones.Query()
                    .Where(c => c.ProveedorCode == proveedorCode && c.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            var condicion = new ProveedorCondicion
            {
                ProveedorCode = proveedorCode,
                FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant(),
                PaymentTermDays = request.PaymentTermDays,
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                CreditLimit = request.CreditLimit,
                DiscountPercentage = request.DiscountPercentage,
                IsPrimary = request.IsPrimary,
                IsActive = true
            };

            await _uow.Logistica.Maestros.ProveedorCondiciones.AddAsync(condicion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(condicion);
        }

        internal static ProveedorCondicionResponse Map(ProveedorCondicion condicion) => new()
        {
            Id = condicion.Id,
            ProveedorCode = condicion.ProveedorCode,
            FormaPagoCode = condicion.FormaPagoCode,
            PaymentTermDays = condicion.PaymentTermDays,
            MonedaCode = condicion.MonedaCode,
            CreditLimit = condicion.CreditLimit,
            DiscountPercentage = condicion.DiscountPercentage,
            IsPrimary = condicion.IsPrimary,
            IsActive = condicion.IsActive,
            CreatedAt = condicion.CreatedAt,
            UpdatedAt = condicion.UpdatedAt
        };
    }
}
