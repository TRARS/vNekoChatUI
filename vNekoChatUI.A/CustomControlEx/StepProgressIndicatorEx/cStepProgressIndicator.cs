using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.A.CustomControlEx.StepProgressIndicatorEx
{
    public partial class cStepProgressIndicator : Control
    {
        static cStepProgressIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cStepProgressIndicator), new FrameworkPropertyMetadata(typeof(cStepProgressIndicator)));
        }
    }

    public partial class cStepProgressIndicator
    {
        public int MaxStep
        {
            get { return (int)GetValue(MaxStepProperty); }
            set { SetValue(MaxStepProperty, value); }
        }
        public static readonly DependencyProperty MaxStepProperty = DependencyProperty.Register(
            name: "MaxStep",
            propertyType: typeof(int),
            ownerType: typeof(cStepProgressIndicator),
            typeMetadata: new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                var control = (cStepProgressIndicator)d;
                var newValue = e.NewValue as int? ?? 0;
                control.ProgressGrayItems = new(Enumerable.Range(0, newValue));
            })
        );

        public int CurrentStep
        {
            get { return (int)GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }
        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
            name: "CurrentStep",
            propertyType: typeof(int),
            ownerType: typeof(cStepProgressIndicator),
            typeMetadata: new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                var control = (cStepProgressIndicator)d;
                var newValue = e.NewValue as int? ?? 0;
                control.ProgressItems = new(Enumerable.Range(0, newValue));
            })
        );


        public ObservableCollection<int> ProgressGrayItems
        {
            get { return (ObservableCollection<int>)GetValue(ProgressGrayItemsProperty); }
            set { SetValue(ProgressGrayItemsProperty, value); }
        }
        public static readonly DependencyProperty ProgressGrayItemsProperty = DependencyProperty.Register(
            name: "ProgressGrayItems",
            propertyType: typeof(ObservableCollection<int>),
            ownerType: typeof(cStepProgressIndicator),
            typeMetadata: new FrameworkPropertyMetadata(new ObservableCollection<int>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public ObservableCollection<int> ProgressItems
        {
            get { return (ObservableCollection<int>)GetValue(ProgressItemsProperty); }
            set { SetValue(ProgressItemsProperty, value); }
        }
        public static readonly DependencyProperty ProgressItemsProperty = DependencyProperty.Register(
            name: "ProgressItems",
            propertyType: typeof(ObservableCollection<int>),
            ownerType: typeof(cStepProgressIndicator),
            typeMetadata: new FrameworkPropertyMetadata(new ObservableCollection<int>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
