using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.CreateCentroCosto
{
    public class CreateCentroCostoCommandHandler : IRequestHandler<CreateCentroCostoCommand, CentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCentroCostoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroCostoResponse> Handle(CreateCentroCostoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Maestros.CentrosCosto.Query().Select(c => c.Code), length: 3, entityLabel: "Centros de Costo", cancellationToken);

            var centro = new CentroCosto
            {
                Code = code,
                Name = request.Name.Trim(),
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.CentrosCosto.AddAsync(centro, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(centro);
        }

        internal static CentroCostoResponse Map(CentroCosto centro) => new()
        {
            Code = centro.Code,
            Name = centro.Name,
            PlantaCode = centro.PlantaCode,
            IsActive = centro.IsActive,
            CreatedAt = centro.CreatedAt,
            UpdatedAt = centro.UpdatedAt,
            RowVersion = centro.RowVersion
        };
    }
}
