using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.CreateArea
{
    public class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, AreaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAreaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AreaResponse> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Areas.Query().Select(a => a.Code), length: 3, entityLabel: "Areas", cancellationToken);

            var area = new Area
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Areas.AddAsync(area, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(area);
        }

        internal static AreaResponse Map(Area area) => new()
        {
            Code = area.Code,
            Name = area.Name,
            IsActive = area.IsActive,
            CreatedAt = area.CreatedAt,
            UpdatedAt = area.UpdatedAt,
            RowVersion = area.RowVersion
        };
    }
}