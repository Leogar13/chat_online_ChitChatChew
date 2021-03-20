using ChitChatChew.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChitChatChew.Controllers
{
    public class Utilities
    {
        MainModels db = new MainModels();
        public bool isLoggedIn()
        {
            
            var currentSessionUser = (User)HttpContext.Current.Session["currentUser"];
            if(currentSessionUser != null)
            {
                var currentUser = db.Users.Where(x => x.id == currentSessionUser.id).FirstOrDefault();
                if (currentUser != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            
        }

    }
}