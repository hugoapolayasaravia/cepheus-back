using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.UpdateOficina
{
    public class UpdateOficinaCommandHandler : IRequestHandler<UpdateOficinaCommand, OficinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOficinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OficinaResponse> Handle(UpdateOficinaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Oficinas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Oficina {request.Code} no encontrada.");
            }

            var oficina = new Oficina
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Oficinas.Update(oficina);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La oficina fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new OficinaResponse
            {
                Code = oficina.Code,
                Name = oficina.Name,
                IsActive = oficina.IsActive,
                CreatedAt = oficina.CreatedAt,
                UpdatedAt = oficina.UpdatedAt,
                RowVersion = oficina.RowVersion
            };
        }
    }
}