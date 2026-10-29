using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.UpdateCategoriaTrabajador
{
    public class UpdateCategoriaTrabajadorCommandHandler
        : IRequestHandler<UpdateCategoriaTrabajadorCommand, CategoriaTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCategoriaTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaTrabajadorResponse> Handle(UpdateCategoriaTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.CategoriasTrabajador.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Categoría de trabajador {request.Code} no encontrada.");
            }

            var categoriaTrabajador = new CategoriaTrabajador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.CategoriasTrabajador.Update(categoriaTrabajador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La categoría de trabajador fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new CategoriaTrabajadorResponse
            {
                Code = categoriaTrabajador.Code,
                Name = categoriaTrabajador.Name,
                IsActive = categoriaTrabajador.IsActive,
                CreatedAt = categoriaTrabajador.CreatedAt,
                UpdatedAt = categoriaTrabajador.UpdatedAt,
                RowVersion = categoriaTrabajador.RowVersion
            };
        }
    }
}