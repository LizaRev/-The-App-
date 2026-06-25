using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MessengerGui.Models;
using MessengerGui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Primitives;
using System.IO;
using Avalonia.Media.Imaging;

namespace MessengerGui;

public class StringToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string path && File.Exists(path))
        {
            try { return new Bitmap(path); }
            catch { return null; }
        }
        return null;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public static class BoolToAlignmentConverter {
    public static FuncValueConverter<bool, HorizontalAlignment> Instance = new(b => b ? HorizontalAlignment.Right : HorizontalAlignment.Left);
}

public static class MessageColorConverter {
    private static readonly IBrush LightPinkBrush = Brush.Parse("#ffe6f0");
    private static readonly IBrush WhiteBrush = Brushes.White;

    public static FuncValueConverter<MessageViewModel, IBrush> Instance = new(m => {
        if (m == null) return WhiteBrush;
        return m.IsCurrentUser ? LightPinkBrush : WhiteBrush;
    });
}

public partial class MainWindow : Window
{
    private SqliteRepository _repo = new SqliteRepository();
    private User? _currentUser;
    private Conversation? _activeChat;

    public MainWindow() { InitializeComponent(); UserListBox.ItemsSource = _repo.GetUsers(); }

    public async void Attach_File_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png" } });
        var result = await dialog.ShowAsync(this);
        
