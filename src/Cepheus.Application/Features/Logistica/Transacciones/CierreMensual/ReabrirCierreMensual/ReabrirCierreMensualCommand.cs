using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ReabrirCierreMensual;

/// <summary>Reabre el ÚLTIMO período cerrado de la planta (el legacy no tenía forma de hacerlo).</summary>
public sealed record ReabrirCierreMensualCommand(string PlantaCode) : IRequest<ReabrirCierreMensualResponse>;

public sealed record ReabrirCierreMensualResponse(
    string PlantaCode,
    string PeriodoReabierto,
    int DocumentosRestaurados,
    int SaldosEliminados);
