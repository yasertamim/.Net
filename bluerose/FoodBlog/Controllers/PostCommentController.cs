using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FoodBlog.Data;
using FoodBlog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBlog.Controllers
{
    
    public class PostCommentController:Controller
    {
        
        private readonly ApplicationDbContext _db;
            private readonly UserManager<ApplicationUser> _um;
            private readonly IWebHostEnvironment _env;

        public PostCommentController(ApplicationDbContext db, UserManager<ApplicationUser> um, IWebHostEnvironment env)
        {
            _um = um;
            _db = db;
            _env = env;
        }

        // GET: ArticlesComments


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int PostId, int Rating, string Comment)
        {
            PostComment pComment = new PostComment();
            pComment.PostId = PostId;
            pComment.Rating = Rating;
            pComment.Comments = Comment;
            pComment.ThisDateTime =DateTime.Now;

            _db.PostComments.Add(pComment);
            _db.SaveChanges();

            return RedirectToAction("Details", "Blog", new {id = PostId});
        }

        // POST: ArticlesComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("CommentId,Comments,ThisDateTime,ArticleId,Rating")] PostComment postComment)
        {
            if (ModelState.IsValid)
            {
                _db.PostComments.Add(postComment);
                _db.SaveChanges();
                return RedirectToPage("Blog/Index");
            }

            return View("~/Views/Blog/Index.cshtml");
        }

        // GET: ArticlesComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PostComment articlesComment = _db.PostComments.Find(id);
            if (articlesComment == null)
            {
                return NotFound();
            }
            return View("~/Views/Blog/Index.cshtml");        }

           }


        
    }

