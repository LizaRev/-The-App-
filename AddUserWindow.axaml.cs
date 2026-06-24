using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MessengerGui;

public partial class AddUserWindow : Window
{
    // Це властивість, яку ми будемо читати з MainWindow
    public string NewUserName => NameInput.Text ?? "";

    public AddUserWindow()
    {
        InitializeComponent();
    }

    public void Add_Click(object sender, RoutedEventArgs e)
    {
        // Закриваємо вікно і повертаємо "true", що означає успіх
        Close(true);
    }
}