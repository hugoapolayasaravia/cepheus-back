using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.CreateModalidadFormativa
{
    public class CreateModalidadFormativaCommandHandler
        : IRequestHandler<CreateModalidadFormativaCommand, ModalidadFormativaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateModalidadFormativaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModalidadFormativaResponse> Handle(CreateModalidadFormativaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.ModalidadesFormativas.Query().Select(m => m.Code), length: 3, entityLabel: "ModalidadesFormativas", cancellationToken);

            var modalidadFormativa = new ModalidadFormativa
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.ModalidadesFormativas.AddAsync(modalidadFormativa, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(modalidadFormativa);
        }

        internal static ModalidadFormativaResponse Map(ModalidadFormativa modalidadFormativa) => new()
        {
            Code = modalidadFormativa.Code,
            Name = modalidadFormativa.Name,
            IsActive = modalidadFormativa.IsActive,
            CreatedAt = modalidadFormativa.CreatedAt,
            UpdatedAt = modalidadFormativa.UpdatedAt,
            RowVersion = modalidadFormativa.RowVersion
        };
    }
}