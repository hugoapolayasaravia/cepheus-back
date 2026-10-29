// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/Common/IOrdenCompraAdministracionLinea.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common
{
    /// <summary>Campos de una línea, compartidos por Create y Update para validarlos con una sola clase base.</summary>
    public interface IOrdenCompraAdmLinea
    {
        string Glosa { get; }
        string UnidadMedidaCode { get; }
        decimal Cantidad { get; }
        decimal Precio { get; }
        decimal Descuento { get; }
        string SubCentroCostoCode { get; }
    }
}
