namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado del proyecto/obra asociado a la cotización.
    ///
    /// Legacy: dbo.CotizacionResumen.Estado_Pry (char(2)). Reemplaza al
    /// subconjunto de dbo.TEstados documentado en el comentario en línea del
    /// DDL, mismo criterio que EstadoObra/EstadoCliente: catálogo cerrado y
    /// corto para esta columna puntual, no toda la tabla TEstados.
    ///
    /// Mapeo de valores legacy (Estado_Pry char(2)):
    ///   74 -> LargoPlazo
    ///   46 -> Licitacion
    ///   73 -> MedianoPlazo
    ///   72 -> PorIniciar
    ///   47 -> EnAnteproyecto
    ///   31 -> EnEjecucion
    ///   05 -> Activo
    ///   75 -> StandBy
    /// </summary>
    public enum EstadoProyectoCotizacion
    {
        Activo = 5,
        EnEjecucion = 31,
        Licitacion = 46,
        EnAnteproyecto = 47,
        PorIniciar = 72,
        MedianoPlazo = 73,
        LargoPlazo = 74,
        StandBy = 75
    }
}
