using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Maestros;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common
{
    /// <summary>Datos de un aval (legacy MClientes_Responsables_Letras). Los usan el alta y la modificación.</summary>
    public class ClienteResponsableLetraInputValidator<T> : AbstractValidator<T> where T : IClienteResponsableLetraInput
    {
        public ClienteResponsableLetraInputValidator()
        {
            RuleFor(x => x.FirstNames).NotEmpty().WithMessage("El nombre es obligatorio.").MaximumLength(100);
            RuleFor(x => x.PaternalSurname).NotEmpty().WithMessage("El apellido paterno es obligatorio.").MaximumLength(60);
            RuleFor(x => x.MaternalSurname).MaximumLength(60);
            RuleFor(x => x.DocumentTypeCode).MaximumLength(5);
            RuleFor(x => x.DocumentNumber).MaximumLength(20);
            RuleFor(x => x.Address).MaximumLength(150);
            RuleFor(x => x.Phone).MaximumLength(20);
        }
    }

    public interface IClienteResponsableLetraInput
    {
        string? PaternalSurname { get; }
        string? MaternalSurname { get; }
        string? FirstNames { get; }
        string? DocumentTypeCode { get; }
        string? DocumentNumber { get; }
        string? Address { get; }
        string? Phone { get; }
    }

    public static class ClienteResponsableLetraRules
    {
        public static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        public static void Apply(ClienteResponsableLetra a, IClienteResponsableLetraInput i)
        {
            a.PaternalSurname = Clean(i.PaternalSurname)?.ToUpperInvariant();
            a.MaternalSurname = Clean(i.MaternalSurname)?.ToUpperInvariant();
            a.FirstNames = Clean(i.FirstNames)?.ToUpperInvariant();
            a.DocumentTypeCode = Clean(i.DocumentTypeCode);
            a.DocumentNumber = Clean(i.DocumentNumber);
            a.Address = Clean(i.Address);
            a.Phone = Clean(i.Phone);
        }

        public static ClienteResponsableLetraResponse ToResponse(ClienteResponsableLetra a) => new()
        {
            ClienteCode = a.ClienteCode, Sequence = a.Sequence,
            PaternalSurname = a.PaternalSurname, MaternalSurname = a.MaternalSurname, FirstNames = a.FirstNames,
            DocumentTypeCode = a.DocumentTypeCode, DocumentNumber = a.DocumentNumber,
            Address = a.Address, Phone = a.Phone
        };

        public static async Task<ClienteResponsableLetra> GetTrackedAsync(
            IUnitOfWork uow, string clienteCode, string sequence, CancellationToken ct)
            => await uow.Facturacion.Maestros.ClientesResponsablesLetra.Query()
                   .FirstOrDefaultAsync(a => a.ClienteCode == clienteCode && a.Sequence == sequence, ct)
               ?? throw new KeyNotFoundException($"El cliente {clienteCode} no tiene el aval {sequence}.");
    }
}
