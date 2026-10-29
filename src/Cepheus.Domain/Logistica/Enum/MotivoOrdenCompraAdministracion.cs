// Cepheus.Domain/Logistica/Enum/MotivoOrdenCompraAdministracion.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Motivo de la Orden de Compra de Administración.
    /// Legacy: dbo.MCompraR.Codigo_Mot (char(2)):
    /// 01 Otros, 02 Detracciones, 03 Publicaciones, 04 Seminarios,
    /// 05 Artículos Promocionales, 06 Folletería, 07 Auspicio.
    /// El valor numérico del enum coincide con el código legacy.
    /// </summary>
    public enum MotivoOrdenCompraAdministracion
    {
        Otros = 1,
        Detracciones = 2,
        Publicaciones = 3,
        Seminarios = 4,
        ArticulosPromocionales = 5,
        Folleteria = 6,
        Auspicio = 7
    }
}
