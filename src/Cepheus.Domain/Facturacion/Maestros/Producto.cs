using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Catalogos;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Producto de venta (concreto premezclado, agregados, servicios de bombeo y
    /// transporte, etc.). Catálogo maestro del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.MProductos (SQL Server). PK compuesta (Codigo_tpr, Codigo_prd):
    /// se mantiene como PK compuesta (TipoProductoCode, Code); Code es un correlativo
    /// de 4 dígitos POR tipo de producto. La columna calculada legacy
    /// "Codigo" (= Codigo_tpr + Codigo_prd) es ahora la propiedad FullCode, que no se
    /// guarda en la tabla.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tpr          -> TipoProductoCode (FK -> TipoProducto)
    ///   Codigo_prd          -> Code (correlativo por tipo)
    ///   Descripcion_prd     -> Name;  DesAbreviada_prd -> ShortName (opcional)
    ///   Unidad_prd          -> UnitCode (FK -> UnidadMedida; antes sin FK)
    ///   Codigo_cta          -> AccountingAccountCode
    ///   CtaTransporte_prd   -> TransportAccountCode
    ///   ctancredito_prd     -> CreditNoteAccountCode
    ///                          (las tres cuentas contables quedan como texto de 8
    ///                          caracteres, sin FK: aún no existe plan de cuentas en
    ///                          Cepheus; mismo criterio que Articulo.AccountingAccountCode)
    ///   codigo_est          -> reemplazado por IsActive (ver análisis de TEstados)
    ///   longtope_prd        -> LengthLimit (longitud tope)
    ///   Codigotrans_tpr, Codigotrans_prd -> TransportTipoProductoCode, TransportCode
    ///                          (FK compuesta opcional a Producto: producto de transporte)
    ///   Categoria_Prd       -> CategoryCode (FK -> CategoriaProducto)
    ///   COD_RESISTENCIA, COD_TIP_CEMENTO, COD_TAM_PIEDRA, COD_SLUMP,
    ///   COD_AGUA_CEMENTO, COD_EDAD, COD_COND_ESPECIAL, COD_PROP_MEZCLA
    ///                       -> StrengthCode, CementTypeCode, StoneSizeCode, SlumpCode,
    ///                          WaterCementRatioCode, AgeCode, SpecialConditionCode,
    ///                          MixProportionCode (FK -> AtributoConcreto; el tipo de
    ///                          atributo de cada columna se valida en la aplicación)
    ///   IND_CONC_BOMBEABLE  -> IsPumpable (char(1) NULL -> bool)
    ///   INDI_AFE_DETRACCION -> IsSubjectToDetraction (char(1) '0'/'1' -> bool)
    ///   COD_TIPO_BIENES     -> GoodsTypeCode (FK -> TipoBien)
    ///   COD_TIP_OPERACION   -> OperationTypeCode (FK -> TipoOperacion)
    ///   Valor_cemento       -> CementValue (numeric(18,2) NULL)
    ///
    /// Agregados frente al legacy: IsActive, auditoría (IAuditableEntity) y RowVersion.
    /// Reemplaza la versión anterior de Producto (owned type EspecificacionConcreto):
    /// sus atributos ahora son propiedades con FK al catálogo AtributoConcreto.
    /// </summary>
    public class Producto : IAuditableEntity
    {
        public string TipoProductoCode { get; set; } = default!;
        public TipoProducto TipoProducto { get; set; } = default!;

        /// <summary>Correlativo de 4 dígitos por tipo de producto (Codigo_prd).</summary>
        public string Code { get; set; } = default!;

        /// <summary>Código completo (tipo + correlativo). Calculado, no se guarda.</summary>
        public string FullCode => TipoProductoCode + Code;

        public string Name { get; set; } = default!;
        public string? ShortName { get; set; }

        public string UnitCode { get; set; } = default!;
        public UnidadMedidaVenta Unit { get; set; } = default!;

        public string? AccountingAccountCode { get; set; }
        public string? TransportAccountCode { get; set; }
        public string? CreditNoteAccountCode { get; set; }

        public decimal LengthLimit { get; set; }

        /// <summary>Producto que factura el transporte de este producto (opcional).</summary>
        public string? TransportTipoProductoCode { get; set; }
        public string? TransportCode { get; set; }
        public Producto? TransportProducto { get; set; }

        public string? CategoryCode { get; set; }
        public CategoriaProducto? Category { get; set; }

        // Especificación técnica del concreto (opcionales)
        /// <summary>Resistencia (legacy COD_RESISTENCIA -> AtributoConcreto.Code).</summary>
        public string? StrengthCode { get; set; }

        /// <summary>TipoCemento (legacy COD_TIP_CEMENTO -> AtributoConcreto.Code).</summary>
        public string? CementTypeCode { get; set; }

        /// <summary>TamanoPiedra (legacy COD_TAM_PIEDRA -> AtributoConcreto.Code).</summary>
        public string? StoneSizeCode { get; set; }

        /// <summary>Slump (legacy COD_SLUMP -> AtributoConcreto.Code).</summary>
        public string? SlumpCode { get; set; }

        /// <summary>RelacionAguaCemento (legacy COD_AGUA_CEMENTO -> AtributoConcreto.Code).</summary>
        public string? WaterCementRatioCode { get; set; }

        /// <summary>Edad (legacy COD_EDAD -> AtributoConcreto.Code).</summary>
        public string? AgeCode { get; set; }

        /// <summary>CondicionEspecial (legacy COD_COND_ESPECIAL -> AtributoConcreto.Code).</summary>
        public string? SpecialConditionCode { get; set; }

        /// <summary>ProporcionMezcla (legacy COD_PROP_MEZCLA -> AtributoConcreto.Code).</summary>
        public string? MixProportionCode { get; set; }

        public bool IsPumpable { get; set; }

        public bool IsSubjectToDetraction { get; set; }

        public string? GoodsTypeCode { get; set; }
        public TipoBien? GoodsType { get; set; }

        public string? OperationTypeCode { get; set; }
        public TipoOperacion? OperationType { get; set; }

        public decimal? CementValue { get; set; }

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
