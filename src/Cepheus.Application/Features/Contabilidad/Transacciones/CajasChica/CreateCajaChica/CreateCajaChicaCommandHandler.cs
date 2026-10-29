using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CreateCajaChica
{
    public class CreateCajaChicaCommandHandler : IRequestHandler<CreateCajaChicaCommand, CajaChicaResponse>
    {
        private const int MaxAttempts = 3;

        private readonly IUnitOfWork _uow;

        public CreateCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(CreateCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var planta = CajaChicaRules.Normalize(request.PlantaCode);
            var tipo = CajaChicaRules.ParseTipo(request.Type);
            var inputs = request.Details ?? new List<CajaChicaDetalleInput>();

            for (var attempt = 1; ; attempt++)
            {
                var code = await SequentialCodeGenerator.NextAsync(
                    _uow.Contabilidad.Transacciones.CajasChica.Query().Select(c => c.Code),
                    length: CajaChicaRules.CodeLength,
                    entityLabel: "Cajas Chica",
                    cancellationToken);

                var lines = await CajaChicaLineBuilder.BuildAsync(_uow, code, planta, inputs, cancellationToken);
                var (monto, total) = CajaChicaRules.ComputeTotals(lines, TipoCambioRules.MonedaBase);

                var caja = new CajaChica
                {
                    Code = code,
                    PlantaCode = planta,
                    TrabajadorCode = request.TrabajadorCode.Trim(),
                    IssueDate = request.IssueDate,
                    Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                    MontoSol = monto,
                    TotalSol = total,
                    Type = tipo,
                    IsPrinted = false,
                    Status = EstadoCajaChica.Pendiente,
                    Details = lines
                };

                await _uow.Contabilidad.Transacciones.CajasChica.AddAsync(caja, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxAttempts)
                {
                    _uow.ClearTracking();
                    continue;
                }

                _uow.ClearTracking();

                return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
            }
        }
    }
}
