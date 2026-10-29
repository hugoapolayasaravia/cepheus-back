using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common
{
    namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common
    {
        public class NotaCompraResponse
        {
            public string Code { get; set; } = default!;
            public string Name { get; set; } = default!;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
            public byte[] RowVersion { get; set; } = default!;
        }
    }
}
