using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Dragablz;
using Dragablz.Dockablz;

namespace DragablzDemo
{
    public class TabablzControlProxy : INotifyPropertyChanged
    {
        private readonly TabablzControl _tabablzControl;
        private readonly ICommand _splitHorizontallyCommand;
        private readonly ICommand _splitVerticallyCommand;
        private double _splitRatio;

        public TabablzControlProxy(TabablzControl tabablzControl)
        {
            _tabablzControl = tabablzControl;

            _splitHorizontallyCommand = new AnotherCommandImplementation(_ => Branch(Orientation.Horizontal));
            _splitVerticallyCommand = new AnotherCommandImplementation(_ => Branch(Orientation.Vertical));
            SplitRatio = 5;
        }

        public ICommand SplitHorizontallyCommand
        {
            get { return _splitHorizontallyCommand; }
        }

        public ICommand SplitVerticallyCommand
        {
            get { return _splitVerticallyCommand; }
        }

        public double SplitRatio
        {
            get { return _splitRatio; }
            set
            {
                _splitRatio = value;
                OnPropertyChanged("SplitRatio");
            }
        }

        private void Branch(Orientation orientation)
        {
            var branchResult = Layout.Branch(_tabablzControl, GetNew(_tabablzControl), orientation, false, SplitRatio/10);

            var newItem = new HeaderedItemViewModel
            {
                Header = "Layout Info",
                Content = new LayoutManagementExample { DataContext = new LayoutManagementExampleViewModel() }
            };

            branchResult.TabablzControl.AddToSource(newItem);
            branchResult.TabablzControl.SelectedItem = newItem;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public TabablzControl GetNew(TabablzControl source)
        {
            var tabablzControl = new TabablzControl { DataContext = source.DataContext };

            Clone(source, tabablzControl);

            var newInterTabController = new InterTabController
            {
                Partition = source.InterTabController.Partition
            };
            Clone(source.InterTabController, newInterTabController);
            tabablzControl.SetCurrentValue(TabablzControl.InterTabControllerProperty, newInterTabController);
            return tabablzControl;
        }

        private static void Clone(DependencyObject from, DependencyObject to)
        {
            var localValueEnumerator = from.GetLocalValueEnumerator();
            while (localValueEnumerator.MoveNext())
            {
                if (localValueEnumerator.Current.Property.ReadOnly ||
                    localValueEnumerator.Current.Value is FrameworkElement) continue;

                if (!(localValueEnumerator.Current.Value is BindingExpressionBase))
                    to.SetCurrentValue(localValueEnumerator.Current.Property, localValueEnumerator.Current.Value);
            }
        }
    }
}