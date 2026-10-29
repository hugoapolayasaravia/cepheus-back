using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoValeStock;

/// <summary>
/// Líneas de la Nota de Ingreso que dejarían saldo negativo al generar el vale (la grilla que el botón "Vales" muestra
/// y con la que se detiene). Lista vacía = se puede generar el vale.
/// </summary>
public sealed record GetNotaIngresoValeStockQuery(string PlantaCode, string NotaIngresoCode)
    : IRequest<IReadOnlyList<LineaSinStock>>;
