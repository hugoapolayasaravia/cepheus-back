using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.CreateUsuarioPlanta
{
    public class CreateUsuarioPlantaCommandHandler
        : IRequestHandler<CreateUsuarioPlantaCommand, UsuarioPlantaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUsuarioPlantaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UsuarioPlantaResponse> Handle(
            CreateUsuarioPlantaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            var entity = new UsuarioPlanta
            {
                UserId = request.UserId,
                PlantaCode = plantaCode
            };

            await _uow.Logistica.Catalogos.UsuariosPlanta.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            // Releer con proyección para devolver también los datos de usuario y planta.
            return await _uow.Logistica.Catalogos.UsuariosPlanta.Query()
                .AsNoTracking()
                .Where(a => a.UserId == request.UserId && a.PlantaCode == plantaCode)
                .Select(a => new UsuarioPlantaResponse
                {
                    UserId = a.UserId,
                    UserEmail = a.User.Email,
                    UserFullName = a.User.FirstName + " " + a.User.LastName,
                    PlantaCode = a.PlantaCode,
                    PlantaName = a.Planta.Name,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    CreatedBy = a.CreatedBy,
                    UpdatedBy = a.UpdatedBy,
                    RowVersion = a.RowVersion
                })
                .FirstAsync(cancellationToken);
        }
    }
}
