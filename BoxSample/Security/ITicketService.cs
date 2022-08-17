using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace BoxSample.Security
{
    public interface ITicketService
    {
        Task<AuthenticationTicket> ValidateToken(string token);
    }
}