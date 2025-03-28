using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChatSignalR.Models;


[Table("Groups")]
public partial class Group
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string Name { get; set; } = null!;

    [Column("CreatorId")]
    public int CreatorId { get; set; }

    [Column("IsBlocked")]
    public bool IsBlocked { get; set; }

    [Column("IsReported")]
    public bool IsReported { get; set; }

    [Column("BlockedBy")]
    public int? BlockedBy { get; set; }

    public virtual AppUser Creator { get; set; }

    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<VoiceMessage> VoiceMessages { get; set; } = new List<VoiceMessage>();
}
