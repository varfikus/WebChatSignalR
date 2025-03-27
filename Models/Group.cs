using System;
using System.Collections.Generic;

namespace WebChatSignalR.Models;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CreatorId { get; set; }

    public bool IsBlocked { get; set; }

    public bool IsReported { get; set; }

    public int? BlockedBy { get; set; }

    public virtual AppUser Creator { get; set; }

    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<VoiceMessage> VoiceMessages { get; set; } = new List<VoiceMessage>();
}
