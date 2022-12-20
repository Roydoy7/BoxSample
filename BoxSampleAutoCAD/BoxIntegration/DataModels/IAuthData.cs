using System;

namespace BoxSampleAutoCAD.BoxIntegration.DataModels
{
    public interface IAuthData
    {
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
        DateTime CreatedTime { get; set; }
        int Expires_in { get; set; }

        bool IsValid();
        bool IsRefreshValid();
    }
}