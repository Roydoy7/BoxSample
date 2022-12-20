using BoxSampleAutoCAD.BoxIntegration.DataModels;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration
{
    public interface IAuthProvider
    {
        Task<IAuthData> AuthAsync(string code);
        Task<IAuthData> RefreshAsync(string refreshToken);
        void Unauth();
    }
}