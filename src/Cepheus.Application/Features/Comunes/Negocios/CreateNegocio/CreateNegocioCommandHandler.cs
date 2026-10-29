using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Negocios.Common;
using Cepheus.Domain.Comun;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.CreateNegocio
{
    public class CreateNegocioCommandHandler : IRequestHandler<CreateNegocioCommand, NegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNegocioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NegocioResponse> Handle(CreateNegocioCommand request, CancellationToken cancellationToken)
        {
            var negocio = new Negocio
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Comunes.Negocios.AddAsync(negocio, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(negocio);
        }

        internal static NegocioResponse Map(Negocio negocio) => new()
        {
            Code = negocio.Code,
            Name = negocio.Name,
            IsActive = negocio.IsActive,
            CreatedAt = negocio.CreatedAt,
            UpdatedAt = negocio.UpdatedAt,
            CreatedBy = negocio.CreatedBy,
            UpdatedBy = negocio.UpdatedBy,
            RowVersion = negocio.RowVersion
        };
    }
}
