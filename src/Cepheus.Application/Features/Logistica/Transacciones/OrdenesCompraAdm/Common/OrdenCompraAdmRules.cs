// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/OrdenCompraAdministracionRules.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    public static class OrdenCompraAdmRules
    {
        /// <summary>
        /// Código en TipoTransaccion para la Orden de Compra de Administración.
        /// SUPUESTO (por confirmar): "OA". Debe existir en el catálogo TipoTransaccion
        /// y tener RangoAprobacion/AprobadorAsignado configurados.
        /// </summary>
        public const string TipoTransaccionOCA = "OA";

        /// <summary>Largo del correlativo (Codigo_Com char(6)).</summary>
        public const int CorrelativeLength = 6;
    }
}
