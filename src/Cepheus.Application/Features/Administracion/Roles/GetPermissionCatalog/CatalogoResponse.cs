namespace Cepheus.Application.Features.Administracion.Roles.GetPermissionCatalog
{
    /// <summary>Módulo del catálogo de permisos: nivel 1 del árbol.</summary>
    public class CatalogoModuloResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CatalogoSubmoduloResponse> Submodulos { get; set; } = new();
    }

    /// <summary>Submódulo del catálogo: nivel 2.</summary>
    public class CatalogoSubmoduloResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CatalogoProgramaResponse> Programas { get; set; } = new();
    }

    /// <summary>Programa del catálogo: nivel 3. Sus permisos son los que se marcan en un rol.</summary>
    public class CatalogoProgramaResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CatalogoPermisoResponse> Permisos { get; set; } = new();
    }

    /// <summary>Permiso: nivel 4 (VIEW, CREATE, UPDATE, DELETE, EXPORT_EXCEL, EXPORT_PDF...).</summary>
    public class CatalogoPermisoResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
