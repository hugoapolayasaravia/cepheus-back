using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.UpdateTipoAnexo
{
    public class UpdateTipoAnexoCommandHandler : IRequestHandler<UpdateTipoAnexoCommand, TipoAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAnexoResponse> Handle(UpdateTipoAnexoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Catalogos.TiposAnexo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de anexo {request.Code} no encontrado.");
            }

            var entity = new TipoAnexo
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Flag = string.IsNullOrWhiteSpace(request.Flag) ? null : request.Flag.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Catalogos.TiposAnexo.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoAnexo.CreateTipoAnexoCommandHandler.Map(entity);
        }
    }
}
