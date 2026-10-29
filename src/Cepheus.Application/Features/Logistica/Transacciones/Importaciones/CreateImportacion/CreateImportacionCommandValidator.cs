// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacion/CreateImportacionCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacion
{
    public class CreateImportacionCommandValidator : AbstractValidator<CreateImportacionCommand>
    {
        public CreateImportacionCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.PlantaCode).Cascade(CascadeMode.Stop).NotEmpty()
                .MustAsync(async (c, ct) => await uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La planta indicada no existe.");

            RuleFor(x => x.FechaPoliza).NotEmpty().WithMessage("La fecha de póliza es obligatoria.");

            RuleFor(x => x.PesoNeto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PesoBruto).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Advalorem).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Sobretasa).GreaterThanOrEqualTo(0);
            RuleFor(x => x.OtrosGastos).GreaterThanOrEqualTo(0);
        }
    }
}
