using System;

namespace Idento.Domain.Models
{
    internal interface ITenantChild
    {
        Guid TenantId { get; set; }
    }
}
