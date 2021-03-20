using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ChitChatChew.Models.Entities;
namespace ChitChatChew.Controllers
{
    public class HomeController : Controller
    {
        private MainModels db = new MainModels();

        //GET /Home/index
        public ActionResult Index()
        {
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                ViewBag.currentUser = Session["currentUser"];
            }
            return View();
        }

        //GET /Home/Contact

        public ActionResult Contact()
        {
            return View();
        }

        //GET  /Home/Chat

        public ActionResult Chat()
        {
            var emojis = new Emojis();
            var ult = new Utilities();
            ViewBag.Message = "Chat";
            if(ult.isLoggedIn())
            {
                ViewBag.currentUser = Session["currentUser"];
                ViewBag.emojis = emojis;
                return View();
            }
            else
            {
                TempData["error"] = "Not logged in";
                TempData["errorDetail"] = "You need to login first";
                return RedirectToAction("Login", "Users");
            }
        }


        //POST /Home/Contact

        [HttpPost]
        public ActionResult Contact(FormCollection ticketInfo)
        {
            try
            {
                Models.Entities.ContactTicket newTicket = new ContactTicket()
                {
                    senderEmail = ticketInfo["email"],
                    senderName = ticketInfo["name"],
                    senderPhone = ticketInfo["phone"],
                    ticketText = ticketInfo["message"]
                };
                db.ContactTickets.Add(newTicket);
                db.SaveChangesAsync();
                return RedirectToAction("Chat", "Home");
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}