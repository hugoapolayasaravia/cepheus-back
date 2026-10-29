// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionByCode/GetImportacionByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionByCode
{
    public record GetImportacionByCodeQuery(string PlantaCode, string Code) : IRequest<ImportacionResponse>;
}
