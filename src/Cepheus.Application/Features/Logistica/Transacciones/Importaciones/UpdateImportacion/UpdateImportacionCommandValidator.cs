// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacion/UpdateImportacionCommandValidator.cs
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacion
{
    public class UpdateImportacionCommandValidator : AbstractValidator<UpdateImportacionCommand>
    {
        public UpdateImportacionCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.FechaPoliza).NotEmpty().WithMessage("La fecha de póliza es obligatoria.");
            RuleFor(x => x.PesoNeto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PesoBruto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Advalorem).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Sobretasa).GreaterThanOrEqualTo(0);
            RuleFor(x => x.OtrosGastos).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
