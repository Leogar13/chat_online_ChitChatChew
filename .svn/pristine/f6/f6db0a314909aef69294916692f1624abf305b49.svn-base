using ChitChatChew.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChitChatChew.Controllers
{
    public class MessagesController : Controller
    {
        private MainModels db = new MainModels();

        [Route("users/groups/{gId:int}/messages")]
        public ActionResult getMessages(int gId)
        {
            var utilities = new Utilities();
            if (utilities.isLoggedIn())
            {
                var currentGroup = db.Groups.Where(x => x.id == gId).FirstOrDefault();
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.id == currentSessionUser.id).FirstOrDefault();
                if (!currentGroup.Users.Contains(currentUser))
                {
                    TempData["error"] = "Not a member";
                    TempData["errorDetail"] = "You are not a member of that group.";
                    return RedirectToAction("Chat", "Home");
                }
                
                var messages = currentGroup.Messages.OrderBy(x => x.createdAt)
                                .Join(db.Users, message => message.senderId, user => user.id, 
                                (message, user) => new { messageText = message.messageText, isReceived = (message.senderId != currentUser.id), createdAt = message.createdAt?.ToString("dddd, HH:mm"), senderName = user.username, senderImage = user.profilePicture }).ToList();
                return Json(messages, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, detail = "Not logged in" });
            }
        }

        [HttpPost]
        [Route("users/groups/messages/add")]
        public ActionResult postMessage(PostMessage message)
        {
            var utilities = new Utilities();
            if (utilities.isLoggedIn())
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentGroupId = Convert.ToInt32(message.toGroup);
                var postToGroup = db.Groups.Where(x => x.id == currentGroupId).FirstOrDefault();
                Message newMessage = new Message() {messageText = message.messageText,  senderId = currentSessionUser.id, createdAt = DateTime.Now};
                
                if(postToGroup == null)
                {
                    return Json(new { success = false, detail = "Group not exist" });
                }
                else
                {
                    db.Messages.Add(newMessage);
                    newMessage.Groups.Add(postToGroup);
                    db.SaveChanges();
                    postToGroup.Messages.Add(newMessage);
                    db.SaveChanges();
                    return Json(new { success = true, detail = "Success" });
                }
            }
            else
            {
                return Json(new { success = false, detail = "Not logged in" });
            }
        }
    }

    public class PostMessage
    {
        public string toGroup { get; set; }
        public string messageText { get; set; }
    }
}