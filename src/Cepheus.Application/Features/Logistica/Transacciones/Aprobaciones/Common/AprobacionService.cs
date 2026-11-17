using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;

/// <summary>
/// Implementación de <see cref="IAprobacionService"/>. Se registra Scoped: la matriz de aprobación, los
/// tipos de cambio y los acumulados se cachean durante la petición (la bandeja evalúa muchos documentos).
/// </summary>
public sealed class AprobacionService : IAprobacionService
{
    private const string TipoValeCombustible = "006";   // VC
    private const string TipoValeTransferencia = "005"; // TRA (no se aprueba)

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;

    private List<RangoAprobacion>? _rangos;
    private List<AprobadorAsignado>? _aprobadores;
    private Dictionary<string, string?>? _padres;

    private readonly Dictionary<string, decimal> _tiposCambio = new();
    private readonly Dictionary<string, List<MontoAprobado>> _acumulados = new();

    private sealed record MontoAprobado(DateTime Fecha, decimal Monto, string MonedaCode);

    public AprobacionService(IUnitOfWork uow, ICurrentUserService currentUser, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task<AprobadorIdentificado> VerificarCredencialesAsync(string? login, string? clave, CancellationToken ct)
    {
        // Sin credenciales = el usuario de la sesión (bandeja «Aprobar transacciones» del legacy: aprueba quien está logueado).
        if (string.IsNullOrWhiteSpace(login) && string.IsNullOrEmpty(clave))
        {
            if (_currentUser.UserId is null)
            {
                throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
            }

            var actual = await _uow.Administracion.Users
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value, ct)
                ?? throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");

            if (!actual.IsActive)
            {
                throw new InvalidOperationException("El usuario se encuentra inactivo.");
            }

            return new AprobadorIdentificado(
                actual.Id, actual.Login, $"{actual.FirstName} {actual.LastName}".Trim(), actual.TrabajadorCode, null, null);
        }

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(clave))
        {
            throw new InvalidOperationException("Ingrese el usuario y la clave del aprobador.");
        }

        var loginNormalizado = login.Trim().ToLower();

