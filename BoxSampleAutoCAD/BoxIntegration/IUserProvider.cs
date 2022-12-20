using BoxSampleAutoCAD.BoxIntegration.DataModels;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration
{
    public interface IUserProvider
    {
        Task<IUserModel> GetCurrentUser();
    }
}
