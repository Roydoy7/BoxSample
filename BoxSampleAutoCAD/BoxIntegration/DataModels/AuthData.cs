using System;

namespace BoxSampleAutoCAD.BoxIntegration.DataModels
{
    public class AuthData : IAuthData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Expires_in { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(AccessToken))
                return false;
            return DateTime.UtcNow < CreatedTime.Add(TimeSpan.FromSeconds(Expires_in));
        }
        public bool IsRefreshValid()
        {
            if (string.IsNullOrEmpty(RefreshToken))
                return false;
            return DateTime.UtcNow < CreatedTime.Add(TimeSpan.FromDays(60));
        }
    }
}
