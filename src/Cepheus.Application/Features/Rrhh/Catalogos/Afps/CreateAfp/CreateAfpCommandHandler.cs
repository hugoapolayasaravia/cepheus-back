using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.CreateAfp
{
    public class CreateAfpCommandHandler : IRequestHandler<CreateAfpCommand, AfpResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAfpCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AfpResponse> Handle(CreateAfpCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Afps.Query().Select(a => a.Code), length: 3, entityLabel: "Afps", cancellationToken);

            var afp = new Afp
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Afps.AddAsync(afp, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(afp);
        }

        internal static AfpResponse Map(Afp afp) => new()
        {
            Code = afp.Code,
            Name = afp.Name,
            IsActive = afp.IsActive,
            CreatedAt = afp.CreatedAt,
            UpdatedAt = afp.UpdatedAt,
            RowVersion = afp.RowVersion
        };
    }
}