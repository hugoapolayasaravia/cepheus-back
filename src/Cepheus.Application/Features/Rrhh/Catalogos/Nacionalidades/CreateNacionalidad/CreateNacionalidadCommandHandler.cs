using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.CreateNacionalidad
{
    public class CreateNacionalidadCommandHandler : IRequestHandler<CreateNacionalidadCommand, NacionalidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNacionalidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NacionalidadResponse> Handle(CreateNacionalidadCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Nacionalidades.Query().Select(n => n.Code), length: 3, entityLabel: "Nacionalidades", cancellationToken);

            var nacionalidad = new Nacionalidad
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Nacionalidades.AddAsync(nacionalidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nacionalidad);
        }

        internal static NacionalidadResponse Map(Nacionalidad nacionalidad) => new()
        {
            Code = nacionalidad.Code,
            Name = nacionalidad.Name,
            IsActive = nacionalidad.IsActive,
            CreatedAt = nacionalidad.CreatedAt,
            UpdatedAt = nacionalidad.UpdatedAt,
            RowVersion = nacionalidad.RowVersion
        };
    }
}