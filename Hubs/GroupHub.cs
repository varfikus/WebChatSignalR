using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.Data;
using WebChatSignalR.Models;

namespace WebChatSignalR.Hubs
{
    public class GroupHub : Hub
    {
        private readonly ChatDbContext _db;
        private static readonly Dictionary<string, string> OnlineUser = new Dictionary<string, string>();
        public GroupHub(ChatDbContext db)
        {
            _db = db;
        }

        public async Task SendGroupMessage(string GroupId, string UserId, string message, string base64File, string FileName)
        {
            int GroupIdInt = Convert.ToInt32(GroupId);
            int loginUserId = Convert.ToInt32(GetLoginUser());

            var group = await _db.Groups.Include(g => g.Members).FirstOrDefaultAsync(x => x.Id == GroupIdInt);

            if (group == null)
            {
                await Clients.Caller.SendAsync("onError", "Group not found.");
                return;
            }

            try
            {
                var sanitizedMessage = string.IsNullOrWhiteSpace(message) ? null :
                    Regex.Replace(message.Trim(), @"(?i)<(?!img|a|/a|/img).*?>", string.Empty);

                byte[] fileBytes = !string.IsNullOrEmpty(base64File) ? Convert.FromBase64String(base64File) : null;

                var newMessage = new Message
                {
                    Content = sanitizedMessage,
                    RoomId = null,
                    UserId = loginUserId,
                    Timestamp = DateTime.UtcNow,
                    File = fileBytes?.Length > 0 ? fileBytes : null,
                    FileName = !string.IsNullOrWhiteSpace(FileName) ? FileName : null,
                    GroupId = GroupIdInt
                };

                _db.Messages.Add(newMessage);
                group.Messages.Add(newMessage);
                await _db.SaveChangesAsync();

                var base64FileContent = fileBytes != null && fileBytes.Length > 0 ? Convert.ToBase64String(fileBytes) : string.Empty;

                await Clients.Group(GroupId).SendAsync("ReceiveGroupMessage",
                    newMessage.UserId,
                    newMessage.Content ?? "",
                    base64FileContent,
                    !string.IsNullOrWhiteSpace(newMessage.FileName) ? newMessage.FileName : "",
                    newMessage.Timestamp,
                    newMessage.GroupId);

                var otherUsers = group.Members.Where(m => m.Id != loginUserId).Select(m => m.Id).ToList();
                foreach (var Userid in otherUsers)
                {
                    if (OnlineUser.TryGetValue(Userid.ToString(), out string connectionId))
                    {
                        await SendNotification(connectionId, newMessage.GroupId.ToString(), newMessage.Content);
                    }
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("onError", "Message not sent! Error: " + e.Message);
            }
        }

        public async Task SendGroupVoiceMessage(string GroupId, string UserId, string FilePath, string groupId)
        {
            await Clients.Group(GroupId).SendAsync("ReceiveGroupVoiceMessage", UserId, FilePath, DateTime.UtcNow, groupId);
        }

        public override Task OnConnectedAsync()
        {
            var loginUserId = GetLoginUser();
            if (!OnlineUser.ContainsKey(loginUserId))
            {
                OnlineUser.Add(loginUserId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUser.Remove(GetLoginUser());
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGroup(string GroupId, string UserId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupId);
        }

        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }

        private async Task SendNotification(string UserId, string RoomId, string message)
        {
            await Clients.Client(UserId).SendAsync("Notification", RoomId, message);
        }

        private string GetLoginUser()
        {
            return Context.GetHttpContext().User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
