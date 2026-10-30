
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Impresion;

public sealed class GetPedidoImpresionQueryHandler
    : IRequestHandler<GetPedidoImpresionQuery, PedidoImpresionResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPedidoImpresionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PedidoImpresionResponse?> Handle(
        GetPedidoImpresionQuery request,
        CancellationToken cancellationToken)
    {
        var pedido = await _unitOfWork.Logistica.Transacciones.Pedidos
            .Query()
            .AsNoTracking()
            .Include(p => p.Detalles)
            .Include(p => p.Trabajador)
            .Include(p => p.OrdenTrabajo)
            .Include(p => p.SubCentroCosto)
            .Include(p => p.Planta)
            .FirstOrDefaultAsync(
                p => p.PlantaCode == request.PlantaCode
                  && p.Code == request.PedidoCode,
                cancellationToken);

        if (pedido is null)
            return null;

        // Solicitante: relación explícita del pedido con Trabajador.
        var nombreSolicitante = ObtenerNombreTrabajador(pedido.Trabajador);

        // Aprobador: AprobadoPor contiene el nombre registrado en el pedido.
        string nombreAprobador = pedido.AprobadoPor ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(pedido.AprobadoPor))
        {
            var usuarioAprobador = await _unitOfWork.Administracion.Users
                .Query()
                .AsNoTracking()
                .Include(u => u.Trabajador)
                .FirstOrDefaultAsync(
                    u => u.Login == pedido.AprobadoPor,
                    cancellationToken);

            if (usuarioAprobador is not null)
            {
                nombreAprobador = usuarioAprobador.Trabajador is not null
                    ? ObtenerNombreTrabajador(usuarioAprobador.Trabajador)
                    : $"{usuarioAprobador.FirstName} {usuarioAprobador.LastName}".Trim();
            }
        }

        return new PedidoImpresionResponse
        {
            PlantaCode = pedido.PlantaCode,
            PedidoCode = pedido.Code,

            // Fecha_Pro del procedimiento anterior corresponde a CreatedAt.
            FechaPro = pedido.CreatedAt,

            // El procedimiento anterior devolvía estos campos vacíos.
            OrigenPedido = string.Empty,
            CodigoCosto = string.Empty,

            CodigoOrdenTrabajo = pedido.OrdenTrabajoCode,
            DescripcionOrdenTrabajo = pedido.OrdenTrabajo?.Description,

            CodigoSubCentroCosto = pedido.SubCentroCostoCode,
            DescripcionSubCentroCosto = pedido.SubCentroCosto?.Name,

            Observaciones = pedido.Observaciones,

            CodigoTrabajadorSolicitante = pedido.TrabajadorCode,
            NombreUsuario = nombreSolicitante,

            AprobadoPor = pedido.AprobadoPor,
            NombreAprobador = nombreAprobador,

            DescripcionPlanta = pedido.Planta?.Name,

            Detalles = pedido.Detalles
                .OrderBy(d => d.ItemNumber)
                .Select(d => new PedidoImpresionDetalleResponse
                {
                    ArticuloCode = d.ArticuloCode,
                    ItemNumber = d.ItemNumber,
                    DescripcionArticulo = d.DescripcionArticulo,
                    CantidadArticulo = d.CantidadArticulo,
                    UnidadMedidaCode = d.UnidadMedidaCode
                })
                .ToList()
        };
    }

    private static string ObtenerNombreTrabajador(
        Cepheus.Domain.Rrhh.Maestros.Trabajador? trabajador)
    {
        if (trabajador is null)
            return string.Empty;

        return string.Join(" ", new[]
        {
            trabajador.FirstNames,
            trabajador.PaternalSurname,
            trabajador.MaternalSurname
        }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
