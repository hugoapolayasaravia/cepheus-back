using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CreateProgramacionOtros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.UpdateProgramacionOtros;


public sealed class UpdateProgramacionOtrosCommandValidator : AbstractValidator<UpdateProgramacionOtrosCommand>
{
    public UpdateProgramacionOtrosCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.ProgramTime).NotEmpty().MaximumLength(5);
            RuleFor(x => x.Data.PlantCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Data.TransportObservations).MaximumLength(100);
            RuleFor(x => x.Data.Detalles).NotEmpty().WithMessage("La programación debe tener al menos una línea.");
            RuleForEach(x => x.Data.Detalles).SetValidator(new ProgramacionOtrosDetalleRequestValidator());
            RuleFor(x => x.Data.Detalles)
                .Must(d => d.Select(l => l.Item).Distinct().Count() == d.Count)
                .WithMessage("No repita la misma línea de la OT.");
        });
    }
}
