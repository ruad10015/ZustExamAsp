﻿using Microsoft.AspNetCore.Mvc;
using Zust.Business.Abstract;
using Zust.Entities.Models;
using Zust.Web.Helpers.ConstantHelpers;
using Zust.Web.Helpers.Utilities;
using Zust.Web.Models;

namespace Zust.Web.Controllers.ApiControllers
{
    /// <summary>
    /// API controller responsible for handling chat-related operations.
    /// </summary>
    [Route(Routes.ChatAPI)]
    [ApiController]
    public class ChatController : ControllerBase
    {
        /// <summary>
        /// Gets the user service used by the controller.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Gets the chat service used by the controller.
        /// </summary>
        private readonly IChatService _chatService;

        /// <summary>
        /// Gets the message service used by the controller.
        /// </summary>
        private readonly IMessageService _messageService;

        /// <summary>
        /// Gets the notification service used by the controller.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class with the specified services.
        /// </summary>
        /// <param name="chatService">The chat service to be used by the controller.</param>
        /// <param name="messageService">The message service to be used by the controller.</param>
        /// <param name="userService">The user service to be used by the controller.</param>
        /// <param name="notificationService">The notification service to be used by the controller.</param>
        public ChatController(IChatService chatService, IMessageService messageService, IUserService userService, INotificationService notificationService)
        {
            _chatService = chatService;

            _messageService = messageService;

            _userService = userService;

            _notificationService = notificationService;
        }

        /// <summary>
        /// Adds a new message to the chat.
        /// </summary>
        /// <param name="model">The SendMessageViewModel containing the message data.</param>
        /// <returns>Returns ActionResult with a MessageNotificationViewModel on success, or BadRequest with an error message on failure.</returns>
        [HttpPost(Routes.AddMessage)]
        public async Task<ActionResult<Message>> AddMessage([FromBody] SendMessageViewModel model)
        {
            try
            {
                // For Current User
                var message = new Message()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = model.Message.Text,
                    ReceiverUserId = model.Message.ReceiverUserId,
                    SenderUserId = model.Message.SenderUserId,
                    ChatId = model.Message.ChatId,
                    DateSent = DateTime.Now
                };

                await _messageService.AddMessageAsync(message);

                message.ReceiverUser = await _userService.GetUserByIdAsync(model.Message.ReceiverUserId);

                message.SenderUser = await _userService.GetUserByIdAsync(model.Message.SenderUserId);

                message.Chat = await _chatService.GetChatByIdAsync(model.Message.ChatId);

                // For User To Send Message
                var otherUserChat = await _chatService.GetChatAsync(model.Message.ReceiverUserId, model.Message.SenderUserId);

                var message2 = new Message()
                {
                    Id = Guid.NewGuid().ToString(),

                    Text = model.Message.Text,

                    ReceiverUserId = model.Message.ReceiverUserId,

                    SenderUserId = model.Message.SenderUserId,

                    ChatId = otherUserChat.Id,

                    DateSent = DateTime.Now
                };

                await _messageService.AddMessageAsync(message2);

                var currentUser = await UserHelper.GetCurrentUserAsync(HttpContext);

                var messageNotificationVM = new MessageNotificationViewModel()
                {
                    Message = message,
                    Notification = null
                };

                if (!model.FirstMessageSent)
                {

                    var notification = new Notification()
                    {
                        Id = Guid.NewGuid().ToString(),

                        Date = DateTime.Now,

                        IsRead = false,

                        FromUserId = currentUser.Id,

                        FromUser = currentUser,

                        ToUserId = model.Message.ReceiverUserId,

                        ToUser = await _userService.GetUserByIdAsync(model.Message.ReceiverUserId),

                        Message = NotificationType.GetSentYouMessageMessage(currentUser.UserName),
                    };

                    messageNotificationVM.Notification = notification;

                    await _notificationService.AddAsync(notification);
                }

                // Success
                return Ok(messageNotificationVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all chat users for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose chats need to be retrieved.</param>
        /// <returns>Returns ActionResult with a list of users representing the chat participants on success, or BadRequest with an error message on failure.</returns>
        [HttpGet(Routes.GetChats)]
        public async Task<ActionResult<IEnumerable<User>>> GetChats(string userId)
        {
            try
            {
                var chats = await _chatService.GetAllUserChats(userId);

                var list = chats.ToList();

                var currentUser = await UserHelper.GetCurrentUserAsync(HttpContext);

                var usersTasks = list.Select(async c =>
                {
                    if (c.SenderUserId != currentUser.Id)
                    {
                        return await _userService.GetUserByIdAsync(c.SenderUserId);
                    }
                    else if (c.ReceiverUserId != currentUser.Id)
                    {
                        return await _userService.GetUserByIdAsync(c.ReceiverUserId);
                    }

                    return null; // Return null for cases when neither SenderUserId nor ReceiverUserId matches the currentUser's Id
                });

                var users = await Task.WhenAll(usersTasks);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the last message between the current user and the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user with whom to retrieve the last message.</param>
        /// <returns>
        /// An asynchronous operation that returns an <see cref="ActionResult"/> containing the text of the last message,
        /// or an empty string if there is no last message, or a <see cref="BadRequestResult"/> if an error occurs during the process.
        /// </returns>
        [HttpGet(Routes.GetLastMessage)]
        public async Task<ActionResult<string>> GetLastMessage(string userId)
        {
            try
            {
                var currentUser = await UserHelper.GetCurrentUserAsync(HttpContext);

                var chat = await _chatService.GetChatAsync(currentUser.Id, userId);

                var message = await _messageService.GetLastMessageAsync(chat);

                if (message == null)
                {
                    return Ok(String.Empty);
                }
                else
                {
                    return Ok(message.Text);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
