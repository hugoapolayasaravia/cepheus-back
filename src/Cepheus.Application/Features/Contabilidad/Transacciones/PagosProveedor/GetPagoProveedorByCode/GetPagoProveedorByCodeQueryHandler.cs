using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagoProveedorByCode
{
    public class GetPagoProveedorByCodeQueryHandler : IRequestHandler<GetPagoProveedorByCodeQuery, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPagoProveedorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(GetPagoProveedorByCodeQuery request, CancellationToken cancellationToken)
        {
            var response = await PagoProveedorMapper.LoadAsync(_uow, request.Code.Trim(), cancellationToken);

            if (response is null)
            {
                throw new KeyNotFoundException($"Pago {request.Code} no encontrado.");
            }

            return response;
        }
    }
}
