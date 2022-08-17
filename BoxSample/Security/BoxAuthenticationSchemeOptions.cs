using Microsoft.AspNetCore.Authentication;

namespace BoxSample.Security
{
    public class BoxAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string Name = "BoxAuthenticationScheme";
    }
}
