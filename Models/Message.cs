using System;
using System.Collections.Generic;

namespace WebChatSignalR.Models;

public partial class Message
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public DateTime Timestamp { get; set; }

    public int UserId { get; set; }

    public int? RoomId { get; set; }

    public int? GroupId { get; set; }

    public byte[]? File { get; set; }

    public string? FileName { get; set; }

    public virtual Group? Group { get; set; }

    public virtual Room? Room { get; set; }

    public virtual AppUser? User { get; set; }
}
