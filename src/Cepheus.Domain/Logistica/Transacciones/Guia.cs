// Cepheus.Domain/Logistica/Transacciones/Guia.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Guía de Remisión (cabecera) emitida por una planta hacia un proveedor.
    /// NO mueve stock (confirmado por el usuario).
    ///
    /// Legacy: dbo.MGuias (SQL Server), PK compuesta (Codigo_pla, Numero_gve).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla      -> PlantaCode (FK -> Comunes.Planta.Code)
    ///   Numero_gve      -> Code (char(10), formato 'SSS-NNNNNN': serie de 3
    ///                      dígitos + guion + correlativo de 6 dígitos)
    ///   FecEmision_gve  -> FechaEmision (ingreso manual)
    ///   Hora_gve        -> Hora (formato 'HH:mm', ingreso manual)
    ///   Codigo_prv      -> ProveedorCode (FK -> Logistica.Maestros.Proveedor;
    ///                      el destinatario es un proveedor, no un cliente)
    ///   Codigo_mot      -> MotivoCode (FK -> Comunes.MotivoDevolucion.Code,
    ///                      el catálogo de motivos existente; no se crea un
    ///                      catálogo nuevo por indicación del usuario)
    ///   Direccion_gve   -> Direccion (dirección del proveedor + referencia)
    ///   Codigo_tra      -> TransportistaCode (FK -> Transportista)
    ///   Codigo_cho      -> ConductorCode (FK -> Conductor)
    ///   Codigo_veh      -> VehiculoCode (FK -> Vehiculo)
    ///   Observacion_gve -> Observaciones
    ///   Codigo_Est      -> Estado (enum EstadoGuia: 01 Pendiente, 04 Anulado)
    ///   Codigo_Usu      -> eliminado, lo cubre CreatedBy (IAuditableEntity)
    ///   FecProceso_gve  -> eliminado, lo cubre CreatedAt (IAuditableEntity)
    ///   Punto_par       -> PuntoPartida (por defecto la dirección de la planta)
    ///   Numero_Cer      -> eliminado de la guía: el DDL lo dejó comentado con
    ///                      la nota "agregarlo al transportista"; ya existe
    ///                      Transportista.CertificationNumber.
    ///
    /// Numeración: el correlativo lo genera el backend a partir de
    /// Planta.GuiaNum (último número emitido por la planta) y se actualiza en
    /// la MISMA transacción que inserta la guía.
    ///
    /// Columnas que el PowerBuilder usaba y ya no existen en la tabla
    /// (Nombre_Cli, Nombre_tra, Nombre_cho, Tipo_veh, etc.) se obtienen por
    /// join a los maestros, no se duplican.
    /// </summary>
    public class Guia : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Número de guía 'SSS-NNNNNN' (Numero_gve, char(10)).</summary>
        public string Code { get; set; } = default!;

        public DateTime FechaEmision { get; set; }

        /// <summary>Hora de emisión, formato 'HH:mm'.</summary>
        public string Hora { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string MotivoCode { get; set; } = default!;
        public MotivoDevolucion Motivo { get; set; } = default!;

        public string Direccion { get; set; } = default!;
        public string PuntoPartida { get; set; } = default!;

        public string TransportistaCode { get; set; } = default!;
        public Transportista Transportista { get; set; } = default!;

        public string ConductorCode { get; set; } = default!;
        public Conductor Conductor { get; set; } = default!;

        public string VehiculoCode { get; set; } = default!;
        public Vehiculo Vehiculo { get; set; } = default!;

        public string Observaciones { get; set; } = string.Empty;

        public EstadoGuia Estado { get; set; } = EstadoGuia.Pendiente;

        public ICollection<GuiaDetalle> Detalles { get; set; } = new List<GuiaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
