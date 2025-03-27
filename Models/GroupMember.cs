using System;
using System.Collections.Generic;

namespace WebChatSignalR.Models;

public partial class GroupMember
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int UserId { get; set; }

    public virtual Group? Group { get; set; }

    public virtual AppUser? User { get; set; }
}
