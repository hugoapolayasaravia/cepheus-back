using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.UpdatePlantillaAsiento
{
    public class UpdatePlantillaAsientoCommandValidator : PlantillaAsientoValidator<UpdatePlantillaAsientoCommand>
    {
        public UpdatePlantillaAsientoCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(3);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
