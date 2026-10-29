using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.UpdateAnexo
{
    public class UpdateAnexoCommandHandler : IRequestHandler<UpdateAnexoCommand, AnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnexoResponse> Handle(UpdateAnexoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Maestros.Anexos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TipoAnexoCode == request.TipoAnexoCode && x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Anexo {request.TipoAnexoCode}/{request.Code} no encontrado.");
            }

            var entity = new Anexo
            {
                TipoAnexoCode = request.TipoAnexoCode,
                Code = request.Code,
                Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
                LinkAccount1Code = (request.LinkAccount1Code?.Trim() ?? string.Empty),
                LinkAccount2Code = (request.LinkAccount2Code?.Trim() ?? string.Empty),
                Flag = string.IsNullOrWhiteSpace(request.Flag) ? null : request.Flag.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Maestros.Anexos.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateAnexo.CreateAnexoCommandHandler.Map(entity);
        }
    }
}
