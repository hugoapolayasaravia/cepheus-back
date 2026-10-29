// Cepheus.Domain/Logistica/Enum/EstadoProveedorCotizacion.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de un proveedor dentro de una Cotización. Reemplaza a
    /// dbo.RProvCotizacion.Verifica_prv (char(1)), que mezclaba "proveedor
    /// verificado" con "proveedor seleccionado para comprar" — se separa
    /// en un ciclo de vida explícito, confirmado como selección por
    /// proveedor completo (no por línea, por ahora).
    ///
    /// Invitado -> Respondido -> Seleccionado | Descartado
    /// </summary>
    public enum EstadoProveedorCotizacion
    {
        Invitado = 0,
        Respondido = 1,
        Seleccionado = 2,
        Descartado = 3
    }
}