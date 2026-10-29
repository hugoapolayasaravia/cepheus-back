using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.CreateTipoCentroFormacion
{
    public class CreateTipoCentroFormacionCommandHandler
        : IRequestHandler<CreateTipoCentroFormacionCommand, TipoCentroFormacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCentroFormacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCentroFormacionResponse> Handle(CreateTipoCentroFormacionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposCentroFormacion.Query().Select(t => t.Code), length: 3, entityLabel: "TiposCentroFormacion", cancellationToken);

            var tipoCentroFormacion = new TipoCentroFormacion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposCentroFormacion.AddAsync(tipoCentroFormacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoCentroFormacion);
        }

        internal static TipoCentroFormacionResponse Map(TipoCentroFormacion tipoCentroFormacion) => new()
        {
            Code = tipoCentroFormacion.Code,
            Name = tipoCentroFormacion.Name,
            IsActive = tipoCentroFormacion.IsActive,
            CreatedAt = tipoCentroFormacion.CreatedAt,
            UpdatedAt = tipoCentroFormacion.UpdatedAt,
            RowVersion = tipoCentroFormacion.RowVersion
        };
    }
}