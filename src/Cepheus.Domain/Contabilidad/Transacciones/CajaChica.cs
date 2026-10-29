using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Caja Chica (cabecera): rendición de gastos pagados con fondos de caja, que se provisiona
    /// contablemente (genera documentos de compra y asiento) y luego se cancela.
    ///
    /// Legacy: dbo.MCajaChicaResumen.
    ///   Codigo_chi          -> Code (correlativo, char(6))
    ///   Codigo_pla          -> PlantaCode (FK -> Comunes.Planta)
    ///   Codigo_res          -> TrabajadorCode (FK -> Rrhh.Trabajador; responsable de la caja.
    ///                          Reemplaza a la tabla "usuario por caja")
    ///   caja_usu            -> ELIMINADO (lo reemplaza Codigo_res)
    ///   FecEmision_chi      -> IssueDate
    ///   Observaciones_chi   -> Observations
    ///   MontoSol_chi        -> MontoSol (suma de los documentos en moneda base)
    ///   TotalSoles_chi      -> TotalSol (total de todos los documentos expresado en soles)
    ///   MontoDol_chi        -> NO se guarda: con multimoneda se calcula al emitir los reportes
    ///   Asiento_chi         -> ProvisionAsientoCode (asiento de provisión, 13 caracteres)
    ///   AsientoReversion_chi -> ReversionAsientoCode (asiento de reversión/cancelación)
    ///   tipo_chi            -> Type (enum TipoCajaChica: F, G, R)
    ///   impresion_chi       -> IsPrinted (S/N -> bool)
    ///   FecProceso_chi, Codigo_usu -> ELIMINADOS: auditoría (CreatedAt / CreatedBy)
    ///   Estado_chi          -> Status (enum EstadoCajaChica: 01, 03, 04, 13)
    /// </summary>
    public class CajaChica : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public DateTime IssueDate { get; set; }
        public string? Observations { get; set; }

        public decimal MontoSol { get; set; }
        public decimal TotalSol { get; set; }

        public string? ProvisionAsientoCode { get; set; }
        public string? ReversionAsientoCode { get; set; }

        public TipoCajaChica Type { get; set; }
        public bool IsPrinted { get; set; }
        public EstadoCajaChica Status { get; set; } = EstadoCajaChica.Pendiente;

        public ICollection<CajaChicaDetalle> Details { get; set; } = new List<CajaChicaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
