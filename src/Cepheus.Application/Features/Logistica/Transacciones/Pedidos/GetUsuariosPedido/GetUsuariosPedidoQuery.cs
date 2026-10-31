using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetUsuariosPedido
{
    /// <summary>Etiqueta = el mismo texto que la auditoría guarda en CreatedBy ("Nombre Apellido (email)").</summary>
    public record UsuarioPedidoResponse(int Id, string Email, string FullName, string Label);

    public record GetUsuariosPedidoQuery(string? Search) : IRequest<List<UsuarioPedidoResponse>>;

    public class GetUsuariosPedidoQueryHandler
        : IRequestHandler<GetUsuariosPedidoQuery, List<UsuarioPedidoResponse>>
    {
        private readonly IUnitOfWork _uow;
        public GetUsuariosPedidoQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<List<UsuarioPedidoResponse>> Handle(GetUsuariosPedidoQuery request, CancellationToken ct)
        {
            var q = _uow.Administracion.Users.Query().AsNoTracking().Where(u => u.IsActive);
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var t = request.Search.Trim();
                q = q.Where(u => u.Email.Contains(t) || u.FirstName.Contains(t) || u.LastName.Contains(t));
            }
            var rows = await q.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).Take(200)
                .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName }).ToListAsync(ct);
            return rows.Select(u =>
            {
                var full = (u.FirstName + " " + u.LastName).Trim();
                return new UsuarioPedidoResponse(u.Id, u.Email, full, $"{full} ({u.Email})");
            }).ToList();
        }
    }
}
