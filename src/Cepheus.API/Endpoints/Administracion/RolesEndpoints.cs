using Cepheus.Application.Features.Administracion.Roles.ExportRoles;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Permissions.GetRolePermissions;
using Cepheus.Application.Features.Administracion.Roles.AddRolePermissions;
using Cepheus.Application.Features.Administracion.Roles.AssignRolePermissions;
using Cepheus.Application.Features.Administracion.Roles.CreateRole;
using Cepheus.Application.Features.Administracion.Roles.GetPermissionCatalog;
using Cepheus.Application.Features.Administracion.Roles.GetRoleById;
using Cepheus.Application.Features.Administracion.Roles.GetRolesPaginated;
using Cepheus.Application.Features.Administracion.Roles.ToggleRoleStatus;
using Cepheus.Application.Features.Administracion.Roles.UpdateRole;
using MediatR;

namespace Cepheus.API.Endpoints.Administracion
{
    public static class RolesEndpoints
    {
        public static void MapRolesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/roles")
                .WithTags("Roles")
                .RequireAuthorization();

            // POST /api/roles
            group.MapPost("/", async (CreateRoleCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/roles/{result.Id}", result);
            })
            .WithName("CreateRole")
            .RequireAuthorization("ROLES.CREATE");

            // GET /api/roles/paged
            group.MapGet("/paged", async (
                [AsParameters] GetRolesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRolesPagedQueryString")
            .RequireAuthorization("ROLES.VIEW");

            // POST /api/roles/paged/body
            group.MapPost("/paged/body", async (
                GetRolesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRolesPagedBody")
            .RequireAuthorization("ROLES.VIEW");

            // GET /api/roles/export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportRolesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportRolesExcel")
            .RequireAuthorization("ROLES.EXPORT_EXCEL");

            // GET /api/roles/export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportRolesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportRolesPdf")
            .RequireAuthorization("ROLES.EXPORT_PDF");

            // GET /api/roles/permission-catalog
            // Árbol Módulo -> Submódulo -> Programa -> Permiso (solo lo activo) para armar las casillas
            // de la pantalla de Roles en una sola llamada. Va ANTES de /{id:int} por claridad.
            group.MapGet("/permission-catalog", async (ISender sender) =>
            {
                var result = await sender.Send(new GetPermissionCatalogQuery());
                return Results.Ok(result);
            })
            .WithName("GetPermissionCatalog")
            .RequireAuthorization("ROLES.VIEW");

            // GET /api/roles/{id}
            group.MapGet("/{id:int}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetRoleByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetRoleById")
            .RequireAuthorization("ROLES.VIEW");

            // PUT /api/roles/{id}
            group.MapPut("/{id:int}", async (int id, UpdateRoleCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El Id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateRole")
            .RequireAuthorization("ROLES.UPDATE");

            // PATCH /api/roles/{id}/toggle-status
            group.MapPatch("/{id:int}/toggle-status", async (int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleRoleStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleRoleStatus")
            .RequireAuthorization("ROLES.UPDATE");

            // GET /api/roles/{id}/permissions
            group.MapGet("/{id:int}/permissions", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetRolePermissionsQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetRolePermissions")
            .RequireAuthorization("ROLES.VIEW");

            // PUT /api/roles/{id}/permissions
            group.MapPut("/{id:int}/permissions", async (
                int id, AssignRolePermissionsCommand command, ISender sender) =>
            {
                if (id != command.RoleId)
                {
                    return Results.BadRequest("El Id de la ruta no coincide con el roleId del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("AssignRolePermissions")
            .RequireAuthorization("ROLES.UPDATE");


            group.MapPost("/{id:int}/permissions", async (
                int id, AddRolePermissionsCommand command, ISender sender) =>
                {
                    if (id != command.RoleId)
                    {
                        return Results.BadRequest("El Id de la ruta no coincide con el roleId del cuerpo.");
                    }

                    var result = await sender.Send(command);
                    return Results.Ok(result);
                })
            .WithName("AddRolePermissions")
            .RequireAuthorization("ROLES.UPDATE");
        }
    }


}
