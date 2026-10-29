using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.DeleteSubFamiliaAnexo;

public class DeleteSubFamiliaAnexoCommandHandler : IRequestHandler<DeleteSubFamiliaAnexoCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteSubFamiliaAnexoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteSubFamiliaAnexoCommand request, CancellationToken ct)
    {
        var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
        var sfa = request.SubFamiliaCode.Trim().ToUpperInvariant();

        var entity = await _uow.Logistica.Maestros.SubFamiliaAnexos.Query()
            .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.SubFamiliaCode == sfa, ct)
            ?? throw new KeyNotFoundException($"No existe enlace para el subcentro {scc} y la subfamilia {sfa}.");

        _uow.Logistica.Maestros.SubFamiliaAnexos.Remove(entity);
        await _uow.SaveChangesAsync(ct);
    }
}
