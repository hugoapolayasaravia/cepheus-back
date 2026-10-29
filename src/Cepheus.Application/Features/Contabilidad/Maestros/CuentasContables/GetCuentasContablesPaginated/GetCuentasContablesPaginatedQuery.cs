using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentasContablesPaginated
{
    public class GetCuentasContablesPaginatedQuery : PagedRequest, IRequest<PagedResult<CuentaContableResponse>>
    {
        /// <summary>Cantidad de dígitos del código (0 o vacío = todos). Parámetro @digitos del legacy.</summary>
        public int? Digits { get; set; }

        /// <summary>El código empieza con este valor. Parámetro @codigo del legacy.</summary>
        public string? Code { get; set; }

        /// <summary>La descripción contiene este valor. Parámetro @nombre del legacy.</summary>
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
