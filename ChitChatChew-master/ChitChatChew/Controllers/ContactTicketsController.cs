using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChitChatChew.Models.Entities;

namespace ChitChatChew.Controllers
{
    public class ContactTicketsController : Controller
    {
        private MainModels db = new MainModels();

        // GET: ContactTickets
        public ActionResult Index()
        {
            return View(db.ContactTickets.ToList());
        }

        // GET: ContactTickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactTicket contactTicket = db.ContactTickets.Find(id);
            if (contactTicket == null)
            {
                return HttpNotFound();
            }
            return View(contactTicket);
        }

        // GET: ContactTickets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,senderName,senderEmail,senderPhone,ticketText")] ContactTicket contactTicket)
        {
            if (ModelState.IsValid)
            {
                db.ContactTickets.Add(contactTicket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactTicket);
        }

        // GET: ContactTickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactTicket contactTicket = db.ContactTickets.Find(id);
            if (contactTicket == null)
            {
                return HttpNotFound();
            }
            return View(contactTicket);
        }

        // POST: ContactTickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,senderName,senderEmail,senderPhone,ticketText")] ContactTicket contactTicket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactTicket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactTicket);
        }

        // GET: ContactTickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactTicket contactTicket = db.ContactTickets.Find(id);
            if (contactTicket == null)
            {
                return HttpNotFound();
            }
            return View(contactTicket);
        }

        // POST: ContactTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactTicket contactTicket = db.ContactTickets.Find(id);
            db.ContactTickets.Remove(contactTicket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
