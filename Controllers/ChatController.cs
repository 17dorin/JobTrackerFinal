using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class ChatController : Controller
    {
        private readonly FinalProjectContext _context;

        public ChatController(FinalProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chat(string receiverId)
        {
            AspNetUser sender = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            AspNetUser receiver = _context.AspNetUsers.Find(receiverId);
            TempData["sender"] = sender.UserName;
            TempData["receiver"] = receiver.UserName;

            TempData["senderId"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            TempData["receiverId"] = receiverId;

            List<Chat> myMessages = _context.Chats.Where(x => x.Sender == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Receiver == receiverId).ToList();
            List<Chat> theirMessages = _context.Chats.Where(x => x.Sender == receiverId && x.Receiver == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            myMessages.AddRange(theirMessages);
            myMessages = myMessages.OrderBy(x => x.Id).ToList();
            return View(myMessages);
        }

        [HttpPost]
        public IActionResult SendMessage(string receiverId, string message)
        {
            Chat newChat = new Chat();
            newChat.Sender = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            newChat.Receiver = receiverId;
            newChat.Message = message;
            _context.Chats.Add(newChat);
            _context.SaveChanges();

            TempData["senderId"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            TempData["receiverId"] = receiverId;

            List<Chat> myMessages = _context.Chats.Where(x => x.Sender == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Receiver == receiverId).ToList();
            List<Chat> theirMessages = _context.Chats.Where(x => x.Sender == receiverId && x.Receiver == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            myMessages.AddRange(theirMessages);
            myMessages = myMessages.OrderBy(x => x.Id).ToList();


            return RedirectToAction("Chat", "Chat", new { receiverId = receiverId });
        }

        public IActionResult YourConversations()
        {
            List<string> conversationIds = _context.Chats.Where(x => x.Sender == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .Select(x => x.Receiver).Distinct().ToList();

            List<AspNetUser> conversations = _context.AspNetUsers.Where(x => conversationIds.Contains(x.Id)).ToList();

            return View(conversations);
        }
    }
}
