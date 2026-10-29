using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente
{
    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponse> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Clientes.Query().Select(c => c.Code), length: 5, entityLabel: "Clientes", cancellationToken);

            var cliente = new Cliente
            {
                Code = code,
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
                Estado = EstadoCliente.Activo
            };

            await _uow.Facturacion.Maestros.Clientes.AddAsync(cliente, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(cliente);
        }

        internal static ClienteResponse Map(Cliente cliente) => new()
        {
            Code = cliente.Code,
            PersonType = cliente.PersonType.ToString(),
            DocumentTypeCode = cliente.DocumentTypeCode,
            DocumentNumber = cliente.DocumentNumber,
            Name = cliente.Name,
            Address = cliente.Address,
            UbigeoCode = cliente.UbigeoCode,
            Phone = cliente.Phone,
            ParentClientCode = cliente.ParentClientCode,
            TipoClienteCode = cliente.TipoClienteCode,
            ClasificacionClienteCode = cliente.ClasificacionClienteCode,
            LegalRepresentativeName = cliente.LegalRepresentativeName,
            LegalRepresentativePhone = cliente.LegalRepresentativePhone,
            LegalRepresentativeDni = cliente.LegalRepresentativeDni,
            ContactName = cliente.ContactName,
            ContactPhone = cliente.ContactPhone,
            ContactEmail = cliente.ContactEmail,
            Observations = cliente.Observations,
            IsVip = cliente.IsVip,
            RequiresCashOnly = cliente.RequiresCashOnly,
            HasGlobalCreditLine = cliente.HasGlobalCreditLine,
            GlobalCreditAmount = cliente.GlobalCreditAmount,
            RequiresPurchaseOrderApproval = cliente.RequiresPurchaseOrderApproval,
            RequiresWorkOrderApproval = cliente.RequiresWorkOrderApproval,
            FormaPagoVentaCode = cliente.FormaPagoVentaCode,
            CurrencyCode = cliente.CurrencyCode,
            RequiresManagementApproval = cliente.RequiresManagementApproval,
            Estado = cliente.Estado.ToString(),
            CreatedAt = cliente.CreatedAt,
            UpdatedAt = cliente.UpdatedAt,
            CreatedBy = cliente.CreatedBy,
            UpdatedBy = cliente.UpdatedBy,
            RowVersion = cliente.RowVersion
        };
    }
}
