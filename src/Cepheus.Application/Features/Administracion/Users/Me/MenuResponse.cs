namespace Cepheus.Application.Features.Administracion.Users.Me
{
    /// <summary>Módulo del menú: nivel 1 del sidebar.</summary>
    public class MenuModuloResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public int DisplayOrder { get; set; }
        public List<MenuSubmoduloResponse> Submodulos { get; set; } = new();
    }

    /// <summary>Submódulo del menú: nivel 2 del sidebar.</summary>
    public class MenuSubmoduloResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public int DisplayOrder { get; set; }
        public List<MenuProgramaResponse> Programas { get; set; } = new();
    }

    /// <summary>Programa del menú: nivel 3 (el que lleva a una pantalla).</summary>
    public class MenuProgramaResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public string? Route { get; set; }
        public int DisplayOrder { get; set; }

        /// <summary>Códigos de permiso que el usuario tiene en este programa (VIEW, CREATE...).</summary>
        public List<string> Permissions { get; set; } = new();
    }
}
