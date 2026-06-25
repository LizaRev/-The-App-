# 💬 Messenger GUI

![C#](https://img.shields.io/badge/C%23-.NET-purple)
![Avalonia](https://img.shields.io/badge/UI-Avalonia-blue)
![SQLite](https://img.shields.io/badge/Database-SQLite-green)
![EF Core](https://img.shields.io/badge/ORM-EntityFrameworkCore-orange)
![Version](https://img.shields.io/badge/Version-1.0-pink)

---

You can watch the Messenger video file named "chat.mov".


# 📌 About the Project

**Messenger GUI** is a desktop messenger application developed using **C#**, **Avalonia UI**, **SQLite**, and **Entity Framework Core**.

The application allows users to:

✅ Create users

✅ Create personal and group chats

✅ Send text messages

✅ Attach images

✅ Use emojis

✅ Delete chats

✅ Search messages

✅ Store all data in SQLite

---

# 🛠 Technologies

* C#
* .NET
* Avalonia UI
* SQLite
* Entity Framework Core
* LINQ
* XAML

---

# 🗄 Database

The project uses a local SQLite database.

Database file:

```text
messenger.db
```

Tables:

```text
Users
Conversations
Messages
```

The database is created automatically when the application starts:

```csharp
_db.Database.EnsureCreated();
```

---

# 👤 User Model

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
}
```

Fields:

| Field    | Description       |
| -------- | ----------------- |
| Id       | Unique identifier |
| Username | User name         |

---

# 💬 Conversation Model

```csharp
public class Conversation
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<int> ParticipantIds { get; set; }
}
```

Fields:

| Field          | Description          |
| -------------- | -------------------- |
| Id             | Conversation ID      |
| Title          | Conversation title   |
| ParticipantIds | List of participants |

---

# ✉️ Message Model

```csharp
public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public string? FilePath { get; set; }
}
```

Fields:

| Field          | Description     |
| -------------- | --------------- |
| Id             | Message ID      |
| ConversationId | Conversation ID |
| SenderId       | Sender ID       |
| Text           | Message text    |
| Timestamp      | Sending time    |
| FilePath       | File path       |

---

# 🚀 Features

## 👤 Creating a User

1. Click the button:

```text
+ Add User
```

2. Enter a user name.

3. Click:

```text
Add
```

---

## 💬 Creating a Chat

1. Select the active user.
2. Click:

```text
+ New Chat
```

3. Enter a chat title.
4. Select participants.
5. Click:

```text
Create
```

---

## 🗑 Deleting a Chat

Method 1

Right-click on a chat:

```text
Delete Chat
```

Method 2

Select a chat and press:

```text
Delete
```

---

## ✉️ Sending Messages

1. Open a chat.
2. Enter a message.
3. Click:

```text
Send
```

or press:

```text
Enter
```

---

## 😀 Using Emojis

Click the emoji button:

```text
😊
```

Available emojis:

```text
😀 😂 😍 😎 🤔 🥺 👍 🔥
```

The selected emoji is automatically inserted into the message text.

---

## 📎 Sending Images

Click:

```text
📎
```

Supported formats:

```text
.jpg
.png
```

After selection, the file is copied to:

```text
Uploads
```

and automatically sent to the chat.

---

## 🔍 Message Search

The search field is located at the top of the chat window:

```text
🔍 Search...
```

Features:

* Instant search
* Search by message text
* Automatic result display
* Navigation to the selected message

---

# 🎨 Interface Features

### Current User Messages

* Aligned to the right
* Pink background

### Other Users' Messages

* Aligned to the left
* White background

### Dates

When the day changes, a date header is displayed automatically:

```text
25.06.2026
```

### Auto Scroll

After sending a message, the chat automatically scrolls to the latest message.

---

# 📁 Uploads Folder

All attached images are automatically stored in:

```text
Uploads
```

This allows attachments to remain available after restarting the application.

---

Project created while learning:

* C#
* Avalonia UI
* Entity Framework Core
* SQLite
* Desktop Development

---

## ⭐ Version

```text
Messenger GUI v1.0
```

