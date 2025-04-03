using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.Models;

namespace WebChatSignalR.Data
{
    public class ChatDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
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
            builder.Entity<AppUser>()
        .Property(x => x.Name).HasMaxLength(100);

            // Настройки для Message
            builder.Entity<Message>()
                .Property(x => x.Content).HasMaxLength(500);
            builder.Entity<Message>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Message>()
                .HasOne(x => x.Room)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Message>()
                .HasOne(x => x.Group)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройки для Room
            builder.Entity<Room>()
                .HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Room>()
                .HasOne(x => x.User)
                .WithMany(x => x.Rooms)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройки для Group
            builder.Entity<Group>()
                .HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройки для GroupMember
            builder.Entity<GroupMember>()
                .HasOne(x => x.Group)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<GroupMember>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройки для VoiceMessage
            builder.Entity<VoiceMessage>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<VoiceMessage>()
                .HasOne(x => x.Room)
                .WithMany(x => x.VoiceMessages)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<VoiceMessage>()
                .HasOne(x => x.Group)
                .WithMany(x => x.VoiceMessages)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}