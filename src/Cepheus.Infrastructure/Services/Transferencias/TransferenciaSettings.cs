using Cepheus.Application.Comun.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Cepheus.Infrastructure.Services.Transferencias;

public sealed class TransferenciaSettings : ITransferenciaSettings
{
    /// <summary>Valor del legacy, usado si la clave no está configurada.</summary>
    public const string ProveedorPorDefecto = "00001";

    public TransferenciaSettings(IConfiguration configuration)
    {
        var configurado = configuration["Logistica:ProveedorTransferenciasCode"];
        ProveedorCode = string.IsNullOrWhiteSpace(configurado) ? ProveedorPorDefecto : configurado.Trim();
    }

    public string ProveedorCode { get; }
}
