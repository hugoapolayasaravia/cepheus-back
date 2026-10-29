using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.CreateTitulo
{
    public class CreateTituloCommandHandler : IRequestHandler<CreateTituloCommand, TituloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTituloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TituloResponse> Handle(CreateTituloCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Titulos.Query().Select(t => t.Code), length: 3, entityLabel: "Titulos", cancellationToken);

            var titulo = new Titulo
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Titulos.AddAsync(titulo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(titulo);
        }

        internal static TituloResponse Map(Titulo titulo) => new()
        {
            Code = titulo.Code,
            Name = titulo.Name,
            IsActive = titulo.IsActive,
            CreatedAt = titulo.CreatedAt,
            UpdatedAt = titulo.UpdatedAt,
            RowVersion = titulo.RowVersion
        };
    }
}