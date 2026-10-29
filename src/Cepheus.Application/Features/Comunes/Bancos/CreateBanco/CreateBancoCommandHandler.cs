using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Bancos.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.CreateBanco
{
    public class CreateBancoCommandHandler : IRequestHandler<CreateBancoCommand, BancoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateBancoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BancoResponse> Handle(CreateBancoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Comunes.Bancos.Query().Select(b => b.Code), length: 3, entityLabel: "Bancos", cancellationToken);

            var banco = new Banco
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Comunes.Bancos.AddAsync(banco, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(banco);
        }

        internal static BancoResponse Map(Banco banco) => new()
        {
            Code = banco.Code,
            Name = banco.Name,
            IsActive = banco.IsActive,
            CreatedAt = banco.CreatedAt,
            UpdatedAt = banco.UpdatedAt,
            CreatedBy = banco.CreatedBy,
            UpdatedBy = banco.UpdatedBy,
            RowVersion = banco.RowVersion
        };
    }
}
