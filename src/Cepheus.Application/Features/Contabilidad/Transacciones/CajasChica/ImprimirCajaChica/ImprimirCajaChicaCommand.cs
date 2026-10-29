using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ImprimirCajaChica
{
    /// <summary>Marca o desmarca la impresión de la rendición (legacy impresion_chi S/N).</summary>
    public record ImprimirCajaChicaCommand(string Code, bool Printed) : IRequest<CajaChicaResponse>;
}
