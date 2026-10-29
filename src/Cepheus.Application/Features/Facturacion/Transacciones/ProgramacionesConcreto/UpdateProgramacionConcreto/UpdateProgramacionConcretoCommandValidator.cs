using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CreateProgramacionConcreto;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.UpdateProgramacionConcreto;

public sealed class UpdateProgramacionConcretoCommandValidator : AbstractValidator<UpdateProgramacionConcretoCommand>
{
    public UpdateProgramacionConcretoCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.ProgramTime).NotEmpty().MaximumLength(5);
            RuleFor(x => x.Data.PlantCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Data.ElementoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Data.TransportObservations).MaximumLength(50);
            RuleFor(x => x.Data.Detalles).NotEmpty().WithMessage("La programación debe tener al menos una línea.");
            RuleForEach(x => x.Data.Detalles).SetValidator(new ProgramacionConcretoDetalleRequestValidator());
            RuleFor(x => x.Data.Detalles)
                .Must(d => d.Select(l => l.Item).Distinct().Count() == d.Count)
                .WithMessage("No repita la misma línea de la OT.");
        });
    }
}
