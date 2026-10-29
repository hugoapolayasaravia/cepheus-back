using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Obra: sede o local de un cliente donde se presta el servicio/entrega la
    /// mercadería. Entidad maestra del módulo de Facturación, PK compuesta.
    ///
    /// Legacy: dbo.MObras (SQL Server), PK compuesta (Codigo_cli, Codigo_obr).
    /// Se mantiene la PK compuesta — mismo criterio que OrdenTrabajo en
    /// Mantenimiento (PlantaCode, Code) — confirmado explícitamente por el
    /// usuario: Codigo_obr es un correlativo de 3 dígitos por cliente, no un
    /// código único global.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_cli            -> ClienteCode (FK -> Cliente.Code, parte de la PK)
    ///   Codigo_obr            -> Code (correlativo por cliente, parte de la PK)
    ///   Descripcion_obr       -> Description
    ///   Direccion_obr         -> Address
    ///   UbiDireccion_obr      -> UbigeoCode (FK -> Comunes.Ubigeo.Code)
    ///   Observaciones_obr     -> Observations (nullable acá; el legacy la exigía
    ///                            NOT NULL, mismo caso que Cliente.Observations)
    ///   Estado_obr            -> Estado (enum EstadoObra)
    ///   direntrega_obr        -> DeliveryAddress (nullable, como el legacy)
    ///   ubidirentrega_obr     -> DeliveryUbigeoCode (FK -> Comunes.Ubigeo.Code).
    ///                            NOTA: el legacy la declara NOT NULL aunque
    ///                            DeliveryAddress sí es nullable — inconsistencia
    ///                            original que se preserva; confirmar si en la
    ///                            práctica siempre viene informada junto con la
    ///                            dirección de entrega.
    ///   dircobranza_obr       -> BillingAddress
    ///   ubidircobranza_obr    -> BillingUbigeoCode (FK -> Comunes.Ubigeo.Code)
    ///   Responsable_obr       -> ResponsibleName (nullable, como el legacy)
    ///   TelResponsable_obr    -> ResponsiblePhone
    ///   EmailResponsable_obr  -> ResponsibleEmail
    ///   CodigoPgv_obr         -> FormaPagoVentaCode (FK -> Facturacion.FormaPagoVenta.Code,
    ///                            nullable como el legacy: cuando no se informa,
    ///                            se asume la forma de pago del Cliente)
    ///   Codigo_cob            -> CobradorCode. PENDIENTE DE FK REAL: la tabla
    ///                            Cobrador aún no existe en el repositorio (en
    ///                            desarrollo, según lo indicado). Se guarda como
    ///                            código simple sin relación de navegación EF,
    ///                            mismo criterio que OrdenTrabajo.ResponsableCode
    ///                            en Mantenimiento mientras Trabajador no existía.
    ///   Codigo_ven            -> VendedorCode. PENDIENTE DE FK REAL, igual
    ///                            criterio que CobradorCode (tabla Vendedor aún
    ///                            no existe en el repositorio).
    ///   COD_ANALISIS          -> AnalisisVentaCode (FK -> Facturacion.AnalisisVenta.Code,
    ///                            nullable)
    ///   COD_SEGMENTO          -> ELIMINADO. El script legacy no declara FK para
    ///                            esta columna (a diferencia de
    ///                            GES_MAE_ANALISIS_VENTAS.COD_SEGMENTO, que sí la
    ///                            tiene) y su valor es derivable a través de
    ///                            AnalisisVenta.SegmentoVentasCode. Se propone
    ///                            eliminarla para no duplicar la fuente de verdad
    ///                            del segmento (punto 7: mejora aceptada previo
    ///                            análisis). Si en la práctica una obra necesita
    ///                            un segmento distinto al de su análisis de
    ///                            ventas, avisar para revertir esta decisión.
    ///   Credito_cli           -> CreditLimit (numeric(18,2) en el legacy tenía
    ///                            nombre de Cliente por error de copia/paste;
    ///                            es la línea de crédito de la obra, se renombra
    ///                            para reflejarlo)
    ///   Moneda_cre            -> CreditCurrencyCode (FK -> Comunes.Moneda.Code).
    ///                            NOTA: mismo caso que Cliente.CurrencyCode — el
    ///                            legacy guarda char(1) numérico y Comunes.Moneda
    ///                            usa código ISO alfabético; confirmar equivalencia
    ///                            antes de migrar datos.
    ///   FecIngreso_obr        -> EntryDate
    ///   Recargo_obr           -> HasSurcharge
    ///   NombreCorto_obr       -> ShortName
    ///   codigo_val            -> TipoValorizacionCode (FK -> Facturacion.TipoValorizacion.Code,
    ///                            nullable)
    ///   Dias_Val              -> RequiresValorizacion (char(1) 'SI'/'NO' en el
    ///                            legacy -> bool?, nullable)
    ///   NUM_DIAS_MES          -> ScheduledWeekday (enum DiaSemana?, ver
    ///                            Facturacion.Enum.DiaSemana para el detalle de
    ///                            la numeración legacy y la falta de
    ///                            documentación sobre su propósito exacto)
    ///   Ind_Proyecto          -> IsProject
    ///   Flag_Impresion        -> RequiresPrinting
    ///   FEC_TERMINO           -> CompletionDate (se completa junto con
    ///                            CompletionUser al pasar Estado a Terminada,
    ///                            mismo criterio que Proveedor.DeactivatedAt/By)
    ///   Usuario_FEC_TERMINO   -> CompletionUser
    ///   ind_cli_gerencia      -> RequiresManagementApproval (columna separada de
    ///                            la homónima en Cliente; el legacy la duplica en
    ///                            ambas tablas)
    /// </summary>
    public class Obra : IAuditableEntity
    {
        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        /// <summary>Correlativo por cliente (Codigo_obr, 3 caracteres en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string UbigeoCode { get; set; } = default!;
        public Ubigeo Ubigeo { get; set; } = default!;

        public string? Observations { get; set; }

        public EstadoObra Estado { get; set; } = EstadoObra.Activo;

        public string? DeliveryAddress { get; set; }
        public string DeliveryUbigeoCode { get; set; } = default!;
        public Ubigeo DeliveryUbigeo { get; set; } = default!;

        public string BillingAddress { get; set; } = default!;
        public string BillingUbigeoCode { get; set; } = default!;
        public Ubigeo BillingUbigeo { get; set; } = default!;

        public string? ResponsibleName { get; set; }
        public string ResponsiblePhone { get; set; } = default!;
        public string ResponsibleEmail { get; set; } = default!;

        public string? FormaPagoVentaCode { get; set; }
        public FormaPagoVenta? FormaPagoVenta { get; set; }

        /// <summary>Pendiente de FK real: Cobrador aún no existe como tabla.</summary>
        public string CobradorCode { get; set; } = default!;

        /// <summary>Pendiente de FK real: Vendedor aún no existe como tabla.</summary>
        public string VendedorCode { get; set; } = default!;

        public string? AnalisisVentaCode { get; set; }
        public AnalisisVenta? AnalisisVenta { get; set; }

        public decimal CreditLimit { get; set; }

        public string CreditCurrencyCode { get; set; } = default!;
        public Moneda CreditCurrency { get; set; } = default!;

        public DateTime? EntryDate { get; set; }

        public bool HasSurcharge { get; set; }

        public string ShortName { get; set; } = default!;

        public string? TipoValorizacionCode { get; set; }
        public TipoValorizacion? TipoValorizacion { get; set; }

        public bool? RequiresValorizacion { get; set; }

        public DiaSemana? ScheduledWeekday { get; set; }

        public bool IsProject { get; set; }
        public bool RequiresPrinting { get; set; }

        public DateTime? CompletionDate { get; set; }
        public string? CompletionUser { get; set; }

        public bool? RequiresManagementApproval { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
