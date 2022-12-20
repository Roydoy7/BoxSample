using Autodesk.AutoCAD.Runtime;
using BoxSampleAutoCAD.BoxIntegration.ViewModels;
using BoxSampleAutoCAD.Components;

[assembly: CommandClass(typeof(BoxSampleAutoCAD.BoxIntegration.Commands.BoxIntegrationCommands))]
namespace BoxSampleAutoCAD.BoxIntegration.Commands
{
    public partial class BoxIntegrationCommands
    {
        [CommandMethod("BoxLogin")]
        public void BoxLogin()
        {
            var vm = new BoxAuthFormViewModel();
            vm.ShowDialogByAcad();
        }

        [CommandMethod("BoxUpload")]
        public void BoxUpload()
        {
            var vm = new BoxUploadFormViewModel();
            vm.ShowDialogByAcad();
        }

        [CommandMethod("BoxDownload")]
        public void BoxDownload()
        {
            var vm = new BoxDownloadFormViewModel();
            vm.ShowDialogByAcad();
        }
    }
}
