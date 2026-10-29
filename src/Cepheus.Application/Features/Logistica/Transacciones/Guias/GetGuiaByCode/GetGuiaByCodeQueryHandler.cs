// Cepheus.Application/Features/Logistica/Transacciones/Guias/GetGuiaByCode/GetGuiaByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiaByCode
{
    /// <summary>
    /// Devuelve la guía completa (cabecera + detalle + datos de impresión).
    /// Reemplaza a Logi_sp_Listado_MGuias_Detalle y Logi_sp_Rpt_Guias.
    /// </summary>
    public class GetGuiaByCodeQueryHandler : IRequestHandler<GetGuiaByCodeQuery, GuiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetGuiaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(GetGuiaByCodeQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, code, cancellationToken);
        }
    }
}
