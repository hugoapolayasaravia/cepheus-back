using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Usuario de gestión de Solicitudes de Nota de Crédito de Ventas: define qué
    /// usuario interviene en qué etapa del flujo, para qué negocio, y con qué
    /// facultades (ingresar, gestionar, aprobar).
    ///
    /// Legacy: dbo.MAE_USER_GESTION_SNC (SQL Server).
    /// PK legacy (codigo_neg, COD_ETAPA, COD_USER_GESTIONA) -> PK compuesta
    /// (NegocioCode, Stage, UserId).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_neg           -> NegocioCode (FK -> Comunes.Negocio)
    ///   COD_USER_GESTIONA    -> UserId (FK -> Administracion.User; en el legacy
    ///                           era el login varchar(20), ahora el Id del usuario)
    ///   COD_ETAPA            -> Stage (enum EtapaSolicitudNotaCredito, 00..05)
    ///   IND_ESTADO           -> IsActive (A/I -> bool)
    ///   FEC_INI / FEC_FIN    -> StartDate / EndDate (vigencia, opcionales)
    ///   FEC_PROCESO          -> ELIMINADO: variable de auditoría, se cubre con
    ///                           CreatedAt/UpdatedAt (IAuditableEntity)
    ///   IND_PRINCIPAL        -> IsPrincipal (S/N -> bool)
    ///   IND_USER_INGRESA     -> CanEnter    (1/0 -> bool)
    ///   IND_USER_GESTIONA    -> CanManage   (1/0 -> bool)
    ///   ind_usuario_aprueba  -> CanApprove  (1/0 -> bool)
    /// </summary>
    public class UsuarioSolicitudNotaCredito : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public Negocio Negocio { get; set; } = default!;

        public EtapaSolicitudNotaCredito Stage { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public bool IsPrincipal { get; set; }
        public bool CanEnter { get; set; }
        public bool CanManage { get; set; }
        public bool CanApprove { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
