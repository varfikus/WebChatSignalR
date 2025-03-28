using System;
using System.Collections.Generic;
using System.Reflection.Emit;
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

            // Room - Creator Relationship (Prevent Cascade Delete)
            builder.Entity<Room>()
                .HasOne(r => r.Creator)
                .WithMany()
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room - User Relationship (Prevent Cascade Delete)
            builder.Entity<Room>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room - VoiceMessages Relationship (Cascade Delete)
            builder.Entity<Room>()
                .HasMany(r => r.VoiceMessages)
                .WithOne(vm => vm.Room)
                .HasForeignKey(vm => vm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room - Messages Relationship (Cascade Delete)
            builder.Entity<Room>()
                .HasMany(r => r.Messages)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Group - Creator Relationship
            builder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Group - Members Relationship
            builder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete group members when group is deleted

            // GroupMember - User Relationship
            builder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany()
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user when removing group member

            // Group - Messages Relationship
            builder.Entity<Group>()
                .HasMany(g => g.Messages)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Group - VoiceMessages Relationship
            builder.Entity<Group>()
                .HasMany(g => g.VoiceMessages)
                .WithOne(vm => vm.Group)
                .HasForeignKey(vm => vm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message - User Relationship
            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // VoiceMessage - User Relationship
            builder.Entity<VoiceMessage>()
                .HasOne(vm => vm.User)
                .WithMany()
                .HasForeignKey(vm => vm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
