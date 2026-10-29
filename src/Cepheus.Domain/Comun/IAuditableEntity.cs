namespace Cepheus.Domain.Comun
{
    /// <summary>
    /// Contrato de auditoría que deben implementar todas las entidades del dominio
    /// que requieran registrar quién y cuándo las creó/modificó.
    /// El seteo de estos campos se realiza automáticamente desde
    /// ApplicationDbContext.SaveChangesAsync (Infrastructure), no manualmente
    /// en los Command Handlers.
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        string? UpdatedBy { get; set; }
    }




}
