// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionNotasIngreso/GetImportacionNotasIngresoQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionNotasIngreso
{
    /// <summary>Notas de Ingreso generadas desde la importación (legacy: ue_ver_notasingreso, origen 'I').</summary>
    public record GetImportacionNotasIngresoQuery(string PlantaCode, string ImportacionCode)
        : IRequest<List<NotaIngresoResponse>>;
}
