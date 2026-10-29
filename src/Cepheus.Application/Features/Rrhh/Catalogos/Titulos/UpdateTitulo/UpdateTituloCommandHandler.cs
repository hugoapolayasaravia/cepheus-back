using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.UpdateTitulo
{
    public class UpdateTituloCommandHandler : IRequestHandler<UpdateTituloCommand, TituloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTituloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TituloResponse> Handle(UpdateTituloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Titulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Título {request.Code} no encontrado.");
            }

            var titulo = new Titulo
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Titulos.Update(titulo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El título fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TituloResponse
            {
                Code = titulo.Code,
                Name = titulo.Name,
                IsActive = titulo.IsActive,
                CreatedAt = titulo.CreatedAt,
                UpdatedAt = titulo.UpdatedAt,
                RowVersion = titulo.RowVersion
            };
        }
    }
}