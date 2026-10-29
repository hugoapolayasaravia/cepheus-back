using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexo;

public class GetSubFamiliaAnexoQueryHandler : IRequestHandler<GetSubFamiliaAnexoQuery, SubFamiliaAnexoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetSubFamiliaAnexoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<SubFamiliaAnexoResponse> Handle(GetSubFamiliaAnexoQuery request, CancellationToken ct)
    {
        var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
        var sfa = request.SubFamiliaCode.Trim().ToUpperInvariant();

        return await _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking()
            .ByKey(scc, sfa)
            .ToResponse()
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"No existe enlace para el subcentro {scc} y la subfamilia {sfa}.");
    }
}
