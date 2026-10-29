using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.UpdateOrdenServicioSalida;

public sealed class UpdateOrdenServicioSalidaCommandValidator : AbstractValidator<UpdateOrdenServicioSalidaCommand>
{
    public UpdateOrdenServicioSalidaCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.TrabajadorCode).NotEmpty().MaximumLength(5)
            .WithMessage("El trabajador responsable es obligatorio.");
    }
}
