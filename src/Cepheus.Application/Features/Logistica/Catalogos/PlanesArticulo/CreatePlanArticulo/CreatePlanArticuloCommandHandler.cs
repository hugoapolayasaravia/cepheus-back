using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.CreatePlanArticulo
{
    public class CreatePlanArticuloCommandHandler : IRequestHandler<CreatePlanArticuloCommand, PlanArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanArticuloResponse> Handle(CreatePlanArticuloCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.PlanesArticulo.Query().Select(p => p.Code), length: 3, entityLabel: "Planes de Artículo", cancellationToken);

            var plan = new PlanArticulo
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.PlanesArticulo.AddAsync(plan, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(plan);
        }

        internal static PlanArticuloResponse Map(PlanArticulo plan) => new()
        {
            Code = plan.Code,
            Name = plan.Name,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            CreatedBy = plan.CreatedBy,
            UpdatedBy = plan.UpdatedBy,
            RowVersion = plan.RowVersion
        };
    }
}