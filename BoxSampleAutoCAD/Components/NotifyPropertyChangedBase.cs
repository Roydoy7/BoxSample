using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BoxSampleAutoCAD.Components
{
    public class NotifyPropertyChangedBase : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyAllPropertyChanged()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                NotifyPropertyChanged(propertyInfo.Name);
            }
        }
    }
}
