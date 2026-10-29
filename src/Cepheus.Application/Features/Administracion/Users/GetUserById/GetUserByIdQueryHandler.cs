using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUserByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.Administracion.Users.Query()
                .AsNoTracking()
                .Where(u => u.Id == request.Id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    TrabajadorCode = u.TrabajadorCode,
                    RowVersion = u.RowVersion,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                throw new KeyNotFoundException($"Usuario {request.Id} no encontrado.");
            }

            // Datos del trabajador (RRHH): solo si el usuario tiene código de trabajador.
            if (!string.IsNullOrWhiteSpace(user.TrabajadorCode))
            {
                var codigo = user.TrabajadorCode.Trim();

                var trabajador = await _uow.Rrhh.Maestros.Trabajadores.Query()
                    .AsNoTracking()
                    .Where(t => t.Code == codigo)
                    .Select(t => new { t.FirstNames, t.PaternalSurname, t.MaternalSurname })
                    .FirstOrDefaultAsync(cancellationToken);

                if (trabajador is not null)
                {
                    user.TrabajadorNombre = string.Join(" ", new[] { trabajador.FirstNames, trabajador.PaternalSurname, trabajador.MaternalSurname }
                        .Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

                    // Documento principal; si no hay, el primero registrado.
                    var documento = await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                        .AsNoTracking()
                        .Where(d => d.TrabajadorCode == codigo)
                        .OrderByDescending(d => d.IsPrimary)
                        .ThenBy(d => d.DocumentNumber)
                        .Select(d => new { Tipo = d.TipoDocumento.ShortName ?? d.TipoDocumento.Name, d.DocumentNumber })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (documento is not null)
                    {
                        user.TrabajadorTipoDocumento = documento.Tipo;
                        user.TrabajadorNumeroDocumento = documento.DocumentNumber;
                    }
                }
            }

            return user;
        }
    }




}
