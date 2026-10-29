using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequeVentaByKey
{
    public class GetChequeVentaByKeyQueryHandler : IRequestHandler<GetChequeVentaByKeyQuery, ChequeVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetChequeVentaByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChequeVentaResponse> Handle(GetChequeVentaByKeyQuery request, CancellationToken cancellationToken)
        {
            var cliente = request.ClienteCode.Trim();
            var banco = ChequeVentaRules.Normalize(request.BancoCode);
            var number = request.Number.Trim();

            return await ChequeVentaMapper.LoadAsync(_uow, cliente, banco, number, cancellationToken)
                   ?? throw new KeyNotFoundException(
                       $"Cheque {ChequeVentaRules.FormatKey(cliente, banco, number)} no encontrado.");
        }
    }
}
