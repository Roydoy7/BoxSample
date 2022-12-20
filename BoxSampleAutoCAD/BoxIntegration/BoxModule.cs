using BoxSampleAutoCAD.BoxIntegration.DataModels;
using BoxSampleAutoCAD.BoxIntegration.Services;
using System.Net;

namespace BoxSampleAutoCAD.BoxIntegration
{
    public class BoxModule
    {
        /// <summary>
        /// Box token is saved here.
        /// </summary>
        public static IAuthData AuthData { get; set; }

        /// <summary>
        /// Box login credential is saved here.
        /// </summary>
        public static IBoxAuthCredential BoxAuthCredential { get; set; }

        /// <summary>
        /// Logged in user's information is saved here.
        /// </summary>
        public static IBoxUserModel BoxUserModel { get; set; }

        public static void Init()
        {
            //Init data models
            AuthData = new AuthData(); ;
            BoxAuthCredential = new BoxAuthCredential();
            BoxUserModel = new BoxUserModel();

            //Init token service
            var boxTokenService = BoxTokenService.Instance;
            //Init user service
            var boxUserService = BoxUserService.Instance;

            //This is used to prevent SSL/TLS exception
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

    }
}
