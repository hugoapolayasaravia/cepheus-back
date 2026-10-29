namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common
{
    public interface IArticuloAnexoDatos
    {
        string SubCentroCostoCode { get; }
        string ArticuloCode { get; }
        string AccountingAccountCode { get; }
        string? AccountingAttachmentTypeCode { get; }
        string? AccountingAttachmentCode { get; }
    }
}
