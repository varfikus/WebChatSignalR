using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.Data;
using WebChatSignalR.Models;

namespace WebChatSignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _db;
        private static readonly Dictionary<string, string> OnlineUser = new Dictionary<string, string>();
        public ChatHub(ChatDbContext db)
        {
            _db = db;
        }

        public async Task SendMessage(string conversationId, string UserId, string message, string base64File, string FileName)
        {
            int RoomId = Convert.ToInt32(conversationId);
            int loginUserId = Convert.ToInt32(GetLoginUser());
            var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == RoomId);

            if (room == null)
            {
                return;
            }

            try
            {
                room.UnreadCount += 1;
                room.UpdatedDate = DateTime.Now;
                room.UpdatedBy = loginUserId;

                var sanitizedMessage = string.IsNullOrWhiteSpace(message) ? null :
                    Regex.Replace(message.Trim(), @"(?i)<(?!img|a|/a|/img).*?>", string.Empty);

                
                byte[] fileBytes = !string.IsNullOrEmpty(base64File) ? Convert.FromBase64String(base64File) : null;

                var newMessage = new Message
                {
                    Content = sanitizedMessage,
                    RoomId = RoomId,
                    UserId = loginUserId,
                    Timestamp = DateTime.UtcNow,
                    File = fileBytes?.Length > 0 ? fileBytes : null,
                    FileName = !string.IsNullOrWhiteSpace(FileName) ? FileName : null,
                    GroupId = null
                };

                room.Messages.Add(newMessage);
                await _db.SaveChangesAsync();

                await Clients.Group(conversationId).SendAsync("ReceiveMessage",
     newMessage.UserId,
     newMessage.Content ?? "",  
     newMessage.File != null && newMessage.File.Length > 0 ? Convert.ToBase64String(newMessage.File) : "", 
     !string.IsNullOrWhiteSpace(newMessage.FileName) ? newMessage.FileName : "",
     newMessage.Timestamp);

                var otherUser = loginUserId == room.UserId ? room.CreatorId : room.UserId;
                if (OnlineUser.TryGetValue(otherUser.ToString(), out string connectionId))
                {
                    await SendNotification(connectionId, newMessage.RoomId.ToString(), newMessage.Content);
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("onError", "Message not sent! Message should be 1-500 characters.", e.Message);
            }
        }

        public async Task SendVoiceMessage(string RoomId, string UserId, string FilePath)
        {
            await Clients.Group(RoomId).SendAsync("ReceiveVoiceMessage", UserId, FilePath, DateTime.UtcNow);
        }

        public override Task OnConnectedAsync()
        {
            var loginUserId = GetLoginUser();
            if (!OnlineUser.ContainsKey(loginUserId))
            {
                OnlineUser.Add(loginUserId,Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUser.Remove(GetLoginUser());
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string RoomId)
        {
            if (string.IsNullOrEmpty(RoomId))
                throw new ArgumentException("Invalid room ID");

            await Groups.AddToGroupAsync(
                Context.ConnectionId, RoomId);
        }

        public async Task LeaveRoom(string RoomId)
        {
            if (string.IsNullOrEmpty(RoomId))
                throw new ArgumentException("Invalid room ID");

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId, RoomId);
        }

        private async Task SendNotification(string UserId, string RoomId, string message)
        {
            await Clients.Client(UserId).SendAsync("Notification",RoomId, message);
        }

        private string GetLoginUser()
        {
            return  Context.GetHttpContext().User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
       
    }
   
}