        var usuario = await _uow.Administracion.Users
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login.ToLower() == loginNormalizado, ct);

        // Mismo mensaje para usuario inexistente y clave errónea (no revela cuál falló).
        if (usuario is null || !_passwordHasher.Verify(clave, usuario.PasswordHash))
        {
            throw new InvalidOperationException("Usuario o contraseña inválidos, vuelva a intentar.");
        }

        if (!usuario.IsActive)
        {
            throw new InvalidOperationException("El usuario se encuentra inactivo.");
        }

        return new AprobadorIdentificado(
            usuario.Id,
            usuario.Login,
            $"{usuario.FirstName} {usuario.LastName}".Trim(),
            usuario.TrabajadorCode,
            null,
            null);
    }

    public async Task<AprobadorIdentificado> ValidarAprobadorAsync(
        DocumentoAprobable documento, string? login, string? clave, CancellationToken ct)
    {
        var aprobador = await VerificarCredencialesAsync(login, clave, ct);

        if (string.IsNullOrWhiteSpace(aprobador.TrabajadorCode))
        {
            throw new InvalidOperationException(
                $"El usuario {aprobador.Login} no tiene un Trabajador de RRHH vinculado — no puede aprobar.");
        }

        var resultado = await ObtenerHabilitadosAsync(documento, ct);
        var habilitado = resultado.Habilitados.FirstOrDefault(h => h.TrabajadorCode == aprobador.TrabajadorCode);

        if (habilitado is null)
        {
            var detalle = resultado.Observaciones.Count > 0
                ? " " + string.Join(" ", resultado.Observaciones)
                : string.Empty;

            throw new InvalidOperationException(
                $"El usuario {aprobador.Login} no está autorizado para aprobar este documento " +
                $"({documento.TipoTransaccion} {documento.PlantaCode}/{documento.Code}, " +
                $"monto {documento.MonedaCode} {documento.Monto:N2}).{detalle}");
        }

        return aprobador with { NivelCode = habilitado.NivelCode, Rol = habilitado.Rol };
    }

    public async Task<string?> ObtenerTrabajadorActualAsync(CancellationToken ct)
    {
        if (_currentUser.UserId is null)
        {
            return null;
        }

        return await _uow.Administracion.Users
            .Query()
            .AsNoTracking()
            .Where(u => u.Id == _currentUser.UserId.Value)
            .Select(u => u.TrabajadorCode)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AsegurarAprobadorActualAsync(DocumentoAprobable documento, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
        {
            throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
        }

        var trabajadorCode = await ObtenerTrabajadorActualAsync(ct);

        if (string.IsNullOrWhiteSpace(trabajadorCode))
        {
            throw new InvalidOperationException(
                "Tu usuario no tiene un Trabajador de RRHH vinculado — no puedes aprobar.");
        }

        var resultado = await ObtenerHabilitadosAsync(documento, ct);

        if (resultado.Habilitados.Any(h => h.TrabajadorCode == trabajadorCode))
        {
            return;
        }

        var detalle = resultado.Observaciones.Count > 0
            ? " " + string.Join(" ", resultado.Observaciones)
            : string.Empty;

        throw new InvalidOperationException(
            $"No estás autorizado para aprobar este documento ({documento.TipoTransaccion} " +
            $"{documento.PlantaCode}/{documento.Code}, monto {documento.MonedaCode} {documento.Monto:N2}).{detalle}");
    }

    public async Task<ResultadoHabilitados> ObtenerHabilitadosAsync(DocumentoAprobable documento, CancellationToken ct)
    {
        await CargarMatrizAsync(ct);

        var habilitados = new List<AprobadorHabilitado>();
        var observaciones = new List<string>();

        var tipo = AprobacionTipos.Normalizar(documento.TipoTransaccion);
        var moneda = documento.MonedaCode.Trim().ToUpperInvariant();

        var unidadEfectiva = ResolverUnidadEfectiva(tipo, documento.UnidadNegocioCode, moneda);

        if (unidadEfectiva is null)
        {
            observaciones.Add(
                $"No hay parametrización de aprobación para {tipo} en {moneda} " +
                $"(unidad de negocio {documento.UnidadNegocioCode} ni sus superiores).");
            return new ResultadoHabilitados(habilitados, observaciones);
        }

        var filas = _aprobadores!
            .Where(a => a.TipoTransaccionCode == tipo
                     && a.UnidadNegocioCode == unidadEfectiva
                     && a.MonedaCode == moneda)
            .ToList();

        if (filas.Count == 0)
        {
            observaciones.Add($"No hay aprobadores asignados para {tipo} en la unidad {unidadEfectiva}.");
            return new ResultadoHabilitados(habilitados, observaciones);
        }

        foreach (var fila in filas)
        {
            var rango = _rangos!.FirstOrDefault(r =>
                r.NivelCode == fila.NivelCode
                && r.TipoTransaccionCode == tipo
                && r.UnidadNegocioCode == unidadEfectiva
                && r.MonedaCode == moneda);

            if (rango is null)
            {
                continue;
            }

            var motivo = await MotivoExcedidoAsync(fila.TrabajadorCode, tipo, rango, documento, ct);

            if (motivo is null)
            {
                habilitados.Add(new AprobadorHabilitado(fila.NivelCode, "TITULAR", fila.TrabajadorCode));

                if (!string.IsNullOrWhiteSpace(fila.SuplenteTrabajadorCode))
                {
                    habilitados.Add(new AprobadorHabilitado(fila.NivelCode, "SUPLENTE", fila.SuplenteTrabajadorCode!));
                }
            }
            else
            {
                observaciones.Add($"Nivel {fila.NivelCode} (titular {fila.TrabajadorCode}): {motivo}");

                if (!string.IsNullOrWhiteSpace(fila.SuperiorTrabajadorCode))
                {
                    habilitados.Add(new AprobadorHabilitado(fila.NivelCode, "SUPERIOR", fila.SuperiorTrabajadorCode!));
                }
            }
        }

        return new ResultadoHabilitados(habilitados, observaciones);
    }

    // ------------------------------------------------------------------ límites

    /// <summary>Null si el titular cubre el documento; si no, el límite que excede.</summary>
    private async Task<string?> MotivoExcedidoAsync(
        string titularCode,
        string tipo,
        RangoAprobacion rango,
        DocumentoAprobable documento,
        CancellationToken ct)
    {
        if (documento.Monto > rango.ImporteMaximo)
        {
            return $"el monto {documento.Monto:N2} excede el importe máximo {rango.ImporteMaximo:N2}.";
        }

        var ahora = DateTime.Now;
        var montos = await ObtenerAcumuladosAsync(titularCode, tipo, ct);

        decimal dia = 0m;
        decimal mes = 0m;

        foreach (var m in montos)
        {
            var enMonedaRango = await ConvertirAsync(m.Monto, m.MonedaCode, rango.MonedaCode, m.Fecha, ct);

            mes += enMonedaRango;

            if (m.Fecha.Date == ahora.Date)
            {
                dia += enMonedaRango;
            }
        }

        if (dia + documento.Monto > rango.ImporteAcumuladoDiario)
        {
            return $"supera el acumulado diario ({dia:N2} + {documento.Monto:N2} > {rango.ImporteAcumuladoDiario:N2}).";
        }

        if (mes + documento.Monto > rango.ImporteAcumuladoMensual)
        {
            return $"supera el acumulado mensual ({mes:N2} + {documento.Monto:N2} > {rango.ImporteAcumuladoMensual:N2}).";
        }

        return null;
    }

    // ------------------------------------------------------------------ acumulados (por FechaAprobacion)

    private async Task<List<MontoAprobado>> ObtenerAcumuladosAsync(string trabajadorCode, string tipo, CancellationToken ct)
    {
        var clave = trabajadorCode + "|" + tipo;

        if (_acumulados.TryGetValue(clave, out var cache))
        {
            return cache;
        }

        // AprobadoPor guarda siempre el Login del usuario (Pedido, OC, OS y Vale). Un trabajador puede tener
        // más de un usuario, por eso se buscan todos sus logins.
        var nombres = await _uow.Administracion.Users
            .Query()
            .AsNoTracking()
            .Where(u => u.TrabajadorCode == trabajadorCode)
            .Select(u => u.Login)
            .ToListAsync(ct);

        var resultado = new List<MontoAprobado>();

        if (nombres.Count > 0)
        {
            var ahora = DateTime.Now;
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1);
            var T = _uow.Logistica.Transacciones;

            switch (tipo)
            {
                case AprobacionTipos.Pedido:
                    resultado = (await T.Pedidos.Query().AsNoTracking()
                            .Where(x => x.AprobadoPor != null
                                     && nombres.Contains(x.AprobadoPor)
                                     && x.FechaAprobacion >= inicioMes
                                     && x.EstadoPedido != EstadoPedido.Anulado)
                            .Select(x => new { x.FechaAprobacion, x.TotalPedido })
                            .ToListAsync(ct))
                        .Select(x => new MontoAprobado(x.FechaAprobacion!.Value, x.TotalPedido, AprobacionTipos.MonedaSoles))
                        .ToList();
                    break;

                case AprobacionTipos.OrdenCompra:
                    resultado = (await T.OrdenesCompra.Query().AsNoTracking()
                            .Where(x => x.AprobadoPor != null
                                     && nombres.Contains(x.AprobadoPor)
                                     && x.FechaAprobacion >= inicioMes
                                     && x.Estado != EstadoOrdenCompra.Anulado)
                            .Select(x => new { x.FechaAprobacion, x.TotalCompra, x.MonedaCode })
                            .ToListAsync(ct))
                        .Select(x => new MontoAprobado(x.FechaAprobacion!.Value, x.TotalCompra, x.MonedaCode))
                        .ToList();
                    break;

                case AprobacionTipos.OrdenServicio:
                    resultado = (await T.OrdenesServicio.Query().AsNoTracking()
                            .Where(x => x.AprobadoPor != null
                                     && nombres.Contains(x.AprobadoPor)
                                     && x.FechaAprobacion >= inicioMes
                                     && x.Estado != EstadoOrdenServicio.Anulado)
                            .Select(x => new { x.FechaAprobacion, x.Monto, x.MonedaCode })
                            .ToListAsync(ct))
                        .Select(x => new MontoAprobado(x.FechaAprobacion!.Value, x.Monto, x.MonedaCode))
                        .ToList();
                    break;

                case AprobacionTipos.Vale:
                case AprobacionTipos.ValeCliente:
                    var esCliente = tipo == AprobacionTipos.ValeCliente;
                    resultado = (await T.Vales.Query().AsNoTracking()
                            .Where(x => x.AprobadoPor != null
                                     && nombres.Contains(x.AprobadoPor)
                                     && x.FechaAprobacion >= inicioMes
                                     && x.Estado != EstadoVale.Anulado
                                     && x.TipoValeCode != TipoValeTransferencia
                                     && (esCliente
                                         ? x.TipoValeCode == TipoValeCombustible
                                         : x.TipoValeCode != TipoValeCombustible))
                            .Select(x => new { x.FechaAprobacion, x.Neto })
                            .ToListAsync(ct))
                        .Select(x => new MontoAprobado(x.FechaAprobacion!.Value, x.Neto, AprobacionTipos.MonedaSoles))
                        .ToList();
                    break;
            }
        }

        _acumulados[clave] = resultado;
        return resultado;
    }

    // ------------------------------------------------------------------ moneda

    private async Task<decimal> ConvertirAsync(decimal monto, string desde, string hacia, DateTime fecha, CancellationToken ct)
    {
        if (string.Equals(desde, hacia, StringComparison.OrdinalIgnoreCase))
        {
            return monto;
        }

        var enSoles = string.Equals(desde, AprobacionTipos.MonedaSoles, StringComparison.OrdinalIgnoreCase)
            ? monto
            : monto * await ObtenerVentaAsync(desde, fecha, ct);

        return string.Equals(hacia, AprobacionTipos.MonedaSoles, StringComparison.OrdinalIgnoreCase)
            ? enSoles
            : enSoles / await ObtenerVentaAsync(hacia, fecha, ct);
    }

    /// <summary>Tipo de cambio venta (SellRate) vigente a la fecha: el último registrado hasta ese día.</summary>
    private async Task<decimal> ObtenerVentaAsync(string monedaCode, DateTime fecha, CancellationToken ct)
    {
        var dia = DateOnly.FromDateTime(fecha);
        var clave = monedaCode.ToUpperInvariant() + "|" + dia.ToString("yyyyMMdd");

        if (_tiposCambio.TryGetValue(clave, out var cache))
        {
            return cache;
        }

        var tc = await _uow.Comunes.TiposCambio
            .Query()
            .AsNoTracking()
            .Where(t => t.MonedaCode == monedaCode && t.Date <= dia)
            .OrderByDescending(t => t.Date)
            .Select(t => (decimal?)t.SellRate)
            .FirstOrDefaultAsync(ct);

        if (tc is null || tc <= 0)
        {
            throw new InvalidOperationException(
                $"No hay tipo de cambio (venta) registrado para {monedaCode} a la fecha {dia:dd/MM/yyyy}; " +
                "es necesario para validar los acumulados de aprobación.");
        }

        _tiposCambio[clave] = tc.Value;
        return tc.Value;
    }

    // ------------------------------------------------------------------ matriz

    private async Task CargarMatrizAsync(CancellationToken ct)
    {
        if (_rangos is not null)
        {
            return;
        }

        _rangos = await _uow.Logistica.Catalogos.RangosAprobacion
            .Query().AsNoTracking().Where(r => r.IsActive).ToListAsync(ct);

        _aprobadores = await _uow.Logistica.Catalogos.AprobadoresAsignados
            .Query().AsNoTracking().Where(a => a.IsActive).ToListAsync(ct);

        _padres = await _uow.Logistica.Catalogos.UnidadesNegocio
            .Query().AsNoTracking()
            .Select(u => new { u.Code, u.ParentCode })
            .ToDictionaryAsync(u => u.Code, u => u.ParentCode, ct);
    }

    /// <summary>
    /// Sube por UnidadNegocio.ParentCode (máx. 20 niveles, igual que ResolverRangoAprobacion) hasta la primera
    /// unidad que tenga rangos para (tipo, moneda).
    /// </summary>
    private string? ResolverUnidadEfectiva(string tipo, string unidadCode, string moneda)
    {
        var actual = unidadCode;

        for (var i = 0; i < 20 && actual is not null; i++)
        {
            var codigo = actual;

            if (_rangos!.Any(r => r.TipoTransaccionCode == tipo
                               && r.UnidadNegocioCode == codigo
                               && r.MonedaCode == moneda))
            {
                return codigo;
            }

            actual = _padres!.TryGetValue(codigo, out var padre) ? padre : null;
        }

        return null;
    }
}
