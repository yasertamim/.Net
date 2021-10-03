using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FoodBlog.Models;
using FoodBlog.Data;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using Stripe;
using Stripe.Checkout;
using Session = Stripe.BillingPortal.Session;
using SessionCreateOptions = Stripe.BillingPortal.SessionCreateOptions;
using SessionService = Stripe.BillingPortal.SessionService;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;
        private readonly IWebHostEnvironment _env;


        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> um, IWebHostEnvironment env)
        {
            _um = um;
            _context = context;
            _env = env;
            StripeConfiguration.ApiKey = "sk_test_51HtOk2GZGPrFNUvXZTINPypjksVivotpoBI1HJmrm7qM3uHX8sXhSpwzdWpFHYxVr5sIk1G5BH2Nf3EMzN0RAmfa00LgDcGrBB";

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        //contact
        
        
        public IActionResult Contact()
        {
            return View();
        }


        //Contact controller
        public IActionResult SendMail(string email, string title, string content)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("bluerose.ikt201g20h@gmail.com"));
            message.To.Add(new MailboxAddress("sondre-eftedal@hotmail.com"));
            message.Subject = title;
            message.Body = new TextPart("html")
            {
                Text = "From: " + email + "<br>" +
                       "Message : " +content
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.Authenticate("bluerose.ikt201g20h@gmail.com", "rhfjnsajbqzfqvaq");
                client.Send(message);
                client.Disconnect(false);
            }
            
            
            return View("Contact");
        }


        [HttpGet]
        public IActionResult success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult cancel()
        {
            return View();
        }
    }
}
