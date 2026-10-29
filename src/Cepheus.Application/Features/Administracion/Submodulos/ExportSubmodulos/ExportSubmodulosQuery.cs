using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.ExportSubmodulos
{
    public class ExportSubmodulosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public int? ModuloId { get; set; }
    }
}
