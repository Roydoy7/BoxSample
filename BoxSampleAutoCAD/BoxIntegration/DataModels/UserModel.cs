using PropertyChanged;

namespace BoxSampleAutoCAD.BoxIntegration.DataModels
{
    [AddINotifyPropertyChangedInterface]
    public class UserModel : IUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Avatar_Url { get; set; }
    }
}
