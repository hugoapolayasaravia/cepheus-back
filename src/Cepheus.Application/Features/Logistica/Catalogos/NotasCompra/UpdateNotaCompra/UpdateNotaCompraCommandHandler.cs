using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common.Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.UpdateNotaCompra
{
    public class UpdateNotaCompraCommandHandler : IRequestHandler<UpdateNotaCompraCommand, NotaCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNotaCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCompraResponse> Handle(UpdateNotaCompraCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.NotasCompra.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nota de compra {request.Code} no encontrada.");
            }

            var nota = new NotaCompra
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.NotasCompra.Update(nota);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de compra fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NotaCompraResponse
            {
                Code = nota.Code,
                Name = nota.Name,
                IsActive = nota.IsActive,
                CreatedAt = nota.CreatedAt,
                UpdatedAt = nota.UpdatedAt,
                CreatedBy = nota.CreatedBy,
                UpdatedBy = nota.UpdatedBy,
                RowVersion = nota.RowVersion
            };
        }
    }
}