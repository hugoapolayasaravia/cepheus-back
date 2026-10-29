using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.UpdateProveedorCondicion
{
    public class UpdateProveedorCondicionCommandHandler
       : IRequestHandler<UpdateProveedorCondicionCommand, ProveedorCondicionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCondicionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCondicionResponse> Handle(UpdateProveedorCondicionCommand request, CancellationToken cancellationToken)
        {
            var condicion = await _uow.Logistica.Maestros.ProveedorCondiciones.GetByIdAsync(request.Id, cancellationToken);

            if (condicion is null)
            {
                throw new KeyNotFoundException($"Condición {request.Id} no encontrada.");
            }

            if (request.IsPrimary && !condicion.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorCondiciones.Query()
                    .Where(c => c.ProveedorCode == condicion.ProveedorCode && c.IsPrimary && c.Id != condicion.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            condicion.FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant();
            condicion.PaymentTermDays = request.PaymentTermDays;
            condicion.MonedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            condicion.CreditLimit = request.CreditLimit;
            condicion.DiscountPercentage = request.DiscountPercentage;
            condicion.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateProveedorCondicion.CreateProveedorCondicionCommandHandler.Map(condicion);
        }
    }
}
