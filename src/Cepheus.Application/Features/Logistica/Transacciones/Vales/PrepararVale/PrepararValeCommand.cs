using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.PrepararVale;

/// <summary>Marca el vale como preparado para despacho (PB: botón Preparar; Prepara_val = 'S').</summary>
public sealed record PrepararValeCommand(string PlantaCode, string Code) : IRequest<ValeResponse>;
