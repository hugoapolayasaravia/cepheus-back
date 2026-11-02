using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Aval / responsable de letras de un cliente. Se elige al imprimir la letra.
    ///
    /// Legacy: dbo.MClientes_Responsables_Letras (DataWindow dw_bus_cre_avales).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_cli     -> ClienteCode
    ///   SEC_LETRA      -> Sequence (correlativo del aval dentro del cliente)
    ///   APE_PAT_LET    -> PaternalSurname
    ///   APE_MAT_LET    -> MaternalSurname
    ///   DES_NOMBRE     -> FirstNames
    ///   COD_DOCUMENTO  -> DocumentTypeCode (texto simple, sin FK: el DDL legacy no se entregó)
    ///   NUM_DOCUMENTO  -> DocumentNumber
    ///   DES_DIRECCION  -> Address (dirección del aval, se imprime en la letra)
    ///   NUM_TELEFONO   -> Phone
    /// </summary>
    public class ClienteResponsableLetra : IAuditableEntity
    {
        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string Sequence { get; set; } = default!;

        public string? PaternalSurname { get; set; }
        public string? MaternalSurname { get; set; }
        public string? FirstNames { get; set; }
        public string? DocumentTypeCode { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
