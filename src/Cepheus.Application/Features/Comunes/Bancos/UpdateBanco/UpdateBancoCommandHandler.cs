using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Bancos.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.UpdateBanco
{
    public class UpdateBancoCommandHandler : IRequestHandler<UpdateBancoCommand, BancoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateBancoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BancoResponse> Handle(UpdateBancoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.Bancos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Banco {request.Code} no encontrado.");
            }

            var banco = new Banco
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.Bancos.Update(banco);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El banco fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new BancoResponse
            {
                Code = banco.Code,
                Name = banco.Name,
                IsActive = banco.IsActive,
                CreatedAt = banco.CreatedAt,
                UpdatedAt = banco.UpdatedAt,
                CreatedBy = banco.CreatedBy,
                UpdatedBy = banco.UpdatedBy,
                RowVersion = banco.RowVersion
            };
        }
    }
}
