// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/UpdateGuiaDetalle/UpdateGuiaDetalleCommandValidator.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.UpdateGuiaDetalle
{
    public class UpdateGuiaDetalleCommandValidator : AbstractValidator<UpdateGuiaDetalleCommand>
    {
        public UpdateGuiaDetalleCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.GuiaCode).NotEmpty().WithMessage("El número de guía es obligatorio.");
            RuleFor(x => x.ArticuloCode).NotEmpty().WithMessage("El artículo es obligatorio.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("El RowVersion es obligatorio para controlar la concurrencia.");

            RuleFor(x => x.Cantidad).ValidCantidad();
        }
    }
}
