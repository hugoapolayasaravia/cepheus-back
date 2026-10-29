using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CreateDetraccion
{
    public class CreateDetraccionCommandValidator : AbstractValidator<CreateDetraccionCommand>
    {
        public CreateDetraccionCommandValidator()
        {
            // Nota: no se valida el cruce de periodos entre lotes. Un documento solo puede estar en un
            // lote (índice único), así que varios lotes del mismo mes son válidos (el legacy tenía
            // esa validación comentada).
            DetraccionPeriodRules.Apply(this);
        }
    }
}
