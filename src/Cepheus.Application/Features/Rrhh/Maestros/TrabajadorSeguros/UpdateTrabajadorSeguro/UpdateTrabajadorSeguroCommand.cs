using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.UpdateTrabajadorSeguro
{
    public record UpdateTrabajadorSeguroCommand(
        long Id,
        string? TrabajadorCode,
        string? EpsCode,
        string? SituacionEpsCode,
        string? NumeroSeguro,
        string? SctrTipoCode,
        string? SctrSaludCode,
        string? SctrPensionCode,
        bool EspsActivo,
        bool SeguroMedico,
        bool EssaludVida,
        byte[] RowVersion
    ) : IRequest<TrabajadorSeguroResponse>;
}
