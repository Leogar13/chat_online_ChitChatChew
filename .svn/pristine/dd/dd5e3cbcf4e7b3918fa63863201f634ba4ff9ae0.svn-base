using ChitChatChew.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChitChatChew.Controllers
{
    public class TempGroup
    {
        public string groupName { get; set; }
        public string createdBy { get; set; }
        public string[] Users { get; set; }
    }

    public class DelGroup
    {
        public string groupid { get; set; }
    }
    public class GroupsController : Controller
    {
        private MainModels db = new MainModels();

        //POST: Users/Groups/Create
        [HttpPost]
        public ActionResult Create(TempGroup requestGroup)
        {
            //check if group already exist
            var groupCreatorID = Convert.ToInt32(requestGroup.createdBy);
            var existingGroup = db.Groups.Where(x => x.groupName == requestGroup.groupName && x.createdBy == groupCreatorID).FirstOrDefault();
            if (existingGroup == null)
            {
                //Create a new group (name, createdBy, isPublic)
                Group newGroup = new Group();
                newGroup.groupName = setGroupName(requestGroup.groupName);
                if (requestGroup.createdBy != null && requestGroup.createdBy.Trim() != "")
                {
                    newGroup.createdBy = Convert.ToInt32(requestGroup.createdBy);
                }

                //add users
                for (int i = 0; i < requestGroup.Users.Length; i++)
                {
                    var addedUserId = Convert.ToInt32(requestGroup.Users[i]);
                    var addUser = db.Users.Where(x => x.id == addedUserId).FirstOrDefault();
                    if (addUser != null)
                    {
                        newGroup.Users.Add(addUser);
                    }
                }
                //if the creator is not in already then add creator
                if (!newGroup.Users.Contains(db.Users.Where(x => x.id == groupCreatorID).FirstOrDefault()))
                {
                    newGroup.Users.Add(db.Users.Where(x => x.id == groupCreatorID).FirstOrDefault());
                }
                db.Groups.Add(newGroup);
                db.SaveChangesAsync();
                var isGroupActive = false;
                foreach(var user in newGroup.Users)
                {
                    if(user.id != newGroup.createdBy && user.isActive == true)
                    {
                        isGroupActive = true;
                        break;
                    }
                }
                return Json(new { groupId = newGroup.id, groupName = newGroup.groupName, isActive = isGroupActive }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var isGroupActive = false;
                foreach (var user in existingGroup.Users)
                {
                    if (user.id != existingGroup.createdBy && user.isActive == true)
                    {
                        isGroupActive = true;
                        break;
                    }
                }
                return Json(new { groupId = existingGroup.id, groupName = existingGroup.groupName, isActive = isGroupActive }, JsonRequestBehavior.AllowGet);
            }
        }
        //POST: Users/Groups/Delete
        [HttpPost]
        public ActionResult Delete (DelGroup delGroup)
        {
            var id = Convert.ToInt32(delGroup.groupid);
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.id == currentSessionUser.id).FirstOrDefault();
                var joinedGroups = currentUser.InGroups.Select(x => new { id = x.id, createid = x.createdBy, users = x.Users }).ToList();
                foreach (var g in joinedGroups)
                {
                    if (id == g.id && currentUser.id == g.createid)
                    {
                        DelGroup(id);
                        return Json(new { success = true, detail = "Delete succesfully" });

                    }
                }
                return Json(new { success = false, detail = "You are not creator" });
            }
            else
            {
                return Json(new { success = false, detail = "Not logged in" });
            }
        }
        [HttpPost]
        public ActionResult Leave (DelGroup delGroup)
        {
            var id = Convert.ToInt32(delGroup.groupid);
            var ult = new Utilities();
            if (ult.isLoggedIn())
            {
                var currentSessionUser = (User)Session["currentUser"];
                var currentUser = db.Users.Where(x => x.id == currentSessionUser.id).FirstOrDefault();
                var joinedGroups = currentUser.InGroups.Select(x => new { id = x.id, createid = x.createdBy, users = x.Users }).ToList();
                int count = 0;
                foreach (var g in joinedGroups)
                {
                    if (id == g.id)
                    {
                        foreach (var u in g.users)
                        {
                            count++;
                        }
                    }
                    while (count > 2)
                    {
                        if (id == g.id && currentUser.id == g.createid)
                        {
                            return Json(new { success = false, detail = "You cannot leave" });
                        }
                        if (id == g.id && currentUser.id != g.createid)
                        {
                            LeaveGroup(id, currentUser.id);
                            return Json(new { sucess = true, detail = "Leave successfully" });
                        }
                    };
                }
                return Json(new { success = false, detail = "You cannot leave" });
            }
            else
            {
                return Json(new { success = false, detail = "Not logged in" });
            }

        }

        public void LeaveGroup (int groupid, int id)
        {
            Group group = new Group();
            group = db.Groups.Find(groupid);
            if (group != null)
            {
                var query = "Delete from GroupUsers where groupId = "+groupid+ " and userId = "+ id;
                db.Database.ExecuteSqlCommand(query);
                db.SaveChangesAsync();
            }
        }
        public void DelGroup(int id)
        {
            Group group = new Group();
            group = db.Groups.Find(id);
            if (group != null)
            {
                var query = "DELETE FROM GroupMessages WHERE groupId = "+ id +" DELETE FROM GroupUsers WHERE groupId = "+id+" DELETE FROM Groups WHERE id = "+id;
                db.Database.ExecuteSqlCommand(query);
                db.SaveChangesAsync();
            }
        }

        //GET: Users/Groups/List
        [Route("users/groups/list")]
        public ActionResult getUserGroups()
        {
            var ult = new Utilities();
            if (!ult.isLoggedIn())
            {
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
                    var joinedGroups = currentUser.InGroups.Select(x => new { id = x.id, groupName = x.groupName, users = x.Users, messages = x.Messages }).ToList();
                    List<Object> resultGroups = new List<object>();
                    foreach (var g in joinedGroups)
                    {
                        var isGroupActive = false;
                        foreach (var u in g.users)
                        {
                            if (u.isActive == true && u.id != currentUser.id)
                            {
                                isGroupActive = true;
                                break;
                            }
                        }
                        if (g.messages.Count > 0)
                        {
                            var latestMessageText = g.messages.OrderByDescending(x => x.createdAt).FirstOrDefault().messageText;
                            var latestMessageTime = g.messages.OrderByDescending(x => x.createdAt).FirstOrDefault().createdAt;
                            var timeString = "";
                            if(latestMessageTime != null)
                            {
                                var now = DateTime.Now;
                                var msgTime = latestMessageTime.Value;
                                if(now.Year > msgTime.Year)
                                {
                                    timeString = latestMessageTime?.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    int dowNow, dowMsg;
                                    TimeSpan span = now.Subtract(msgTime);
                                    dowNow = (int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek;
                                    dowMsg = (int)msgTime.DayOfWeek == 0 ? 7 : (int)msgTime.DayOfWeek;
                                    bool isSameWeek = (span.Days > (dowNow - dowMsg)) ? false : true;
                                    if (isSameWeek == true)
                                    {
                                        if (span.Days == 0)
                                        {
                                            timeString = latestMessageTime?.ToString("HH:mm");
                                        }
                                        else
                                        {
                                            timeString = latestMessageTime?.ToString("ddd");
                                        }
                                    }
                                    else
                                    {
                                        timeString = latestMessageTime?.ToString("dd MMM");
                                    }
                                }
                            }
                            resultGroups.Add(new { id = g.id, groupName = g.groupName, isActive = isGroupActive, latestMessageText = latestMessageText, latestMessageTime = timeString });
                        }
                        else
                        {
                            resultGroups.Add(new { id = g.id, groupName = g.groupName, isActive = isGroupActive, latestMessageText = "", latestMessageTime = "" });
                        }
                    }
                    return Json(resultGroups, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private string setGroupName(string groupName)
        {
            if (groupName == null || groupName.Trim() == "")
            {
                Random r = new Random();
                groupName = "GroupChat" + r.Next(0, 9999).ToString();
            }
            return groupName;
        }


        // GET: Groups/getUserInGroup
        public ActionResult getUserInGroup(string ID)
        {
            int id = Int32.Parse(ID);
            var ult = new Utilities();
            if (!ult.isLoggedIn())
            {
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
                    var joinedGroups = currentUser.InGroups.Select(x => new { id = x.id, groupName = x.groupName, users = x.Users, messages = x.Messages, creatid = x.createdBy }).ToList();
                    List<Object> resultList = new List<object>();
                    foreach(var g in joinedGroups)
                    {
                        if (id == g.id)
                        {
                            foreach (var u in g.users)
                            {
                                if (u.id != currentUser.id)
                                {
                                    if ((u.phone == null || u.phone == ""))
                                    {
                                        if (u.id != g.creatid)
                                        {
                                            resultList.Add(new { username = u.username, lastname = u.lastName, firstname = u.firstName, email = u.email, phone = "", picture = u.profilePicture, creator = "Group Member" });
                                        }
                                        else
                                        {
                                            resultList.Add(new { username = u.username, lastname = u.lastName, firstname = u.firstName, email = u.email, phone = "", picture = u.profilePicture, creator = "Group Creator" });

                                        }

                                    }
                                    else
                                    {
                                        if (u.id != g.creatid)
                                        {
                                            resultList.Add(new { username = u.username, lastname = u.lastName, firstname = u.firstName, email = u.email, phone = u.phone, picture = u.profilePicture, creator = "Group Member" });
                                        }
                                        else
                                        {
                                            resultList.Add(new { username = u.username, lastname = u.lastName, firstname = u.firstName, email = u.email, phone = u.phone, picture = u.profilePicture, creator = "Group Creator" });

                                        }
                                    }
                                }
                            }
                        }

                    }
                    return Json(resultList, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // GET: Groups/getNumberMembers
        public ActionResult getNumberMembers(string ID)
        {
            int id = Int32.Parse(ID);
            var ult = new Utilities();
            if (!ult.isLoggedIn())
            {
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
                    var joinedGroups = currentUser.InGroups.Select(x => new { id = x.id, groupName = x.groupName, users = x.Users, messages = x.Messages }).ToList();
                    int count = 0;
                    foreach (var g in joinedGroups)
                    {
                        if (id == g.id)
                        {
                            foreach (var u in g.users)
                            {
                                count++;
                            }
                        }

                    }
                    return Json(count, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }

}