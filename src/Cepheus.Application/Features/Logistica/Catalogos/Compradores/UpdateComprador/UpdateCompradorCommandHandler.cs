using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.UpdateComprador
{
    public class UpdateCompradorCommandHandler : IRequestHandler<UpdateCompradorCommand, CompradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCompradorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CompradorResponse> Handle(UpdateCompradorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.Compradores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Comprador {request.Code} no encontrado.");
            }

            var comprador = new Comprador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.Compradores.Update(comprador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El comprador fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new CompradorResponse
            {
                Code = comprador.Code,
                Name = comprador.Name,
                IsActive = comprador.IsActive,
                CreatedAt = comprador.CreatedAt,
                UpdatedAt = comprador.UpdatedAt,
                CreatedBy = comprador.CreatedBy,
                UpdatedBy = comprador.UpdatedBy,
                RowVersion = comprador.RowVersion
            };
        }
    }
}