        if (result != null && result.Length > 0 && _activeChat != null && _currentUser != null) 
        {
            string sourcePath = result[0];
            string uploadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            Directory.CreateDirectory(uploadDir);
            
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
            string destPath = Path.Combine(uploadDir, fileName);
            File.Copy(sourcePath, destPath, true);

            _repo.AddMessage(new Message { 
                ConversationId = _activeChat.Id, 
                SenderId = _currentUser.Id, 
                Text = "", 
                FilePath = destPath, 
                Timestamp = DateTime.Now 
            });
            RefreshMessages();
        }
    }

    public void Insert_Emoji(object sender, RoutedEventArgs e) {
        if (sender is Button btn && btn.Content is string emoji) { 
            MessageInput.Text += emoji; 
            var flyout = btn.FindAncestorOfType<FlyoutPresenter>();
            if (flyout?.Parent is Popup host) host.IsOpen = false;
        }
    }

    public void RefreshMessages() {
        if (_activeChat == null || _currentUser == null) return;
        
        var messages = _repo.GetMessagesForConversation(_activeChat.Id);
        var viewModels = new List<MessageViewModel>();
        DateTime lastDate = DateTime.MinValue;

        foreach (var m in messages) {
            var vm = new MessageViewModel {
                Id = m.Id, 
                Text = m.Text, 
                Timestamp = m.Timestamp,
                SenderName = _repo.GetUsers().FirstOrDefault(u => u.Id == m.SenderId)?.Username ?? "Unknown",
                IsCurrentUser = (m.SenderId == _currentUser.Id),
                FilePath = m.FilePath
            };

            if (m.Timestamp.Date != lastDate.Date) {
                vm.ShowDateHeader = true;
                lastDate = m.Timestamp;
            } else {
                vm.ShowDateHeader = false;
            }
            viewModels.Add(vm);
        }

        MessageList.ItemsSource = viewModels;
        if (viewModels.Count > 0) Dispatcher.UIThread.Post(() => MessageList.ScrollIntoView(viewModels.Last()));
    }

    public void Search_TextChanged(object sender, TextChangedEventArgs e) {
        string s = SearchInput.Text?.ToLower() ?? "";
        bool show = !string.IsNullOrWhiteSpace(s);
        SearchResults.IsVisible = show;
        if (show && _activeChat != null) SearchResults.ItemsSource = _repo.GetMessagesForConversation(_activeChat.Id)
            .Where(m => m.Text.ToLower().Contains(s)).Select(m => new MessageViewModel { 
                Id = m.Id, Text = m.Text, Timestamp = m.Timestamp,
                SenderName = _repo.GetUsers().FirstOrDefault(u => u.Id == m.SenderId)?.Username ?? "Unknown"
            }).ToList();
    }

    public void Delete_User_Click(object sender, RoutedEventArgs e) {
        if (sender is MenuItem item && item.DataContext is User user) {
            _repo.DeleteUser(user.Id); UserListBox.ItemsSource = _repo.GetUsers();
            ChatList.ItemsSource = null; _activeChat = null;
        }
    }

    public void ChatList_KeyDown(object sender, KeyEventArgs e) {
        if ((e.Key == Key.Delete || e.Key == Key.Back) && ChatList.SelectedItem is Conversation chat) {
            _repo.DeleteConversation(chat.Id); ChatList.ItemsSource = _repo.GetConversationsForUser(_currentUser!.Id);
            MessageList.ItemsSource = null; _activeChat = null;
        }
    }

    public void MessageList_KeyDown(object sender, KeyEventArgs e) {
        if ((e.Key == Key.Delete || e.Key == Key.Back) && MessageList.SelectedItem is MessageViewModel msg) {
            _repo.DeleteMessage(msg.Id); RefreshMessages();
        }
    }

    public void Delete_Chat_Click(object sender, RoutedEventArgs e) {
        if (sender is MenuItem item && item.DataContext is Conversation chat) {
            _repo.DeleteConversation(chat.Id); ChatList.ItemsSource = _repo.GetConversationsForUser(_currentUser!.Id);
            MessageList.ItemsSource = null; _activeChat = null;
        }
    }

    public void User_Selected(object sender, SelectionChangedEventArgs e) {
        if (UserListBox.SelectedItem is User user) { _currentUser = user; ChatList.ItemsSource = _repo.GetConversationsForUser(user.Id); }
    }

    public void Chat_Selected(object sender, SelectionChangedEventArgs e) {
        if (ChatList.SelectedItem is Conversation chat) { _activeChat = chat; RefreshMessages(); }
    }

    public void MessageInput_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) Send_Message(sender, e);
    }

    public void Send_Message(object sender, RoutedEventArgs e) {
        if (_activeChat != null && _currentUser != null && !string.IsNullOrWhiteSpace(MessageInput.Text)) {
            _repo.AddMessage(new Message { ConversationId = _activeChat.Id, SenderId = _currentUser.Id, Text = MessageInput.Text, Timestamp = DateTime.Now });
            MessageInput.Text = ""; 
            RefreshMessages();
        }
    }

    public async void Open_AddUser_Dialog(object sender, RoutedEventArgs e) {
        var dialog = new AddUserWindow();
        if (await dialog.ShowDialog<bool>(this)) { _repo.AddUser(new User { Username = dialog.NewUserName }); UserListBox.ItemsSource = _repo.GetUsers(); }
    }

    public async void Create_Chat_Click(object sender, RoutedEventArgs e) {
        if (_currentUser == null) return;
        var dialog = new CreateChatWindow(_repo.GetUsers().Where(u => u.Id != _currentUser.Id).ToList());
        if (await dialog.ShowDialog<bool>(this)) {
            var newConv = new Conversation { Title = dialog.ChatTitle };
            newConv.ParticipantIds.Add(_currentUser.Id);
            foreach (var user in dialog.SelectedUsers) newConv.ParticipantIds.Add(user.Id);
            _repo.AddConversation(newConv); ChatList.ItemsSource = _repo.GetConversationsForUser(_currentUser.Id);
        }
    }

    public void Result_Selected(object sender, SelectionChangedEventArgs e) {
        if (SearchResults.SelectedItem is MessageViewModel m) {
            var target = MessageList.Items.Cast<MessageViewModel>().FirstOrDefault(x => x.Id == m.Id);
            if (target != null) { MessageList.SelectedItem = target; MessageList.ScrollIntoView(target); }
            SearchResults.IsVisible = false;
        }
    }
}