using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MessengerGui.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerGui;

public partial class CreateChatWindow : Window 
{
    public string ChatTitle => string.IsNullOrWhiteSpace(TitleInput.Text) ? "Новий чат" : TitleInput.Text;

    // Властивість, яка шукає всі відмічені чекбокси всередині ListBox
    public List<User> SelectedUsers 
    {
        get 
        {
            var selected = new List<User>();
            // Отримуємо всі CheckBox, які знаходяться всередині ListBox
            var checkBoxes = UserSelector.GetVisualDescendants().OfType<CheckBox>();
            
            foreach (var cb in checkBoxes) 
            {
                if (cb.IsChecked == true && cb.Tag is User user) 
                {
                    selected.Add(user);
                }
            }
            return selected;
        }
    }

    public CreateChatWindow() 
    { 
        InitializeComponent(); 
    }

    public CreateChatWindow(List<User> users) : this() 
    {
        UserSelector.ItemsSource = users;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e) 
    {
        // Перевіряємо, чи обрано хоча б одного учасника (крім себе)
        if (SelectedUsers.Count > 0) 
        {
            Close(true);
        }
    }
}