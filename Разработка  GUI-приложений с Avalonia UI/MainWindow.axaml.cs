using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Разработка__GUI_приложений_с_Avalonia_UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
    }

    public async void ShowMessage(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 350,
            Height = 250,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Command = new RelayCommand(() => messageBox.Close()),
            Padding = new Avalonia.Thickness(30, 10),
            Background = Avalonia.Media.Brushes.White,
            Foreground = Avalonia.Media.Brushes.Black,
            BorderThickness = new Avalonia.Thickness(1),
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(100, 100, 100)),
            CornerRadius = new Avalonia.CornerRadius(6)
        };

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            FontSize = 14,
            Margin = new Avalonia.Thickness(0, 0, 0, 20)
        };

        var stackPanel = new StackPanel
        {
            Margin = new Avalonia.Thickness(25),
            Spacing = 15,
            Children = { textBlock, okButton }
        };

        messageBox.Content = stackPanel;

        await messageBox.ShowDialog(this);
    }
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string? _clientName;
    private string? _clientPhone;
    private string _selectedPizza = "Маргарита";
    private bool _extraCheese;
    private bool _mushrooms;
    private bool _olives;
    private bool _jalapeno;
    private int _quantity = 1;
    private int _totalAmount;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public string? ClientName
    {
        get => _clientName;
        set { SetField(ref _clientName, value); CalculateTotal(); }
    }

    public string? ClientPhone
    {
        get => _clientPhone;
        set { SetField(ref _clientPhone, value); CalculateTotal(); }
    }

    public string SelectedPizza
    {
        get => _selectedPizza;
        set 
        { 
            if (SetField(ref _selectedPizza, value))
            {
                OnPropertyChanged(nameof(IsMargheritaSelected));
                OnPropertyChanged(nameof(IsPepperoniSelected));
                OnPropertyChanged(nameof(IsHawaiianSelected));
                OnPropertyChanged(nameof(IsFourCheeseSelected));
            }
            CalculateTotal();
        }
    }

    // Свойства для RadioButton
    public bool IsMargheritaSelected
    {
        get => _selectedPizza == "Маргарита";
        set { if (value) { SelectedPizza = "Маргарита"; } }
    }

    public bool IsPepperoniSelected
    {
        get => _selectedPizza == "Пепперони";
        set { if (value) { SelectedPizza = "Пепперони"; } }
    }

    public bool IsHawaiianSelected
    {
        get => _selectedPizza == "Гавайская";
        set { if (value) { SelectedPizza = "Гавайская"; } }
    }

    public bool IsFourCheeseSelected
    {
        get => _selectedPizza == "Четыре сыра";
        set { if (value) { SelectedPizza = "Четыре сыра"; } }
    }

    public bool ExtraCheese
    {
        get => _extraCheese;
        set { SetField(ref _extraCheese, value); CalculateTotal(); }
    }

    public bool Mushrooms
    {
        get => _mushrooms;
        set { SetField(ref _mushrooms, value); CalculateTotal(); }
    }

    public bool Olives
    {
        get => _olives;
        set { SetField(ref _olives, value); CalculateTotal(); }
    }

    public bool Jalapeno
    {
        get => _jalapeno;
        set { SetField(ref _jalapeno, value); CalculateTotal(); }
    }

    public int Quantity
    {
        get => _quantity;
        set { SetField(ref _quantity, value); CalculateTotal(); }
    }

    public int TotalAmount
    {
        get => _totalAmount;
        private set => SetField(ref _totalAmount, value);
    }

    public ICommand ConfirmOrderCommand { get; }

    public MainWindowViewModel(MainWindow mainWindow)
    {
        ConfirmOrderCommand = new RelayCommand(ConfirmOrder);
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        int basePrice = _selectedPizza switch
        {
            "Маргарита" => 450,
            "Пепперони" => 550,
            "Гавайская" => 500,
            "Четыре сыра" => 600,
            _ => 450
        };

        int extrasPrice = 0;
        if (_extraCheese) extrasPrice += 50;
        if (_mushrooms) extrasPrice += 70;
        if (_olives) extrasPrice += 60;
        if (_jalapeno) extrasPrice += 50;

        TotalAmount = (basePrice + extrasPrice) * _quantity;
    }

    private void ConfirmOrder()
    {
        string message = $"✅ Заказ подтверждён!\n\n" +
                        $"👤 Клиент: {_clientName ?? "Не указан"}\n" +
                        $"📞 Телефон: {_clientPhone ?? "Не указан"}\n" +
                        $"🍕 Пицца: {_selectedPizza}\n" +
                        $"🥗 Добавки: {GetExtrasText()}\n" +
                        $"🔢 Количество: {_quantity} шт.\n" +
                        $"💰 Итого: {TotalAmount} ₽";

        Dispatcher.UIThread.Post(() =>
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                ((MainWindow)lifetime.MainWindow!).ShowMessage("Заказ подтверждён", message);
            }
        });
    }

    private string GetExtrasText()
    {
        var extras = new List<string>();
        if (_extraCheese) extras.Add("Сыр");
        if (_mushrooms) extras.Add("Грибы");
        if (_olives) extras.Add("Оливки");
        if (_jalapeno) extras.Add("Халапеньо");
        return extras.Count > 0 ? string.Join(", ", extras) : "Нет";
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => _execute();

#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
}
