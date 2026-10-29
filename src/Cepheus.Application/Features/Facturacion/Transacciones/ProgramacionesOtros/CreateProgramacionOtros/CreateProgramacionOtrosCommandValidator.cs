using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CreateProgramacionOtros;

public sealed class CreateProgramacionOtrosCommandValidator : AbstractValidator<CreateProgramacionOtrosCommand>
{
    public CreateProgramacionOtrosCommandValidator()
    {
        RuleFor(x => x.OrdenTrabajoCode).NotEmpty().MaximumLength(8);
        RuleFor(x => x.ProgramTime).NotEmpty().MaximumLength(5);
        RuleFor(x => x.PlantCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.TransportObservations).MaximumLength(100);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La programación debe tener al menos una línea.");
        RuleForEach(x => x.Detalles).SetValidator(new ProgramacionOtrosDetalleRequestValidator());
        RuleFor(x => x.Detalles)
            .Must(d => d.Select(l => l.Item).Distinct().Count() == d.Count)
            .WithMessage("No repita la misma línea de la OT.");
    }
}

public sealed class ProgramacionOtrosDetalleRequestValidator : AbstractValidator<ProgramacionOtrosDetalleRequest>
{
    public ProgramacionOtrosDetalleRequestValidator()
    {
        RuleFor(x => x.Item).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0m);
    }
}
