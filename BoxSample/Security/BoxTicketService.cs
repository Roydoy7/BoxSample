using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BoxSample.Security
{
    public class BoxTicketService : ITicketService
    {
        public async Task<AuthenticationTicket> ValidateToken(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("https://api.box.com/2.0/users/me");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var boxUser = JsonConvert.DeserializeObject<BoxUser>(await response.Content.ReadAsStringAsync());
                    return CreateAuthenticationTicket(boxUser.Id);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private AuthenticationTicket CreateAuthenticationTicket(string id)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id)
            };

            var claimsIdentity = new ClaimsIdentity(claims, nameof(BoxAuthenticationHandler));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authTicket = new AuthenticationTicket(claimsPrincipal, BoxAuthenticationSchemeOptions.Name);
            return authTicket;
        }

        class BoxUser
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Login { get; set; }
            public string Avatar_Url { get; set; }
        }
    }
}
