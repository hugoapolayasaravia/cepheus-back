// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaMapper.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    internal static class GuiaMapper
    {
        public static GuiaResponse ToResponse(Guia g) => new()
        {
            PlantaCode = g.PlantaCode,
            PlantaName = g.Planta?.Name,

            Code = g.Code,
            Serie = g.Code.Length >= 3 ? g.Code.Substring(0, 3) : g.Code,
            Numero = g.Code.Length > 4 ? g.Code.Substring(4) : string.Empty,

            FechaEmision = g.FechaEmision,
            Hora = g.Hora,

            ProveedorCode = g.ProveedorCode,
            ProveedorName = g.Proveedor?.LegalName,
            ProveedorDocumentNumber = g.Proveedor?.DocumentNumber,

            MotivoCode = g.MotivoCode,
            MotivoName = g.Motivo?.Name,

            Direccion = g.Direccion,
            PuntoPartida = g.PuntoPartida,

            TransportistaCode = g.TransportistaCode,
            TransportistaName = g.Transportista?.LegalName,
            TransportistaDocumentNumber = g.Transportista?.DocumentNumber,
            TransportistaAddress = g.Transportista?.Address,
            TransportistaCertificationNumber = g.Transportista?.CertificationNumber,

            ConductorCode = g.ConductorCode,
            ConductorName = g.Conductor is null ? null : $"{g.Conductor.FirstName} {g.Conductor.LastName}",
            ConductorDriverLicenseNumber = g.Conductor?.DriverLicenseNumber,

            VehiculoCode = g.VehiculoCode,
            VehiculoLicensePlate = g.Vehiculo?.LicensePlate,
            VehiculoBrand = g.Vehiculo?.Brand,

            Observaciones = g.Observaciones,
            Estado = g.Estado.ToString(),

            Detalles = g.Detalles
                .OrderBy(d => d.ItemNumber)
                .Select(ToDetalleResponse)
                .ToList(),

            CreatedAt = g.CreatedAt,
            CreatedBy = g.CreatedBy,
            UpdatedAt = g.UpdatedAt,
            RowVersion = g.RowVersion
        };

        public static GuiaDetalleResponse ToDetalleResponse(GuiaDetalle d) => new()
        {
            PlantaCode = d.PlantaCode,
            GuiaCode = d.GuiaCode,
            ItemNumber = d.ItemNumber,
            ArticuloCode = d.ArticuloCode,
            ArticuloName = d.Articulo?.Name,
            UnidadMedidaCode = d.Articulo?.UnidadMedidaCode,
            Cantidad = d.Cantidad,
            IsVerified = d.IsVerified,
            Estado = d.Estado.ToString(),
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            RowVersion = d.RowVersion
        };

        public static GuiaListItemResponse ToListItem(Guia g) => new()
        {
            PlantaCode = g.PlantaCode,
            PlantaName = g.Planta?.Name,
            Code = g.Code,
            FechaEmision = g.FechaEmision,
            Hora = g.Hora,
            CreatedBy = g.CreatedBy,
            ProveedorCode = g.ProveedorCode,
            ProveedorName = g.Proveedor?.LegalName,
            MotivoCode = g.MotivoCode,
            MotivoName = g.Motivo?.Name,
            Estado = g.Estado.ToString(),
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt,
            RowVersion = g.RowVersion
        };
    }
}
