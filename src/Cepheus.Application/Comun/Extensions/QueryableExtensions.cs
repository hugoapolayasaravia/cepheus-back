using System.Linq.Expressions;
using System.Reflection;
using Cepheus.Application.Comun.Models;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Comun.Extensions
{
    /// <summary>
    /// Utilidades genéricas de ordenamiento/paginación, sin dependencias externas
    /// (Expression Trees + reflexión en vez de System.Linq.Dynamic.Core, para evitar
    /// conflictos de versiones de ensamblados como el que tuvimos con Microsoft.OpenApi).
    /// Se usan en CUALQUIER Query Handler de CUALQUIER módulo (Users, Role, Modulo, Logística...):
    ///   query.ApplySort(request.SortBy, request.SortDesc)
    ///        .ToPagedResultAsync(request.PageNumber, request.PageSize, ct);
    /// </summary>
    public static class QueryableExtensions
    {
        private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByDescendingMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.ThenByDescending) && m.GetParameters().Length == 2);

        /// <summary>
        /// Ordena dinámicamente por uno o más nombres de propiedad recibidos como string,
        /// separados por coma (ej. "LastName,FirstName"). El mismo SortDesc aplica a todas
        /// las columnas listadas. Cualquier nombre que no coincida con una propiedad de T
        /// se ignora silenciosamente (no rompe el request por un typo del cliente).
        /// </summary>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query;
            }

            var propertyNames = sortBy
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IQueryable<T> result = query;
            var isFirst = true;

            foreach (var propertyName in propertyNames)
            {
                var property = typeof(T).GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property is null)
                {
                    continue; // nombre inválido: se ignora esa columna, no rompe el resto
                }

                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                var genericMethod = (isFirst, sortDescending) switch
                {
                    (true, false) => OrderByMethod,
                    (true, true) => OrderByDescendingMethod,
                    (false, false) => ThenByMethod,
                    (false, true) => ThenByDescendingMethod
                };

                var method = genericMethod.MakeGenericMethod(typeof(T), property.PropertyType);
                result = (IQueryable<T>)method.Invoke(null, new object[] { result, lambda })!;

                isFirst = false;
            }

            return result;
        }

        /// <summary>
        /// Aplica Count + Skip/Take y arma el PagedResult&lt;T&gt;. Centraliza la lógica
        /// de paginación para que ningún Query Handler la reimplemente a mano.
        /// </summary>
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var safePageNumber = pageNumber < 1 ? 1 : pageNumber;
            var safePageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

            var items = await query
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<T>.Create(items, totalCount, safePageNumber, safePageSize);
        }
    }



}
