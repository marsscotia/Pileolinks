using System.Windows.Input;

namespace Pileolinks.Components;

public partial class Chip : ContentView
{
    public static readonly BindableProperty ChipTextProperty =
        BindableProperty.Create(nameof(ChipText), typeof(string), typeof(Chip), propertyChanged: OnChipTextChanged);
    
    public static readonly BindableProperty ChipCommandProperty = 
        BindableProperty.Create(nameof(ChipCommand), typeof(ICommand), typeof(Chip));
    
    public static readonly BindableProperty ChipIconProperty = 
        BindableProperty.Create(nameof(ChipIcon), typeof(ImageSource), typeof(Chip), propertyChanged: OnChipIconChanged);

    public static readonly BindableProperty IsIconVisibleProperty =
        BindableProperty.Create(nameof(IsIconVisible), typeof(bool), typeof(Chip), propertyChanged: OnIsIconVisibleChanged);

    public static readonly BindableProperty ChipForegroundColorProperty =
        BindableProperty.Create(nameof(ChipForegroundColor), typeof(Color), typeof(Chip), propertyChanged: OnForegroundColorChanged);

    public static readonly BindableProperty ChipBackgroundColorProperty =
        BindableProperty.Create(nameof(ChipBackgroundColor), typeof(Color), typeof(Chip), propertyChanged: OnBackgroundColorChanged);

    public string ChipText 
    { 
        get => (string)GetValue(ChipTextProperty); 
        set => SetValue(ChipTextProperty, value); 
    }

    public ICommand ChipCommand
    {
        get => (ICommand)GetValue(ChipCommandProperty);
        set => SetValue(ChipCommandProperty, value);
    }

    public ImageSource ChipIcon
    {
        get => (ImageSource)GetValue(ChipIconProperty);
        set => SetValue(ChipIconProperty, value);
    }

    public bool IsIconVisible
    {
        get => (bool)GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    public Color ChipForegroundColor
    {
        get => (Color)GetValue(ChipForegroundColorProperty);
        set => SetValue(ChipForegroundColorProperty, value);
    }

    public Color ChipBackgroundColor
    {
        get => (Color)(GetValue(ChipBackgroundColorProperty));
        set => SetValue(ChipBackgroundColorProperty, value);
    }

    public Chip()
    {
        InitializeComponent();
    }

    static void OnChipTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chip = (Chip)bindable;
        chip.ChipTextLabel.Text = (string)newValue;
    }

    static void OnChipIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chip = (Chip)bindable;
        chip.ChipImageButton.Source = (ImageSource)newValue;
    }

    static void OnIsIconVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chip = (Chip)bindable;
        chip.ChipImageButton.IsVisible = (bool)newValue;
    }

    static void OnForegroundColorChanged(BindableObject bindableObject, object oldValue, object newValue)
    {
        var chip = (Chip)bindableObject;
        Color newColor = (Color)newValue;
        chip.ChipTextLabel.TextColor = newColor;
    }

    static void OnBackgroundColorChanged(BindableObject bindable,  object oldValue, object newValue)
    {
        var chip = (Chip)bindable;
        chip.ChipBorder.BackgroundColor = (Color)newValue;
    }
        
    private void ChipImageButton_Clicked(object sender, EventArgs e)
    {
        ChipCommand.Execute(ChipTextLabel.Text);
    }

}