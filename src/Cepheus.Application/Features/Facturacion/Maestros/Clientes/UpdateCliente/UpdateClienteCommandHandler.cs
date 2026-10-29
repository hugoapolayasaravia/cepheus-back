using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.UpdateCliente
{
    public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, ClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponse> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Maestros.Clientes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cliente {request.Code} no encontrado.");
            }

            var cliente = new Cliente
            {
                Code = request.Code,
                PersonType = request.PersonType,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
                Address = request.Address.Trim(),
                UbigeoCode = request.UbigeoCode.Trim(),
                Phone = request.Phone.Trim(),
                ParentClientCode = string.IsNullOrWhiteSpace(request.ParentClientCode) ? null : request.ParentClientCode.Trim().ToUpperInvariant(),
                TipoClienteCode = request.TipoClienteCode.Trim().ToUpperInvariant(),
                ClasificacionClienteCode = request.ClasificacionClienteCode.Trim().ToUpperInvariant(),
                LegalRepresentativeName = string.IsNullOrWhiteSpace(request.LegalRepresentativeName) ? null : request.LegalRepresentativeName.Trim(),
                LegalRepresentativePhone = string.IsNullOrWhiteSpace(request.LegalRepresentativePhone) ? null : request.LegalRepresentativePhone.Trim(),
                LegalRepresentativeDni = string.IsNullOrWhiteSpace(request.LegalRepresentativeDni) ? null : request.LegalRepresentativeDni.Trim(),
                ContactName = request.ContactName.Trim(),
                ContactPhone = request.ContactPhone.Trim(),
                ContactEmail = request.ContactEmail.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsVip = request.IsVip,
                RequiresCashOnly = request.RequiresCashOnly,
                HasGlobalCreditLine = request.HasGlobalCreditLine,
                GlobalCreditAmount = request.GlobalCreditAmount,
                RequiresPurchaseOrderApproval = request.RequiresPurchaseOrderApproval,
                RequiresWorkOrderApproval = request.RequiresWorkOrderApproval,
                FormaPagoVentaCode = request.FormaPagoVentaCode.Trim().ToUpperInvariant(),
                CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode) ? null : request.CurrencyCode.Trim().ToUpperInvariant(),
                RequiresManagementApproval = request.RequiresManagementApproval,

                Estado = current.Estado,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Clientes.Update(cliente);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El cliente fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente.CreateClienteCommandHandler.Map(cliente);
        }
    }
}
