using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.UpdateTrabajadorFiscal
{
    public record UpdateTrabajadorFiscalCommand(
        long Id,
        string? TrabajadorCode,
        bool ConInmTrabajador,
        bool Domiciliado,
        bool OtrosIngresosQuinta,
        bool RentaQuintaExonerada,
        bool MadreResFamiliar,
        byte[] RowVersion
    ) : IRequest<TrabajadorFiscalResponse>;
}
