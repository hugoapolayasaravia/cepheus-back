using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.ExportModulos
{
    public class ExportModulosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
