using PropertyChanged;

namespace BoxSampleAutoCAD.BoxIntegration.DataModels
{
    [AddINotifyPropertyChangedInterface]
    public class BoxUserModel : IBoxUserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
