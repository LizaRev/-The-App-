using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MessengerGui;

public partial class AddUserWindow : Window
{
    public string NewUserName => NameInput.Text ?? "";

    public AddUserWindow()
    {
        InitializeComponent();
    }

    public void Add_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}