using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.CreateCategoriaTrabajador
{
    public class CreateCategoriaTrabajadorCommandHandler
        : IRequestHandler<CreateCategoriaTrabajadorCommand, CategoriaTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCategoriaTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaTrabajadorResponse> Handle(CreateCategoriaTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.CategoriasTrabajador.Query().Select(c => c.Code), length: 3, entityLabel: "CategoriasTrabajador", cancellationToken);

            var categoriaTrabajador = new CategoriaTrabajador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.CategoriasTrabajador.AddAsync(categoriaTrabajador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(categoriaTrabajador);
        }

        internal static CategoriaTrabajadorResponse Map(CategoriaTrabajador categoriaTrabajador) => new()
        {
            Code = categoriaTrabajador.Code,
            Name = categoriaTrabajador.Name,
            IsActive = categoriaTrabajador.IsActive,
            CreatedAt = categoriaTrabajador.CreatedAt,
            UpdatedAt = categoriaTrabajador.UpdatedAt,
            RowVersion = categoriaTrabajador.RowVersion
        };
    }
}