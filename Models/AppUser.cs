using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChatSignalR.Models;

public class AppUser : IdentityUser<int>
{
    public string Name { get; set; }
    public string Avatar { get; set; }
    public ICollection<Room> CreatedRooms { get; set; } = new List<Room>(); 
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

