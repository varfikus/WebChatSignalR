using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.Models;

namespace WebChatSignalR.Data
{
    public class ChatDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<VoiceMessage> VoiceMessages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Room>()
                .HasOne(r => r.Creator)
                .WithMany()
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Room>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Room>()
                .HasMany(r => r.VoiceMessages)
                .WithOne()
                .HasForeignKey(vm => vm.RoomId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Room>()
                .HasMany(r => r.Messages)
                .WithOne()
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
