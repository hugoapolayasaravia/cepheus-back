using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.ExportAtributosConcreto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportAtributosConcretoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? AttributeType { get; set; }
    }
}
