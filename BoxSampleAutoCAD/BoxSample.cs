using Autodesk.AutoCAD.Runtime;
using BoxSampleAutoCAD.BoxIntegration;

namespace BoxSampleAutoCAD
{
    public class BoxSample : IExtensionApplication
    {
        public void Initialize()
        {
            BoxModule.Init();
        }

        public void Terminate()
        {
        }
    }
}
