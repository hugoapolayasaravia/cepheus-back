using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.CreateNivelEducativo
{
    public class CreateNivelEducativoCommandHandler : IRequestHandler<CreateNivelEducativoCommand, NivelEducativoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNivelEducativoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelEducativoResponse> Handle(CreateNivelEducativoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.NivelesEducativos.Query().Select(n => n.Code), length: 3, entityLabel: "NivelesEducativos", cancellationToken);

            var nivelEducativo = new NivelEducativo
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.NivelesEducativos.AddAsync(nivelEducativo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nivelEducativo);
        }

        internal static NivelEducativoResponse Map(NivelEducativo nivelEducativo) => new()
        {
            Code = nivelEducativo.Code,
            Name = nivelEducativo.Name,
            IsActive = nivelEducativo.IsActive,
            CreatedAt = nivelEducativo.CreatedAt,
            UpdatedAt = nivelEducativo.UpdatedAt,
            RowVersion = nivelEducativo.RowVersion
        };
    }
}