using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexo;

public record GetSubFamiliaAnexoQuery(string SubCentroCostoCode, string SubFamiliaCode) : IRequest<SubFamiliaAnexoResponse>;
