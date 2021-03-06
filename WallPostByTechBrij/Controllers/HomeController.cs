﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WallPostByTechBrij.Filters;
using WallPostByTechBrij.Manager;
using WallPostByTechBrij.Models;
using WebMatrix.WebData;

namespace WallPostByTechBrij.Controllers
{
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        private WallEntities db = new WallEntities();
        LogicManager aManager = new LogicManager();
        [Authorize]
        public ActionResult Index()
        {
            string searchString = "Need To think";
            var userProfiles = from m in db.UserProfiles
                               select m;
            LogicManager aLogicManager = new LogicManager();
            List<int> myfriends = aLogicManager.GetAllfriendIncludingPendingRequest(WebSecurity.CurrentUserId);

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var myfriend in myfriends)
                {
                    userProfiles = userProfiles.Where(s => s.UserId != myfriend);
                }
                userProfiles = userProfiles.Where(s =>s.UserId != WebSecurity.CurrentUserId);

            }

            return View(userProfiles.ToList());
        }

       
        public ActionResult UsersInfo(int? id)
        {
           
           var user = db.UserProfiles.Find(id);
            ViewBag.Message = "Your app description page.";

            return View(user);
        }
        //[HttpPost]
        //public ActionResult UsersInfo(int? id)
        //{
        //    ViewBag.Message = "Your app description page.";

        //    return View();
        //}

        public ActionResult Friends()
        {
            var myFriends = aManager.GetAllfriendWithProfile(WebSecurity.CurrentUserId);
            //var userProfiles = from m in db.UserProfiles
            //                   select m;

            return View(myFriends);
        }
        public ActionResult MyProfile(int? id)
        {
            
            bool isFriendRequestAllreadySend = aManager.CheckFriendShipPending((int)id);
            if (!isFriendRequestAllreadySend)
            {
                Friendship aFriendship = new Friendship();
                aFriendship.FriendId = WebSecurity.CurrentUserId;
                aFriendship.UserId = id;
                aFriendship.Status = 0;
                db.Friendships.Add(aFriendship);
                db.SaveChanges();
               
            }
            else
            {
                Friendship aFriendship = new Friendship();
                aFriendship.FriendId = WebSecurity.CurrentUserId;
                aFriendship.UserId =id;
                aFriendship.Status = 1;
                using (WallEntities db = new WallEntities())
                {
                    foreach (var friendshipcheck in db.Friendships.ToList())
                    {
                        if (friendshipcheck.UserId == aFriendship.FriendId)
                        {
                            if (friendshipcheck.FriendId == aFriendship.UserId)
                            {
                                friendshipcheck.Status = 1;
                                db.Friendships.AddOrUpdate(friendshipcheck);
                                db.SaveChanges();
                            }
                        }
                    }

                    db.Friendships.Add(aFriendship);
                    db.SaveChanges();

                }
            }

            return RedirectToAction("Index");
        }
      
    }
}
