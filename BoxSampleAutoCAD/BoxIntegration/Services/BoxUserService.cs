using BoxSampleAutoCAD.BoxIntegration.Components;
using BoxSampleAutoCAD.BoxIntegration.DataModels;
using System;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public class BoxUserService
    {
        private static BoxUserService mInstance;
        public static BoxUserService Instance
        {
            get
            {
                mInstance ??= new();
                return mInstance;
            }
        }

        public event EventHandler<BoxUserEventArgs> BoxUserInformationAcquiredEvent;
        private BoxUserService()
        {
            BoxAuthProvider.Instance.BoxTokenExpiredEvent += (o, e) =>
            {
                OnBoxAccessTokenExpired();
            };
            BoxAuthProvider.Instance.BoxUnauthEvent += (o, e) =>
            {
                OnBoxAccessTokenExpired();
            };
            BoxAuthProvider.Instance.BoxAuthedEvent += (o, e) =>
            {
                OnBoxUserInformationRequest();
            };
        }

        private void OnBoxAccessTokenExpired()
        {
            var boxUserModel = BoxModule.BoxUserModel;
            boxUserModel.Email = string.Empty;
            boxUserModel.UserName = string.Empty;
        }

        private async void OnBoxUserInformationRequest()
        {
            var userProvider = BoxUserProvider.Instance;
            var curUser = await userProvider.GetCurrentUser();
            if (curUser == null)
                return;

            var boxUserModel = BoxModule.BoxUserModel;
            boxUserModel.UserName = curUser.Name;
            boxUserModel.Email = curUser.Email;
            BoxUserInformationAcquiredEvent?.Invoke(this, new BoxUserEventArgs { BoxUserModel = boxUserModel });
        }

        public class BoxUserEventArgs : EventArgs
        {
            public IBoxUserModel BoxUserModel { get; set; }
        }
    }
}
