using System;

namespace Idento.Domain.Exceptions
{
    public class TenantIdDoesNotMatchContextException : ArgumentException
    {
        public TenantIdDoesNotMatchContextException(string paramName)
            : base("TenantId does not match context", paramName)
        { }
    }
}
