using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Donation_Managment_System
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Signup1",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Signup1", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "donation",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "donation", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "error",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "error", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BlockedErrorLogin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "BlockedErrorLogin", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PasswordError",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "PasswordError", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RecipientFileUpload",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "RecipientFileUpload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "recipient_navbar",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "recipient_navbar", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "requestSubmit",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "requestSubmit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "appoint_post",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "appoint_post", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "InvalidBG",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "InvalidBG", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ExtraQuantityError",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "ExtraQuantityError", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RecipientFront",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "RecipientFront", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ErrorLogin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "ErrorLogin", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DonationFront",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "DonationFront", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "appointment_set",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "appointment_set", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Error_Appoint",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Error_Appoint", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Reci_BloodForm",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Reci_BloodForm", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_appointment",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_appointment", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_Blockuser",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_Blockuser", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_request",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_request", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_ManageUser",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_ManageUser", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_ViewHistory",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_ViewHistory", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_Inventory",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_Inventory", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin_ViewProff",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Admin_ViewProff", id = UrlParameter.Optional }
            );
        }
    }
}
