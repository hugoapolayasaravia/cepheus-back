using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.UpdateLugarEnvio
{
    public class UpdateLugarEnvioCommandHandler : IRequestHandler<UpdateLugarEnvioCommand, LugarEnvioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateLugarEnvioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LugarEnvioResponse> Handle(UpdateLugarEnvioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.LugaresEnvio.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Lugar de envío {request.Code} no encontrado.");
            }

            var lugar = new LugarEnvio
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.LugaresEnvio.Update(lugar);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El lugar de envío fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new LugarEnvioResponse
            {
                Code = lugar.Code,
                Name = lugar.Name,
                Address = lugar.Address,
                IsActive = lugar.IsActive,
                CreatedAt = lugar.CreatedAt,
                UpdatedAt = lugar.UpdatedAt,
                CreatedBy = lugar.CreatedBy,
                UpdatedBy = lugar.UpdatedBy,
                RowVersion = lugar.RowVersion
            };
        }
    }
}