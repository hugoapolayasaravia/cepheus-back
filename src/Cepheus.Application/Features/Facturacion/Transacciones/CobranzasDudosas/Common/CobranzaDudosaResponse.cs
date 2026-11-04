using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common
{
    public class CobranzaDudosaResponse
    {
        public string TypeCode { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string BancoCode { get; set; } = default!;
        public string? ObraCode { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime SystemDate { get; set; }
        public TipoContabilidad TipoContabilidad { get; set; }
        public string CurrencyCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal TotalSoles { get; set; }
        public decimal TotalDollars { get; set; }
        public string? ProvisionEntry { get; set; }
        public string? CanjeEntry { get; set; }
        public EstadoRegistroCobranzaDudosa Status { get; set; }
        public DateTime? WrittenOffAt { get; set; }
        public bool IsWrittenOff => WrittenOffAt.HasValue;
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class CobranzaDudosaMapper
    {
        public static CobranzaDudosaResponse ToResponse(CobranzaDudosa x) => new()
        {
            TypeCode = x.TypeCode,
            PlantaCode = x.PlantaCode,
            Number = x.Number,
            ClienteCode = x.ClienteCode,
            BancoCode = x.BancoCode,
            ObraCode = x.ObraCode,
            ProcessDate = x.ProcessDate,
            SystemDate = x.SystemDate,
            TipoContabilidad = x.TipoContabilidad,
            CurrencyCode = x.CurrencyCode,
            ExchangeRate = x.ExchangeRate,
            TotalSoles = x.TotalSoles,
            TotalDollars = x.TotalDollars,
            ProvisionEntry = x.ProvisionEntry,
            CanjeEntry = x.CanjeEntry,
            Status = x.Status,
            WrittenOffAt = x.WrittenOffAt,
            RowVersion = x.RowVersion
        };

        public static async Task AttachClienteNamesAsync(
            IUnitOfWork uow, IReadOnlyCollection<CobranzaDudosaResponse> items, CancellationToken cancellationToken)
        {
            var codes = items.Select(i => i.ClienteCode).Distinct().ToList();
            if (codes.Count == 0)
            {
                return;
            }

            var names = await uow.Facturacion.Maestros.Clientes.Query().AsNoTracking()
                .Where(c => codes.Contains(c.Code))
                .Select(c => new { c.Code, c.Name })
                .ToDictionaryAsync(c => c.Code, c => c.Name, cancellationToken);

            foreach (var i in items)
            {
                i.ClienteName = names.GetValueOrDefault(i.ClienteCode);
            }
        }

        public static async Task<CobranzaDudosaResponse> LoadAsync(
            IUnitOfWork uow, string typeCode, string? plantaCode, string number, string? clienteCode, string? bancoCode,
            CancellationToken cancellationToken)
        {
            var cd = await CobranzaDudosaRules.GetTrackedAsync(uow, typeCode, plantaCode, number, clienteCode, bancoCode, cancellationToken);
            var response = ToResponse(cd);
            await AttachClienteNamesAsync(uow, new[] { response }, cancellationToken);
            return response;
        }
    }
}
