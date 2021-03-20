using ChitChatChew.Models.Entities;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Collections.Generic;

namespace ChitChatChew.Controllers
{
    public class UsersController : Controller
    {
        private MainModels db = new MainModels();   

        //GET: Users/Search/(username)
        public JsonResult Search(string username)
        {
            if(username != "")
            {
                var searchUser = db.Users.Where(x => x.username.StartsWith(username))
                                     .Select(x => new { id = x.id, username = x.username, profilePicture = x.profilePicture }).ToList();
                return Json(searchUser, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }
        #region Security
        /* -------------------------- Security ---------------------------- */

        // GET: Users/Login
        public ActionResult Login()
        {
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                TempData["error"] = "Already logged in";
                TempData["errorDetail"] = "You need to log out first";
                return RedirectToAction("Chat", "Home");
            }
            else
            {
                if(Request.Cookies["Username"] != null && Request.Cookies["Password"] != null)
                {
                    TempData["hasCookie"] = true;
                    TempData["cookieUsername"] = Request.Cookies["Username"].Value;
                    TempData["cookiePassword"] = Request.Cookies["Password"].Value;
                }
                return View();
            }
        }

        // GET: Users/Signup
        public ActionResult Signup()
        {
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                TempData["error"] = "Already logged in";
                TempData["errorDetail"] = "You need to log out first";
                return RedirectToAction("Chat", "Home");
            }
            else
            {
                return View();
            }
        }

