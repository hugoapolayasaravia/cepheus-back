using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.UpdateModalidadFormativa
{
    public class UpdateModalidadFormativaCommandHandler
        : IRequestHandler<UpdateModalidadFormativaCommand, ModalidadFormativaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateModalidadFormativaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModalidadFormativaResponse> Handle(UpdateModalidadFormativaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.ModalidadesFormativas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Modalidad formativa {request.Code} no encontrada.");
            }

            var modalidadFormativa = new ModalidadFormativa
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.ModalidadesFormativas.Update(modalidadFormativa);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La modalidad formativa fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ModalidadFormativaResponse
            {
                Code = modalidadFormativa.Code,
                Name = modalidadFormativa.Name,
                IsActive = modalidadFormativa.IsActive,
                CreatedAt = modalidadFormativa.CreatedAt,
                UpdatedAt = modalidadFormativa.UpdatedAt,
                RowVersion = modalidadFormativa.RowVersion
            };
        }
    }
}