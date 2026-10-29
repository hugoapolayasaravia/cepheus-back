using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;


namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.CreateEps
{
    public class CreateEpsCommandHandler : IRequestHandler<CreateEpsCommand, EpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEpsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EpsResponse> Handle(CreateEpsCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Epss.Query().Select(e => e.Code), length: 3, entityLabel: "Eps", cancellationToken);

            var eps = new Eps
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Epss.AddAsync(eps, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(eps);
        }

        internal static EpsResponse Map(Eps eps) => new()
        {
            Code = eps.Code,
            Name = eps.Name,
            IsActive = eps.IsActive,
            CreatedAt = eps.CreatedAt,
            UpdatedAt = eps.UpdatedAt,
            RowVersion = eps.RowVersion
        };
    }
}