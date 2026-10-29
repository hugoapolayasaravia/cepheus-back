using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.CreateTrabajadorSeguro
{
    public record CreateTrabajadorSeguroCommand(
        string TrabajadorCode,
        string? EpsCode,
        string? SituacionEpsCode,
        string? NumeroSeguro,
        string? SctrTipoCode,
        string? SctrSaludCode,
        string? SctrPensionCode,
        bool EpsActivo,
        bool SeguroMedico,
        bool EssaludVida
    ) : IRequest<TrabajadorSeguroResponse>;
}
