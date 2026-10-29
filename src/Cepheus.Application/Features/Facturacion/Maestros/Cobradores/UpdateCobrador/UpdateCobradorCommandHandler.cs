using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.UpdateCobrador
{
    public class UpdateCobradorCommandHandler : IRequestHandler<UpdateCobradorCommand, CobradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCobradorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobradorResponse> Handle(UpdateCobradorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Maestros.Cobradores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cobrador {request.Code} no encontrado.");
            }

            var entity = new Cobrador
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                UserId = request.UserId,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Cobradores.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateCobrador.CreateCobradorCommandHandler.Map(entity);
        }
    }
}
