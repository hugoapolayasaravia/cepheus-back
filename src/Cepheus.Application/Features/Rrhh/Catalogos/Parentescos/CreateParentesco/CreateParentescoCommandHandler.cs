using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.CreateParentesco
{
    public class CreateParentescoCommandHandler : IRequestHandler<CreateParentescoCommand, ParentescoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateParentescoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ParentescoResponse> Handle(CreateParentescoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Parentescos.Query().Select(p => p.Code), length: 3, entityLabel: "Parentescos", cancellationToken);

            var parentesco = new Parentesco
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Parentescos.AddAsync(parentesco, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(parentesco);
        }

        internal static ParentescoResponse Map(Parentesco parentesco) => new()
        {
            Code = parentesco.Code,
            Name = parentesco.Name,
            IsActive = parentesco.IsActive,
            CreatedAt = parentesco.CreatedAt,
            UpdatedAt = parentesco.UpdatedAt,
            RowVersion = parentesco.RowVersion
        };
    }
}