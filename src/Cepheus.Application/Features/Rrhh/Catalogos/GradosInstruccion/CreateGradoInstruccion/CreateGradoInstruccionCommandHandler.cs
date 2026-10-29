using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.CreateGradoInstruccion
{
    public class CreateGradoInstruccionCommandHandler : IRequestHandler<CreateGradoInstruccionCommand, GradoInstruccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateGradoInstruccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GradoInstruccionResponse> Handle(CreateGradoInstruccionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.GradosInstruccion.Query().Select(g => g.Code), length: 3, entityLabel: "GradosInstruccion", cancellationToken);

            var gradoInstruccion = new GradoInstruccion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.GradosInstruccion.AddAsync(gradoInstruccion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(gradoInstruccion);
        }

        internal static GradoInstruccionResponse Map(GradoInstruccion gradoInstruccion) => new()
        {
            Code = gradoInstruccion.Code,
            Name = gradoInstruccion.Name,
            IsActive = gradoInstruccion.IsActive,
            CreatedAt = gradoInstruccion.CreatedAt,
            UpdatedAt = gradoInstruccion.UpdatedAt,
            RowVersion = gradoInstruccion.RowVersion
        };
    }
}