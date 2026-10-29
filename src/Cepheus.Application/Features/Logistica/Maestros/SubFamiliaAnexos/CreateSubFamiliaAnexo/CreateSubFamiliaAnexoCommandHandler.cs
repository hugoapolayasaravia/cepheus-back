using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.CreateSubFamiliaAnexo;

public class CreateSubFamiliaAnexoCommandHandler : IRequestHandler<CreateSubFamiliaAnexoCommand, SubFamiliaAnexoResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateSubFamiliaAnexoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<SubFamiliaAnexoResponse> Handle(CreateSubFamiliaAnexoCommand request, CancellationToken ct)
    {
        var entity = new SubFamiliaAnexo
        {
            SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant(),
            SubFamiliaCode = request.SubFamiliaCode.Trim().ToUpperInvariant(),
            AccountingAccountCode = request.AccountingAccountCode.Trim(),
            AccountingAttachmentTypeCode = SubFamiliaAnexoProjection.Clean(request.AccountingAttachmentTypeCode, upper: false),
            AccountingAttachmentCode = SubFamiliaAnexoProjection.Clean(request.AccountingAttachmentCode, upper: false)
        };

        await _uow.Logistica.Maestros.SubFamiliaAnexos.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return await _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking()
            .ByKey(entity.SubCentroCostoCode, entity.SubFamiliaCode)
            .ToResponse()
            .FirstAsync(ct);
    }
}
