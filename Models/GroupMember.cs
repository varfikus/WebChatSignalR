using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChatSignalR.Models;

[Table("GroupMembers")]
public partial class GroupMember
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("GroupId")]
    public int GroupId { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    public virtual Group? Group { get; set; }

    public virtual AppUser? User { get; set; }
}