        //GET: Users/Logout
        public ActionResult Logout()
        {
            var ult = new Utilities();
            try
            {
                if (ult.isLoggedIn())
                {
                    var currentSessionUser = (User)Session["currentUser"];
                    var currentUser = db.Users.Where(x => x.username == currentSessionUser.username).FirstOrDefault();
                    currentUser.isActive = false;
                    currentUser.isBusy = false;
                    db.SaveChanges();
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    TempData["error"] = "Not logged in";
                    TempData["errorDetail"] = "You need to login first";
                    return RedirectToAction("Login", "Users");
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Users/Login
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                Models.Entities.User loginUser = new User()
                {
                    username = collection["username"],
                    pw = collection["password"]
                };
                Models.Entities.User resultUser = new User();
                if (validateUserLogin(loginUser, ref resultUser))
                {
                    var check = collection["rememberMeCheck"];
                    if(check != null)
                    {
                        if(check == "on")
                        {
                            Response.Cookies["Password"].HttpOnly = true;
                            Response.Cookies["Username"].HttpOnly = true;
                            Response.Cookies["Username"].Value = resultUser.username;
                            Response.Cookies["Password"].Value = resultUser.pw;
                            Response.Cookies["Username"].Expires = DateTime.Now.AddYears(1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddYears(1);
                        }
                    }
                    else
                    {
                        Response.Cookies["Username"].Expires = DateTime.Now.AddYears(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddYears(-1);
                    }
                    Session.Add("currentUser", resultUser);
                    return RedirectToAction("Chat", "Home");
                }
                else
                {
                    TempData["error"] = "Wrong username/password";
                    TempData["errorDetail"] = "Please check your username/password.";
                    TempData["username"] = loginUser.username;
                    TempData["password"] = loginUser.pw;
                    return RedirectToAction("Login","Users");
                }
            }
            catch
            {
                return View();
            }
        }

        //POST: Users/Signup
        [HttpPost]
        public ActionResult Signup(FormCollection collection)
        {
            try
            {
                Models.Entities.User newUser = new User()
                {
                    username = collection["username"],
                    firstName = collection["firstName"],
                    lastName = collection["lastName"],
                    pw = collection["password"],
                    phone = "",
                    profilePicture = "man.png",
                    verificationCode = getRandomValidationCode(),
                    email = collection["email"],
                    createdAt = DateTime.Now,
                    isAdmin = false,
                    isActive = false,
                    isBusy = false,
                    statusText = ""
                };
                var existingUser = db.Users.Where(x => x.username == newUser.username).ToList();
                if (existingUser.Count == 0)
                {
                    db.Users.Add(newUser);
                    db.SaveChangesAsync();
                    TempData["success"] = "Successfully signing up";
                    TempData["successDetail"] = "Welcome to our site. You may now login and start chatting.";
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    TempData["error"] = "Username exists";
                    TempData["errorDetail"] = "That sucks isn't it? See if you can pick another username.";
                    TempData["username"] = newUser.username;
                    TempData["firstName"] = newUser.firstName;
                    TempData["lastName"] = newUser.lastName;
                    TempData["password"] = newUser.pw;
                    TempData["email"] = newUser.email;

                    return RedirectToAction("Signup", "Users");
                }
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        private bool validateUserLogin(User loginUser, ref User resultUser)
        {
            try
            {
                var dbUser = db.Users.Where(x => x.username == loginUser.username && x.pw == loginUser.pw).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.isActive = true;
                    dbUser.isBusy = false;
                    db.SaveChanges();
                    resultUser = dbUser;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Friends
        /* -------------------- Friends -------------------------- */

        //GET: Users/Friends/List
        [Route("users/friends/list")]
        public ActionResult getFriendList()
        {
            var ult = new Utilities();
            if (!ult.isLoggedIn())
            {
                TempData["error"] = "Not logged in";
                TempData["errorDetail"] = "You need to login first";
                return RedirectToAction("Login", "Users");
            }
            else
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.id == currentSessionUser.id).FirstOrDefault();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    var friends = currentUser.FriendList.Select(x => new { id = x.id, username = x.username, firstName = x.firstName, lastName = x.lastName, status = x.statusText, isActive = x.isActive, isBusy = x.isBusy,profilePicture = x.profilePicture }).ToList();
                    return Json(friends, JsonRequestBehavior.AllowGet);
                }
            }

        }

        //POST: Users/Friends/add
        [HttpPost]
        [Route("users/friends/add")]
        public void addFriend(TempUser requestUser)
        {
            int friendId = Convert.ToInt32(requestUser.id);
            var currentSessionUser = (User)Session["currentUser"];
            int currentUserId = Convert.ToInt32(currentSessionUser.id);
            var currentUser = db.Users.Where(x => x.id == currentUserId).FirstOrDefault();
            var foundUser = db.Users.Where(x => x.id == friendId).FirstOrDefault();
            if (foundUser != null && currentUser != null && currentUser != foundUser)
            {
                if (!currentUser.FriendList.Contains(foundUser))
                {
                    currentUser.FriendList.Add(foundUser);
                    foundUser.FriendList.Add(currentUser);
                    db.SaveChangesAsync();
                }
            }
        }

        [HttpPost]
        [Route("users/friends/delete")]
        public void deleteFriend(TempUser requestUser)
        {
            int friendId = Convert.ToInt32(requestUser.id);
            var currentSessionUser = (User)Session["currentUser"];
            int currentUserId = Convert.ToInt32(currentSessionUser.id);
            var currentUser = db.Users.Where(x => x.id == currentUserId).FirstOrDefault();
            var delUser = currentUser.FriendList.Where(x => x.id == friendId).FirstOrDefault();
            if (delUser != null && currentUser != null)
            {
                if (currentUser.FriendList.Contains(delUser))
                {
                    currentUser.FriendList.Remove(delUser);
                    delUser.FriendList.Remove(currentUser);
                    db.SaveChangesAsync();
                }
            }
        }


        //POST: Users/Friends/Remove
        #endregion

        #region Blocks
        /* ------------------ Blocks ----------------------------- */

        //GET: Users/Blocks/List

        //POST: Users/Blocks/Add

        //POST: Users/Blocks/Remove


        #endregion

        #region Profile
        /* -------------------- Profile ------------------------- */

        //POST: Users/Edit
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            Models.Entities.User currentUser = new User();
            if (Session["currentUser"] != null)
            {
                currentUser = (User)Session["currentUser"];
                var currentUserEdit = db.Users.Where(x => x.username == currentUser.username).FirstOrDefault();

                if (collection["password"] != currentUser.pw)
                {
                    TempData["wrongPasswordMessage"] = "Your entered password is wrong.";
                    return RedirectToAction("Chat", "Home");
                }
                else
                {
                    if (collection["newPassword"] != "" && collection["passwordConfirm"] != "")
                    {
                        if (collection["newPassword"] != collection["passwordConfirm"])
                        {
                            TempData["confirmNotMatchMessage"] = "Your new password and confirm password does not match.";
                            return RedirectToAction("Chat", "Home");
                        }
                        else
                        {
                            if(Request.Files.Count > 0)
                            {
                                var userImage = Request.Files["userImage"];
                                var filePath = Server.MapPath("~/Content/UserContent/UserImages/" + userImage.FileName);
                                userImage.SaveAs(filePath);
                                currentUserEdit.profilePicture = userImage.FileName;
                            }

                            currentUserEdit.firstName = collection["firstName"];
                            currentUserEdit.lastName = collection["lastName"];
                            currentUserEdit.pw = collection["newPassword"];
                            currentUserEdit.phone = collection["phone"];
                            currentUserEdit.email = collection["email"];
                            TempData["userEditSuccess"] = "Profile updated";
                            db.SaveChangesAsync();
                            Session["currentUser"] = currentUserEdit;
                            return RedirectToAction("Chat", "Home");
                        }
                    }
                    else
                    {
                        if (Request.Files.Count > 0)
                        {
                            var userImage = Request.Files["userImage"];
                            var filePath = Server.MapPath("~/Content/UserContent/UserImages/" + userImage.FileName);
                            userImage.SaveAs(filePath);
                            currentUserEdit.profilePicture = userImage.FileName;
                        }
                        currentUserEdit.firstName = collection["firstName"];
                        currentUserEdit.lastName = collection["lastName"];
                        currentUserEdit.phone = collection["phone"];
                        currentUserEdit.email = collection["email"];
                        db.SaveChangesAsync();
                        TempData["userEditSuccess"] = "Profile updated";
                        Session["currentUser"] = currentUserEdit;
                        return RedirectToAction("Chat", "Home");
                    }
                }
            }
            else
            {
                TempData["loginFirstMessage"] = "You need to login first";
                return RedirectToAction("Login", "Users");
            }
        }

        //POST: Users/setActiveState
        [HttpPost]
        public void setActiveState(string requestState)
        {
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.username == currentSessionUser.username).FirstOrDefault();
                if (currentUser != null)
                {
                    if (requestState == "busy")
                    {
                        currentUser.isBusy = true;
                    }
                    if (requestState == "active")
                    {
                        currentUser.isBusy = false;
                    }
                    db.SaveChanges();
                    Session["currentUser"] = currentUser;
                }
            }
        }

        public ActionResult getActiveState()
        {
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.username == currentSessionUser.username).FirstOrDefault();
                if (currentUser != null)
                {
                    var result = new { isActive = currentUser.isActive, isBusy = currentUser.isBusy };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }

        #endregion

        #region Helper
        /* -------------------- Helper functions ---------------------*/
        private string getRandomValidationCode()
        {
            Random r = new Random();
            var randomNumber = r.Next(1, 1000000);
            return randomNumber.ToString("000000");
        }

        #endregion
    }

    public class TempUser
    {
        public string id { get; set; }
    }
}