using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.DeleteArticuloAnexo
{
    public record DeleteArticuloAnexoCommand(string SubCentroCostoCode, string ArticuloCode) : IRequest;
}
