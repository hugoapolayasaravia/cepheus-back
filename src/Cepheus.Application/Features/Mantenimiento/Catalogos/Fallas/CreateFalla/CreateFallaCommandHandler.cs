using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.CreateFalla
{
    public class CreateFallaCommandHandler
        : IRequestHandler<CreateFallaCommand, FallaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateFallaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FallaResponse> Handle(
            CreateFallaCommand request,
            CancellationToken cancellationToken)
        {
            var falla = new Falla
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Fallas.AddAsync(falla, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(falla);
        }

        internal static FallaResponse Map(Falla falla) => new()
        {
            Code = falla.Code,
            Name = falla.Name,
            IsActive = falla.IsActive,
            CreatedAt = falla.CreatedAt,
            UpdatedAt = falla.UpdatedAt,
            CreatedBy = falla.CreatedBy,
            UpdatedBy = falla.UpdatedBy,
            RowVersion = falla.RowVersion
        };
    }
}
