using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.UpdateSexo
{
    public class UpdateSexoCommandHandler : IRequestHandler<UpdateSexoCommand, SexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SexoResponse> Handle(UpdateSexoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Sexos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Sexo {request.Code} no encontrado.");
            }

            var sexo = new Sexo
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Sexos.Update(sexo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El sexo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SexoResponse
            {
                Code = sexo.Code,
                Name = sexo.Name,
                IsActive = sexo.IsActive,
                CreatedAt = sexo.CreatedAt,
                UpdatedAt = sexo.UpdatedAt,
                RowVersion = sexo.RowVersion
            };
        }
    }
}