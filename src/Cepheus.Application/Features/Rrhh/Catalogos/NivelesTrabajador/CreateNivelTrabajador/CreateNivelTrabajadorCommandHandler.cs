using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.CreateNivelTrabajador
{
    public class CreateNivelTrabajadorCommandHandler
        : IRequestHandler<CreateNivelTrabajadorCommand, NivelTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNivelTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelTrabajadorResponse> Handle(CreateNivelTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.NivelesTrabajador.Query().Select(n => n.Code), length: 3, entityLabel: "NivelesTrabajador", cancellationToken);

            var nivelTrabajador = new NivelTrabajador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.NivelesTrabajador.AddAsync(nivelTrabajador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nivelTrabajador);
        }

        internal static NivelTrabajadorResponse Map(NivelTrabajador nivelTrabajador) => new()
        {
            Code = nivelTrabajador.Code,
            Name = nivelTrabajador.Name,
            IsActive = nivelTrabajador.IsActive,
            CreatedAt = nivelTrabajador.CreatedAt,
            UpdatedAt = nivelTrabajador.UpdatedAt,
            RowVersion = nivelTrabajador.RowVersion
        };
    }
}