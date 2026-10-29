using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.UpdateSubFamiliaAnexo;

public class UpdateSubFamiliaAnexoCommandValidator : AbstractValidator<UpdateSubFamiliaAnexoCommand>
{
    public UpdateSubFamiliaAnexoCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.SubCentroCostoCode).NotEmpty().MaximumLength(6);
        RuleFor(x => x.SubFamiliaCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión del registro es obligatoria.");

        SubFamiliaAnexoRules.AddAccountingRules(
            this, uow, x => x.AccountingAccountCode, x => x.AccountingAttachmentTypeCode, x => x.AccountingAttachmentCode);
    }
}
