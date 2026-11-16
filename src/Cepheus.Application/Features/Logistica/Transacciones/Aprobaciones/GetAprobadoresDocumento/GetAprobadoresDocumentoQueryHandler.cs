using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetAprobadoresDocumento;

public sealed class GetAprobadoresDocumentoQueryHandler
    : IRequestHandler<GetAprobadoresDocumentoQuery, AprobadoresDocumentoResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IAprobacionService _aprobacion;

    public GetAprobadoresDocumentoQueryHandler(IUnitOfWork uow, IAprobacionService aprobacion)
    {
        _uow = uow;
        _aprobacion = aprobacion;
    }

    public async Task<AprobadoresDocumentoResponse> Handle(GetAprobadoresDocumentoQuery request, CancellationToken ct)
    {
        var tipo = AprobacionTipos.Normalizar(request.Tipo);
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var code = request.Code.Trim().ToUpperInvariant();

        var documento = await BandejaAprobacion.CargarDocumentoAsync(_uow, tipo, planta, code, ct);
        var resultado = await _aprobacion.ObtenerHabilitadosAsync(documento, ct);

        var codigos = resultado.Habilitados.Select(h => h.TrabajadorCode).Distinct().ToList();

        var trabajadores = await _uow.Rrhh.Maestros.Trabajadores
            .Query()
            .AsNoTracking()
            .Where(t => codigos.Contains(t.Code))
            .Select(t => new { t.Code, t.FirstNames, t.PaternalSurname, t.MaternalSurname, t.Email })
            .ToListAsync(ct);

        var logins = await _uow.Administracion.Users
            .Query()
            .AsNoTracking()
            .Where(u => u.TrabajadorCode != null && codigos.Contains(u.TrabajadorCode))
            .Select(u => new { u.TrabajadorCode, u.Login })
            .ToListAsync(ct);

        var response = new AprobadoresDocumentoResponse
        {
            TipoTransaccion = documento.TipoTransaccion,
            PlantaCode = documento.PlantaCode,
            Code = documento.Code,
            Monto = documento.Monto,
            MonedaCode = documento.MonedaCode,
            Observaciones = resultado.Observaciones.ToList()
        };

        foreach (var h in resultado.Habilitados)
        {
            var t = trabajadores.FirstOrDefault(x => x.Code == h.TrabajadorCode);

            var nombre = t is null
                ? h.TrabajadorCode
                : string.Join(' ', new[] { t.PaternalSurname, t.MaternalSurname, t.FirstNames }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            response.Aprobadores.Add(new AprobadorDocumentoItem
            {
                NivelCode = h.NivelCode,
                Rol = h.Rol,
                TrabajadorCode = h.TrabajadorCode,
                Nombre = nombre,
                Login = logins.FirstOrDefault(x => x.TrabajadorCode == h.TrabajadorCode)?.Login,
                Email = t?.Email
            });
        }

        return response;
    }
}
