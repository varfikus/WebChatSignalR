using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChatSignalR.Models;
using WebChatSignalR.Utils.Pagination;

namespace WebChatSignalR.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            Rooms = new  PagedResult<RoomViewModel>();
            Conversation = new ConversationViewModel();
        }
        public PagedResult<RoomViewModel> Rooms { get; set; }
        public ConversationViewModel Conversation { get; set; }
    }
    public class RoomViewModel{
        public int Id { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsReported { get; set; }
        public int? BlockedBy { get; set; }
        public int UpdateBy { get; set; }
        public string Excerpt { get; set; }
        public PersonViewModel Sender { get; set; }
        public PersonViewModel Recipient { get; set; }
        public int UnreadCount { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

   public class ConversationViewModel
    {
        public ConversationViewModel()
        {
            Messages = new PagedResult<MessageViewModel>();
            VoiceMessages = new PagedResult<VoiceMessageViewModel>();
            Sender = new PersonViewModel(); 
            Recipient = new PersonViewModel();
        }

        public string Id { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsReported { get; set; }
        public int? BlockedBy { get; set; }
        public PersonViewModel Sender { get; set; }
        public PersonViewModel Recipient { get; set; }
        public PagedResult<MessageViewModel>? Messages { get; set; }
        public PagedResult<VoiceMessageViewModel>? VoiceMessages { get; set; }
    }
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public byte[]? File { get; set; }
        public string? FileName { get; set; }
    }

    public class VoiceMessageViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public string? FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class PersonViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool? IsOnline { get; set; }
    }
    public class GroupMemberViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public PersonViewModel User { get; set; }
    }
    public class GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public PersonViewModel Creator { get; set; }
        public List<PersonViewModel> AllUsers { get; set; } = new List<PersonViewModel>();
        public List<GroupMemberViewModel> Members { get; set; } = new List<GroupMemberViewModel>();
        public List<int> SelectedUserIds { get; set; } = new List<int>();
        public bool IsBlocked { get; set; } 
        public int UnreadMessagesCount { get; set; } 
        public DateTime LastUpdated { get; set; } 
    }

    public class GroupConversationViewModel
    {
        public GroupConversationViewModel()
        {
            Messages = new PagedResult<MessageViewModel>();
            VoiceMessages = new PagedResult<VoiceMessageViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public List<GroupMemberViewModel> Members { get; set; } = new List<GroupMemberViewModel>(); 
        public PagedResult<MessageViewModel>? Messages { get; set; }
        public PagedResult<VoiceMessageViewModel>? VoiceMessages { get; set; }
        public bool IsBlocked { get; set; } 
        public int UnreadMessagesCount { get; set; }
    }

    public class GroupChatViewModel
    {
        public GroupChatViewModel()
        {
            Groups = new PagedResult<GroupViewModel>();
            GroupConversation = new GroupConversationViewModel();
        }
        public PersonViewModel CurrentUser { get; set; }
        public PagedResult<GroupViewModel> Groups { get; set; }
        public GroupConversationViewModel GroupConversation { get; set; }
    }
}
