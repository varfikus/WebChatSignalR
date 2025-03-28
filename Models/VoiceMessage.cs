using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChatSignalR.Models;

[Table("VoiceMessages")]
public partial class VoiceMessage
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("Timestamp")]
    public DateTime Timestamp { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("RoomId")]
    public int? RoomId { get; set; }

    [Column("GroupId")]
    public int? GroupId { get; set; }

    [Column("FileName")]
    public string? FileName { get; set; }

    [Column("FilePath")]
    public string FilePath { get; set; } = null!;

    public virtual Group? Group { get; set; }

    public virtual Room? Room { get; set; }

    public virtual AppUser? User { get; set; }
}
