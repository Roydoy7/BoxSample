using System.Runtime.InteropServices;
using System;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Interop;

namespace BoxSampleAutoCAD.Components
{
    public static class ViewModelBase2Ex
    {
        public static void ShowDialogByAcad(this ViewModelBase2 viewModel)
        {
            if (viewModel.View is Window window)
            {
                window.SetOwner(Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle);
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModalWindow(window);
            }
        }

        public static void ShowByAcad(this ViewModelBase2 viewModel)
        {
            if (viewModel.View is Window window)
            {
                window.SetOwner(Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle);
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModelessWindow(window);
            }
        }

        private static readonly int GWL_HWNDPARENT = -8;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetOwner(this Window window, IntPtr ownerHandle)
        {
            if (!(ownerHandle == IntPtr.Zero))
            {
                IntPtr handle = window.GetHandle();
                SetWindowLong(handle, GWL_HWNDPARENT, ownerHandle.ToInt32());
            }
        }

        public static IntPtr GetHandle(this Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            return windowInteropHelper.Handle;
        }
    }
}
