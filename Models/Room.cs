using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChatSignalR.Models;

[Table("Rooms")]
public class Room
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string? Name { get; set; }

    [Column("IsBlocked")]
    public bool IsBlocked { get; set; }

    [Column("IsReported")]
    public bool IsReported { get; set; }

    [Column("BlockedBy")]
    public int BlockedBy { get; set; }

    [Column("UnreadCount")]
    public int UnreadCount { get; set; }

    [Column("UpdatedDate")]
    public DateTime UpdatedDate { get; set; }

    [Column("UpdatedBy")]
    public int UpdatedBy { get; set; }

    [Column("CreatorId")]
    public int CreatorId { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    public virtual AppUser Creator { get; set; }

    public virtual AppUser User { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<VoiceMessage> VoiceMessages { get; set; } = new List<VoiceMessage>();
}
