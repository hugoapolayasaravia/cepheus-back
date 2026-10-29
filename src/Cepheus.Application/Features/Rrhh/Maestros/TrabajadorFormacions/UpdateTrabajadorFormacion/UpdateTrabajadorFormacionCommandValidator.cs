using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.UpdateTrabajadorFormacion
{
    public class UpdateTrabajadorFormacionCommandValidator
        : AbstractValidator<UpdateTrabajadorFormacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorFormacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El id de la formación es obligatorio.");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.NivelEducativoCode)
                .MustAsync(NivelEducativoExists)
                .WithMessage("El nivel educativo indicado no existe.");

            RuleFor(x => x.GradoInstruccionCode)
                .MustAsync(GradoInstruccionExists)
                .WithMessage("El grado de instrucción indicado no existe.");

            RuleFor(x => x.TituloCode)
                .MustAsync(TituloExists)
                .WithMessage("El título indicado no existe.");

            RuleFor(x => x.EspecialidadCode)
                .MustAsync(EspecialidadExists)
                .WithMessage("La especialidad indicada no existe.");

            RuleFor(x => x.TipoCentroFormacionCode)
                .MustAsync(TipoCentroFormacionExists)
                .WithMessage("El tipo de centro de formación indicado no existe.");

            RuleFor(x => x.ModalidadFormativaCode)
                .MustAsync(ModalidadFormativaExists)
                .WithMessage("La modalidad formativa indicada no existe.");
        }

        private async Task<bool> TrabajadorExists(
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.Trabajadores
                .Query()
                .AnyAsync(
                    x => x.Code == trabajadorCode.Trim(),
                    cancellationToken);
        }

        private async Task<bool> NivelEducativoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.NivelesEducativos
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> GradoInstruccionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.GradosInstruccion
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> TituloExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Titulos
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> EspecialidadExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.EspecialidadesTrabajador
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> TipoCentroFormacionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposCentroFormacion
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> ModalidadFormativaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.ModalidadesFormativas
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
