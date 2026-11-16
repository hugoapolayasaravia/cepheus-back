using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetResumenPendientes;

public sealed class GetResumenPendientesQueryHandler
    : IRequestHandler<GetResumenPendientesQuery, List<ResumenPendientesItem>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAprobacionService _aprobacion;

    public GetResumenPendientesQueryHandler(IUnitOfWork uow, IAprobacionService aprobacion)
    {
        _uow = uow;
        _aprobacion = aprobacion;
    }

    public async Task<List<ResumenPendientesItem>> Handle(GetResumenPendientesQuery request, CancellationToken ct)
    {
        var nombres = await _uow.Logistica.Catalogos.TiposTransaccion
            .Query()
            .AsNoTracking()
            .Where(t => AprobacionTipos.Todos.Contains(t.Code))
            .ToDictionaryAsync(t => t.Code, t => t.Name, ct);

        var resultado = new List<ResumenPendientesItem>();

        foreach (var tipo in AprobacionTipos.Todos)
        {
            var filas = await BandejaAprobacion.LeerPendientesAsync(_uow, _aprobacion, tipo, request.Todos, ct);

            if (filas.Count == 0)
            {
                continue;
            }

            resultado.Add(new ResumenPendientesItem
            {
                TipoTransaccion = tipo,
                Nombre = nombres.TryGetValue(tipo, out var nombre) ? nombre : tipo,
                Cantidad = filas.Count
            });
        }

        return resultado;
    }
}
