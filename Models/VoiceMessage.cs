using System;
using System.Collections.Generic;

namespace WebChatSignalR.Models;

public partial class VoiceMessage
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public int UserId { get; set; }

    public int? RoomId { get; set; }

    public int? GroupId { get; set; }

    public string? FileName { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual Group? Group { get; set; }

    public virtual Room? Room { get; set; }

    public virtual AppUser? User { get; set; }
}
