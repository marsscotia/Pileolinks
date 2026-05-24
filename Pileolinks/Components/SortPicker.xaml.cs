using Pileolinks.ViewModels;

namespace Pileolinks.Components;

public partial class SortPicker : ContentView
{
    public static readonly BindableProperty SortBehaviorProperty =
        BindableProperty.Create(
            nameof(SortBehavior),
            typeof(object),
            typeof(SortPicker),
            null,
            propertyChanged: OnSortBehaviorChanged);

    public static readonly BindableProperty SortOptionsProperty =
        BindableProperty.Create(
            nameof(SortOptions),
            typeof(List<Sort>),
            typeof(SortPicker),
            new List<Sort>());

    public static readonly BindableProperty SelectedSortProperty =
        BindableProperty.Create(
            nameof(SelectedSort),
            typeof(Sort),
            typeof(SortPicker),
            null,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedSortChanged);

    public static readonly BindableProperty HasItemsProperty =
        BindableProperty.Create(
            nameof(HasItems),
            typeof(bool),
            typeof(SortPicker),
            false);

    public static readonly BindableProperty ShowLabelProperty =
        BindableProperty.Create(
            nameof(ShowLabel),
            typeof(bool),
            typeof(SortPicker),
            true);

    public object SortBehavior
    {
        get => GetValue(SortBehaviorProperty);
        set => SetValue(SortBehaviorProperty, value);
    }

    public List<Sort> SortOptions
    {
        get => (List<Sort>)GetValue(SortOptionsProperty);
        set => SetValue(SortOptionsProperty, value);
    }

    public Sort SelectedSort
    {
        get => (Sort)GetValue(SelectedSortProperty);
        set => SetValue(SelectedSortProperty, value);
    }

    public bool HasItems
    {
        get => (bool)GetValue(HasItemsProperty);
        set => SetValue(HasItemsProperty, value);
    }

    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    public SortPicker()
    {
        InitializeComponent();
    }

    private static void OnSortBehaviorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SortPicker)bindable;
        if (newValue != null)
        {
            // Bind to the SortableBehavior properties dynamically
            var behaviorType = newValue.GetType();

            // Get Sorts, SelectedSort, and HasItems properties from the behavior
            var sortsProperty = behaviorType.GetProperty("Sorts");
            var selectedSortProperty = behaviorType.GetProperty("SelectedSort");
            var hasItemsProperty = behaviorType.GetProperty("HasItems");

            if (sortsProperty != null)
            {
                var sorts = sortsProperty.GetValue(newValue) as List<Sort>;
                control.SortOptions = sorts;
            }

            if (selectedSortProperty != null)
            {
                control.SetBinding(SelectedSortProperty, 
                    new Binding(nameof(SelectedSort), BindingMode.TwoWay, source: newValue));
            }

            if (hasItemsProperty != null)
            {
                control.SetBinding(HasItemsProperty, 
                    new Binding(nameof(HasItems), BindingMode.OneWay, source: newValue));
            }
        }
    }

    private static void OnSelectedSortChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SortPicker)bindable;
        var behavior = control.SortBehavior;

        if (behavior != null && newValue != null)
        {
            // Execute the Sort command from the behavior
            var sortCommandProperty = behavior.GetType().GetProperty("SortCommand");
            if (sortCommandProperty != null)
            {
                var sortCommand = sortCommandProperty.GetValue(behavior);
                var executeMethod = sortCommand?.GetType().GetMethod("Execute", new Type[] { typeof(object) });
                executeMethod?.Invoke(sortCommand, new object[] { null });
            }
        }
    }
}
