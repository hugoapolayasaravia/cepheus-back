namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de vehículo de transporte.
    ///
    /// Legacy: dbo.TVehiculos.Tipo_veh (char(1), parte de la PK). Catálogo cerrado y
    /// corto (Camión, Bomba Mixer, Carreta, Otros) -> enum, mismo criterio que
    /// Turno/EstadoOrdenTrabajo en Mantenimiento. Si el negocio necesita altas de
    /// tipos sin desplegar, el paso natural es migrarlo a tabla.
    ///
    /// Los valores parten en 1 (no en 0) porque forman parte de la PK de Vehiculo:
    /// así un valor sin asignar nunca se confunde con un tipo real.
    ///
    /// PENDIENTE: confirmar qué carácter usaba el legacy para cada tipo
    /// (SELECT DISTINCT Tipo_veh FROM dbo.TVehiculos) para armar el mapeo del ETL.
    /// </summary>
    public enum TipoVehiculo
    {
        Camion = 1,
        BombaMixer = 2,
        Carreta = 3,
        Otros = 4
    }
}
