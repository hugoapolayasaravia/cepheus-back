namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado de un período contable (año-mes).
    ///   Abierto: admite generar, modificar y anular asientos.
    ///   Cerrado: no admite movimientos; para corregir hay que reabrirlo.
    /// Reemplaza a la variable global del legacy (gs_cierre: último año-mes cerrado).
    /// </summary>
    public enum EstadoPeriodoContable
    {
        Abierto = 1,
        Cerrado = 2
    }
}
