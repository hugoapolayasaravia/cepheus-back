using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.UpdateGradoInstruccion
{
    public class UpdateGradoInstruccionCommandHandler : IRequestHandler<UpdateGradoInstruccionCommand, GradoInstruccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateGradoInstruccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GradoInstruccionResponse> Handle(UpdateGradoInstruccionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.GradosInstruccion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Grado de instrucción {request.Code} no encontrado.");
            }

            var gradoInstruccion = new GradoInstruccion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.GradosInstruccion.Update(gradoInstruccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El grado de instrucción fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new GradoInstruccionResponse
            {
                Code = gradoInstruccion.Code,
                Name = gradoInstruccion.Name,
                IsActive = gradoInstruccion.IsActive,
                CreatedAt = gradoInstruccion.CreatedAt,
                UpdatedAt = gradoInstruccion.UpdatedAt,
                RowVersion = gradoInstruccion.RowVersion
            };
        }
    }
}