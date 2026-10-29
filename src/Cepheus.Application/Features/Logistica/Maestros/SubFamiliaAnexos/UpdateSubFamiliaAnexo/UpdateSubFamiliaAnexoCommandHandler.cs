using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.UpdateSubFamiliaAnexo;

public class UpdateSubFamiliaAnexoCommandHandler : IRequestHandler<UpdateSubFamiliaAnexoCommand, SubFamiliaAnexoResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateSubFamiliaAnexoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<SubFamiliaAnexoResponse> Handle(UpdateSubFamiliaAnexoCommand request, CancellationToken ct)
    {
        var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
        var sfa = request.SubFamiliaCode.Trim().ToUpperInvariant();

        var current = await _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking()
            .ByKey(scc, sfa)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"No existe enlace para el subcentro {scc} y la subfamilia {sfa}.");

        var entity = new SubFamiliaAnexo
        {
            SubCentroCostoCode = scc,
            SubFamiliaCode = sfa,
            AccountingAccountCode = request.AccountingAccountCode.Trim(),
            AccountingAttachmentTypeCode = SubFamiliaAnexoProjection.Clean(request.AccountingAttachmentTypeCode, upper: false),
            AccountingAttachmentCode = SubFamiliaAnexoProjection.Clean(request.AccountingAttachmentCode, upper: false),

            CreatedAt = current.CreatedAt,
            CreatedBy = current.CreatedBy,
            RowVersion = request.RowVersion
        };

        _uow.Logistica.Maestros.SubFamiliaAnexos.Update(entity);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El enlace fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
        }

        return await _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking()
            .ByKey(scc, sfa)
            .ToResponse()
            .FirstAsync(ct);
    }
}
