using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetAvalesCliente
{
    /// <summary>Avales del cliente para elegir al imprimir la letra (legacy dw_bus_cre_avales).</summary>
    public record GetAvalesClienteQuery(string ClienteCode) : IRequest<List<AvalClienteResponse>>;

    public class AvalClienteResponse
    {
        public string ClienteCode { get; set; } = default!;
        public string Sequence { get; set; } = default!;
        public string? PaternalSurname { get; set; }
        public string? MaternalSurname { get; set; }
        public string? FirstNames { get; set; }
        public string? DocumentTypeCode { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        /// <summary>Nombres y apellidos en una sola línea, en el orden que imprime la letra (nombres, paterno, materno).</summary>
        public string FullName =>
            string.Join(" ", new[] { FirstNames, PaternalSurname, MaternalSurname }
                .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim()));
    }
}
