using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.CreatePlantillaAsiento
{
    public class CreatePlantillaAsientoCommandValidator : PlantillaAsientoValidator<CreatePlantillaAsientoCommand>
    {
        public CreatePlantillaAsientoCommandValidator(IUnitOfWork uow) : base(uow)
        {
        }
    }
}
