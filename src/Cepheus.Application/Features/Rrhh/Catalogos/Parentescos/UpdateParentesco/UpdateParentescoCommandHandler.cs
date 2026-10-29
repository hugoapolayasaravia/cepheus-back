using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.UpdateParentesco
{
    public class UpdateParentescoCommandHandler : IRequestHandler<UpdateParentescoCommand, ParentescoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateParentescoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ParentescoResponse> Handle(UpdateParentescoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Parentescos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Parentesco {request.Code} no encontrado.");
            }

            var parentesco = new Parentesco
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Parentescos.Update(parentesco);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El parentesco fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ParentescoResponse
            {
                Code = parentesco.Code,
                Name = parentesco.Name,
                IsActive = parentesco.IsActive,
                CreatedAt = parentesco.CreatedAt,
                UpdatedAt = parentesco.UpdatedAt,
                RowVersion = parentesco.RowVersion
            };
        }
    }
}