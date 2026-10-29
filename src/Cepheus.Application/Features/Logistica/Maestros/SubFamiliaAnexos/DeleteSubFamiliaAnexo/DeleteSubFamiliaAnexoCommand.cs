using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.DeleteSubFamiliaAnexo;

public record DeleteSubFamiliaAnexoCommand(string SubCentroCostoCode, string SubFamiliaCode) : IRequest;
