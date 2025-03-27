using System;
using System.Collections.Generic;

namespace WebChatSignalR.Models;

public class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsBlocked { get; set; }

    public bool IsReported { get; set; }

    public int BlockedBy { get; set; }

    public int UnreadCount { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int UpdatedBy { get; set; }

    public int CreatorId { get; set; }

    public int UserId { get; set; }

    public virtual AppUser Creator { get; set; }

    public virtual AppUser User { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<VoiceMessage> VoiceMessages { get; set; } = new List<VoiceMessage>();
}
