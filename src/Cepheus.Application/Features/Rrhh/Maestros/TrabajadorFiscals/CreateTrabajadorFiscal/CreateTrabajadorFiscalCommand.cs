using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.CreateTrabajadorFiscal
{
    public record CreateTrabajadorFiscalCommand(
        string TrabajadorCode,
        bool ConInmTrabajador,
        bool Domiciliado,
        bool OtrosIngresosQuinta,
        bool RentaQuintaExonerada,
        bool MadreResFamiliar
    ) : IRequest<TrabajadorFiscalResponse>;
}
