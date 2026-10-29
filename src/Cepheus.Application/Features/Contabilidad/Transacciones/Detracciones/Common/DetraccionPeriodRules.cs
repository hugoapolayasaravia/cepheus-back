using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    /// <summary>Comando con año, periodo y rango de fechas de un lote de detracción.</summary>
    public interface IDetraccionPeriodRequest
    {
        string Year { get; }
        string Period { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }

    /// <summary>
    /// Reglas de año, periodo y fechas compartidas por la creación y la modificación de un lote
    /// (legacy: validaciones de ue_set_btn_grb_data).
    /// </summary>
    public static class DetraccionPeriodRules
    {
        public static void Apply<T>(AbstractValidator<T> validator) where T : IDetraccionPeriodRequest
        {
            validator.RuleFor(x => x.Year)
                .Must(y => DetraccionRules.IsValidYear(y?.Trim()))
                .WithMessage("El año debe tener 4 dígitos.");

            validator.RuleFor(x => x.Period)
                .Must(p => DetraccionRules.IsValidPeriod(p?.Trim()))
                .WithMessage("El periodo debe ser un mes entre 01 y 12.");

            validator.RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("La fecha inicial es obligatoria.");

            validator.RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("La fecha final es obligatoria.");

            validator.RuleFor(x => x.StartDate)
                .Must((cmd, start) => start.Year.ToString("0000") == cmd.Year.Trim())
                .WithMessage("El año de la fecha inicial debe ser igual al año de la detracción.")
                .When(x => DetraccionRules.IsValidYear(x.Year?.Trim()));

            validator.RuleFor(x => x.StartDate)
                .Must((cmd, start) => start.Month.ToString("00") == cmd.Period.Trim())
                .WithMessage("El mes de la fecha inicial debe ser igual al periodo de la detracción.")
                .When(x => DetraccionRules.IsValidPeriod(x.Period?.Trim()));

            validator.RuleFor(x => x.EndDate)
                .Must((cmd, end) => end.Year.ToString("0000") == cmd.Year.Trim())
                .WithMessage("El año de la fecha final debe ser igual al año de la detracción.")
                .When(x => DetraccionRules.IsValidYear(x.Year?.Trim()));

            validator.RuleFor(x => x.EndDate)
                .Must((cmd, end) => end.Month.ToString("00") == cmd.Period.Trim())
                .WithMessage("El mes de la fecha final debe ser igual al periodo de la detracción.")
                .When(x => DetraccionRules.IsValidPeriod(x.Period?.Trim()));

            validator.RuleFor(x => x.EndDate)
                .Must((cmd, end) => cmd.StartDate.Date <= end.Date)
                .WithMessage("La fecha inicial debe ser menor o igual a la fecha final.");
        }
    }
}
