using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.UpdateCuentaContable
{
    public record UpdateCuentaContableCommand(
        string Code,
        string Name,
        string TipoCuentaCode,
        string? TipoAnexoCode,
        string? LinkAccount1Code,
        string? LinkAccount2Code,
        string CurrencyCode,
        string? AcmFlag,
        string? PilotFlag,
        string? GainsLossesFlag,
        string? ClosingAccountCode,
        byte[] RowVersion
    ) : IRequest<CuentaContableResponse>;
}
