using System;
using System.Globalization;
using System.Windows;

namespace BoxSampleAutoCAD.Components
{
    public class ViewModelBase2 : NotifyPropertyChangedBase
    {
        public FrameworkElement View { get; set; }

        public ViewModelBase2()
        {
            InitView();
        }

        private void InitView()
        {
            string text = GetType().Name.Replace("`1", "");
            string arg = text.Replace("ViewModel", "View");
            string arg2 = text.Replace("ViewModel", "");
            string text2 = GetType().FullName;
            if (text2.Contains("`1"))
            {
                int length = text2.IndexOf("`1");
                text2 = text2.Substring(0, length);
            }

            string arg3 = text2.Replace(text, "").Replace(".ViewModels.", ".Views.");
            string fullName = GetType().Assembly.FullName;
            string typeName = string.Format(CultureInfo.InvariantCulture, "{0}{1},{2}", arg3, arg, fullName);
            string typeName2 = string.Format(CultureInfo.InvariantCulture, "{0}{1},{2}", arg3, arg2, fullName);
            Type type = Type.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(FrameworkElement)))
                {
                    View = Activator.CreateInstance(type) as FrameworkElement;
                    View.DataContext = this;
                }

                return;
            }

            Type type2 = Type.GetType(typeName2, throwOnError: false, ignoreCase: false);
            if (type2 != null && type2.IsSubclassOf(typeof(FrameworkElement)))
            {
                View = Activator.CreateInstance(type2) as FrameworkElement;
                View.DataContext = this;
            }
        }

        public void ShowDialog()
        {
            if (View != null)
            {
                (View as Window)?.ShowDialog();
            }
        }

        public void Show()
        {
            if (View != null)
            {
                (View as Window)?.Show();
            }
        }

        public void Hide()
        {
            if (View != null)
            {
                (View as Window)?.Hide();
            }
        }

        public void Close()
        {
            if (View != null)
            {
                (View as Window)?.Close();
            }
        }
    }
}
