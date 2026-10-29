using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra
{
    /// <summary>
    /// Codigo_obr es un correlativo de 3 dígitos POR CLIENTE (confirmado por el
    /// usuario), no un correlativo global. Se genera igual que
    /// OrdenTrabajo.Code en Mantenimiento (correlativo por PlantaCode): se
    /// busca el máximo Code existente para el ClienteCode dado y se reintenta
    /// ante colisión por concurrencia.
    /// </summary>
    public class CreateObraCommandHandler : IRequestHandler<CreateObraCommand, ObraResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CodeLength = 3;

        private readonly IUnitOfWork _uow;

        public CreateObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObraResponse> Handle(CreateObraCommand request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(clienteCode, cancellationToken);

                var obra = new Obra
                {
                    ClienteCode = clienteCode,
                    Code = nextCode,
                    Description = request.Description.Trim(),
                    Address = request.Address.Trim(),
                    UbigeoCode = request.UbigeoCode.Trim(),
                    Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                    Estado = EstadoObra.Activo,
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
                    RequiresManagementApproval = request.RequiresManagementApproval
                };

                await _uow.Facturacion.Maestros.Obras.AddAsync(obra, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return Map(obra);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    // Colisión de correlativo por creación simultánea para el mismo
                    // cliente: se reintenta generando el siguiente código.
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Obra para el cliente {clienteCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string clienteCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Facturacion.Maestros.Obras.Query()
                .Where(o => o.ClienteCode == clienteCode)
                .OrderByDescending(o => o.Code)
                .Select(o => o.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }

        internal static ObraResponse Map(Obra obra) => new()
        {
            ClienteCode = obra.ClienteCode,
            Code = obra.Code,
            Description = obra.Description,
            Address = obra.Address,
            UbigeoCode = obra.UbigeoCode,
            Observations = obra.Observations,
            Estado = obra.Estado.ToString(),
            DeliveryAddress = obra.DeliveryAddress,
            DeliveryUbigeoCode = obra.DeliveryUbigeoCode,
            BillingAddress = obra.BillingAddress,
            BillingUbigeoCode = obra.BillingUbigeoCode,
            ResponsibleName = obra.ResponsibleName,
            ResponsiblePhone = obra.ResponsiblePhone,
            ResponsibleEmail = obra.ResponsibleEmail,
            FormaPagoVentaCode = obra.FormaPagoVentaCode,
            CobradorCode = obra.CobradorCode,
            VendedorCode = obra.VendedorCode,
            AnalisisVentaCode = obra.AnalisisVentaCode,
            CreditLimit = obra.CreditLimit,
            CreditCurrencyCode = obra.CreditCurrencyCode,
            EntryDate = obra.EntryDate,
            HasSurcharge = obra.HasSurcharge,
            ShortName = obra.ShortName,
            TipoValorizacionCode = obra.TipoValorizacionCode,
            RequiresValorizacion = obra.RequiresValorizacion,
            ScheduledWeekday = obra.ScheduledWeekday?.ToString(),
            IsProject = obra.IsProject,
            RequiresPrinting = obra.RequiresPrinting,
            CompletionDate = obra.CompletionDate,
            CompletionUser = obra.CompletionUser,
            RequiresManagementApproval = obra.RequiresManagementApproval,
            CreatedAt = obra.CreatedAt,
            UpdatedAt = obra.UpdatedAt,
            CreatedBy = obra.CreatedBy,
            UpdatedBy = obra.UpdatedBy,
            RowVersion = obra.RowVersion
        };
    }
}
