using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.EliminarAsientoConsumo
{
    /// <summary>Elimina el asiento de consumo completo: todas las líneas de vale y vales de esa generación quedan libres.</summary>
    public record EliminarAsientoConsumoCommand(string AsientoCodigo) : IRequest<AsientoConsumoResultResponse>;
}
