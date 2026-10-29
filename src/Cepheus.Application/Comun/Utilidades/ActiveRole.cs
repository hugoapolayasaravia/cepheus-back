namespace Cepheus.Application.Comun.Utilidades
{
    /// <summary>
    /// Un usuario puede tener varios roles, pero trabaja con UNO a la vez (el rol activo).
    /// Los permisos NUNCA se suman entre roles. El cliente indica el rol activo con el
    /// encabezado <see cref="HeaderName"/>; si falta o no es un rol suyo (activo), se usa el
    /// primero por nombre, de forma determinista.
    /// </summary>
    public static class ActiveRole
    {
        public const string HeaderName = "X-Cepheus-Role";

        /// <param name="roleIdsOrderedByName">Ids de los roles ACTIVOS del usuario, ordenados por nombre.</param>
        public static int? Pick(IReadOnlyList<int> roleIdsOrderedByName, int? requested)
        {
            if (roleIdsOrderedByName.Count == 0) return null;
            if (requested.HasValue && roleIdsOrderedByName.Contains(requested.Value)) return requested.Value;
            return roleIdsOrderedByName[0];
        }
    }
}
