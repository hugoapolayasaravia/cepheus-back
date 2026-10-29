using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.UpdateNivelEducativo
{
    public class UpdateNivelEducativoCommandHandler : IRequestHandler<UpdateNivelEducativoCommand, NivelEducativoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNivelEducativoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelEducativoResponse> Handle(UpdateNivelEducativoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nivel educativo {request.Code} no encontrado.");
            }

            var nivelEducativo = new NivelEducativo
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.NivelesEducativos.Update(nivelEducativo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El nivel educativo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NivelEducativoResponse
            {
                Code = nivelEducativo.Code,
                Name = nivelEducativo.Name,
                IsActive = nivelEducativo.IsActive,
                CreatedAt = nivelEducativo.CreatedAt,
                UpdatedAt = nivelEducativo.UpdatedAt,
                RowVersion = nivelEducativo.RowVersion
            };
        }
    }
}