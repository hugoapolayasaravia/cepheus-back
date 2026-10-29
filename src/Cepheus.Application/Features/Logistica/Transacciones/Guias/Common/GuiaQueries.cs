// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaQueries.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    internal static class GuiaQueries
    {
        /// <summary>Incluye los maestros de la cabecera (para listados).</summary>
        public static IQueryable<Guia> WithHeaderIncludes(this IQueryable<Guia> query)
            => query
                .Include(g => g.Planta)
                .Include(g => g.Proveedor)
                .Include(g => g.Motivo)
                .Include(g => g.Transportista)
                .Include(g => g.Conductor)
                .Include(g => g.Vehiculo);

        /// <summary>Cabecera + detalle ordenado por ItemNumber, con el artículo de cada línea.</summary>
        public static IQueryable<Guia> WithFullIncludes(this IQueryable<Guia> query)
            => query
                .WithHeaderIncludes()
                .Include(g => g.Detalles.OrderBy(d => d.ItemNumber))
                    .ThenInclude(d => d.Articulo);
    }
}
