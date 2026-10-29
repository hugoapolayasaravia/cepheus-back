using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.UpdateObra
{
    public class UpdateObraCommandHandler : IRequestHandler<UpdateObraCommand, ObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObraResponse> Handle(UpdateObraCommand request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Maestros.Obras.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Obra {clienteCode}/{code} no encontrada.");
            }

            var obra = new Obra
            {
                ClienteCode = clienteCode,
                Code = code,
                Description = request.Description.Trim(),
                Address = request.Address.Trim(),
                UbigeoCode = request.UbigeoCode.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                DeliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress) ? null : request.DeliveryAddress.Trim(),
                DeliveryUbigeoCode = request.DeliveryUbigeoCode.Trim(),
                BillingAddress = request.BillingAddress.Trim(),
                BillingUbigeoCode = request.BillingUbigeoCode.Trim(),
                ResponsibleName = string.IsNullOrWhiteSpace(request.ResponsibleName) ? null : request.ResponsibleName.Trim(),
                ResponsiblePhone = request.ResponsiblePhone.Trim(),
                ResponsibleEmail = request.ResponsibleEmail.Trim(),
                FormaPagoVentaCode = string.IsNullOrWhiteSpace(request.FormaPagoVentaCode) ? null : request.FormaPagoVentaCode.Trim().ToUpperInvariant(),
                CobradorCode = request.CobradorCode.Trim().ToUpperInvariant(),
                VendedorCode = request.VendedorCode.Trim().ToUpperInvariant(),
                AnalisisVentaCode = string.IsNullOrWhiteSpace(request.AnalisisVentaCode) ? null : request.AnalisisVentaCode.Trim().ToUpperInvariant(),
                CreditLimit = request.CreditLimit,
                CreditCurrencyCode = request.CreditCurrencyCode.Trim().ToUpperInvariant(),
                EntryDate = request.EntryDate,
                HasSurcharge = request.HasSurcharge,
                ShortName = request.ShortName.Trim(),
                TipoValorizacionCode = string.IsNullOrWhiteSpace(request.TipoValorizacionCode) ? null : request.TipoValorizacionCode.Trim().ToUpperInvariant(),
                RequiresValorizacion = request.RequiresValorizacion,
                ScheduledWeekday = string.IsNullOrWhiteSpace(request.ScheduledWeekday)
                    ? null
                    : System.Enum.Parse<DiaSemana>(request.ScheduledWeekday, ignoreCase: true),
                IsProject = request.IsProject,
                RequiresPrinting = request.RequiresPrinting,
                RequiresManagementApproval = request.RequiresManagementApproval,

                Estado = current.Estado,
                CompletionDate = current.CompletionDate,
                CompletionUser = current.CompletionUser,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Obras.Update(obra);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La obra fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra.CreateObraCommandHandler.Map(obra);
        }
    }
}
