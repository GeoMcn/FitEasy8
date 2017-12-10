using FitEasy8.DAL;
using FitEasy8.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitEasy8.ViewModels
{
    public class AppUserManager : UserManager<User>
    {
        public AppUserManager(IUserStore<User> store)
        : base(store)
        {
        }

        // this method is called by Owin therefore best place to configure your User Manager
        public static AppUserManager Create(
            IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(
                new UserStore<User>(context.Get<FitEasyContext>()));

            // optionally configure your manager
            // ...

            return manager;
        }
    }
}