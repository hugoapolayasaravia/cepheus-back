using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.CreateUbigeo
{
    public class CreateUbigeoCommandHandler : IRequestHandler<CreateUbigeoCommand, UbigeoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUbigeoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UbigeoResponse> Handle(CreateUbigeoCommand request, CancellationToken cancellationToken)
        {
            var ubigeo = new Ubigeo
            {
                Code = request.Code.Trim(),
                Department = request.Department.Trim(),
                Province = request.Province.Trim(),
                District = request.District.Trim(),
                IsActive = true
            };

            await _uow.Comunes.Ubigeos.AddAsync(ubigeo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(ubigeo);
        }

        internal static UbigeoResponse Map(Ubigeo ubigeo) => new()
        {
            Code = ubigeo.Code,
            Department = ubigeo.Department,
            Province = ubigeo.Province,
            District = ubigeo.District,
            FullAddress = ubigeo.FullAddress,
            IsActive = ubigeo.IsActive,
            CreatedAt = ubigeo.CreatedAt,
            UpdatedAt = ubigeo.UpdatedAt,
            CreatedBy = ubigeo.CreatedBy,
            UpdatedBy = ubigeo.UpdatedBy,
            RowVersion = ubigeo.RowVersion
        };
    }
}
