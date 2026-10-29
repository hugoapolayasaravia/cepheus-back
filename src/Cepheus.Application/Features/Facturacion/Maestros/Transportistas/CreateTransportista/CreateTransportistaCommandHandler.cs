using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.CreateTransportista
{
    public class CreateTransportistaCommandHandler : IRequestHandler<CreateTransportistaCommand, TransportistaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTransportistaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TransportistaResponse> Handle(CreateTransportistaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Transportistas.Query().Select(t => t.Code), length: 4, entityLabel: "Transportistas (Facturación)", cancellationToken);

            var transportista = new TransportistaVenta
            {
                Code = code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                MtcInternalCode = string.IsNullOrWhiteSpace(request.MtcInternalCode) ? null : request.MtcInternalCode.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Transportistas.AddAsync(transportista, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(transportista);
        }

        internal static TransportistaResponse Map(TransportistaVenta transportista) => new()
        {
            Code = transportista.Code,
            DocumentTypeCode = transportista.DocumentTypeCode,
            DocumentNumber = transportista.DocumentNumber,
            Name = transportista.Name,
            Address = transportista.Address,
            UbigeoCode = transportista.UbigeoCode,
            Phone = transportista.Phone,
            Email = transportista.Email,
            MtcInternalCode = transportista.MtcInternalCode,
            IsActive = transportista.IsActive,
            CreatedAt = transportista.CreatedAt,
            UpdatedAt = transportista.UpdatedAt,
            RowVersion = transportista.RowVersion
        };
    }
}
