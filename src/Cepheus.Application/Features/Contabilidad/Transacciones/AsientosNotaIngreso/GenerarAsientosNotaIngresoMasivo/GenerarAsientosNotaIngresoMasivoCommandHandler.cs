using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientosNotaIngresoMasivo
{
    public class GenerarAsientosNotaIngresoMasivoCommandHandler
        : IRequestHandler<GenerarAsientosNotaIngresoMasivoCommand, GenerarAsientosMasivoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientosNotaIngresoMasivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GenerarAsientosMasivoResponse> Handle(
            GenerarAsientosNotaIngresoMasivoCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var desde = request.FechaDesde.Date;
            var hasta = request.FechaHasta.Date.AddDays(1);

            var codigos = await _uow.Logistica.Transacciones.NotaIngresos.Query()
                .AsNoTracking()
                .Where(n => n.PlantaCode == planta
                            && n.Estado == EstadoNotaIngreso.Procesado
                            && n.FechaRecepcion >= desde
                            && n.FechaRecepcion < hasta
                            && (n.AsientoContable == null || n.AsientoContable == ""))
                .OrderBy(n => n.Code)
                .Select(n => n.Code)
                .ToListAsync(cancellationToken);

            var response = new GenerarAsientosMasivoResponse { Total = codigos.Count };

            foreach (var codigo in codigos)
            {
                try
                {
                    var resultado = await AsientoNotaIngresoGenerator.GenerateAsync(
                        _uow, planta, codigo, cancellationToken);

                    response.Resultados.Add(resultado);
                    response.Generados++;
                }
                catch (Exception ex) when (ex is InvalidOperationException
                                              or KeyNotFoundException
                                              or ArgumentException
                                              or DbUpdateException)
                {
                    _uow.ClearTracking();

                    response.Resultados.Add(new AsientoNotaIngresoResultResponse
                    {
                        PlantaCode = planta,
                        NotaIngresoCode = codigo,
                        Success = false,
                        Message = ex.Message
                    });
                    response.Fallidos++;
                }
            }

            return response;
        }
    }
}
