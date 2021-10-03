using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodBlog.Data;
using FoodBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore.Query;



namespace FoodBlog.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;
        private readonly IWebHostEnvironment _env;

        public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> um, IWebHostEnvironment env)
        {
            _um = um;
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Add(EnumModel.PostType type)
        {
            return View(new Post());
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Index(string currentFilter, string searchString, int ? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            
            var posts = from p in _context.Posts
                select p;
            if(!String.IsNullOrEmpty(searchString)){
            posts = (from p in _context.Posts
                where
                    p.Content.ToUpper().Contains(searchString.ToUpper())
                    || p.Title.ToUpper().Contains(searchString.ToUpper())
                    || p.Summary.ToUpper().Contains(searchString.ToUpper())
                    || p.User.Nickname.ToUpper().Contains(searchString.ToUpper())
                select p);
            }
            int pageSize = 9;
            return View(await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
            
        }


        
        
        [AllowAnonymous]
        [HttpGet]
        
        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.PostId = id.Value;
            var comments = _context.PostComments.Where(d => d.PostId.Equals(id.Value)).ToList();
            ViewBag.Comments = comments;

            var ratings = _context.PostComments.Where(d => d.PostId.Equals(id.Value)).ToList();
            if (ratings.Count() > 0)
            {
                var ratingSum = ratings.Sum(d => d.Rating);
                ViewBag.RatingSum = ratingSum;
                var ratingCount = ratings.Count();
                ViewBag.RatingCount = ratingCount;
            }
            else
            {
                ViewBag.RatingSum = 0;
                ViewBag.RatingCount = 0;
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Add([Bind("Id,Type, Review, Title,Summary,Content,ImagePath")] Post post, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (!Directory.Exists(Path.Combine(_env.WebRootPath, "images")))
                {
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images"));
                }

                if (file != null)
                {
                    var path = Path.Combine(_env.WebRootPath, "images", file.FileName);
                
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    post.ImageRootPath = path; 
                    post.ImageWebPath = $"/images/{file.FileName}";  
                }

                

              

                 var PostType = Request.Form["PostType"].ToString();
                 post.Type = PostType;
                
                   
                 post.Time = DateTime.Now;
                 post.User = _um.GetUserAsync(User).Result;
               
                  
               
                //
                 _context.Add(post);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));

            }
            
          
            return View(post);
        }
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Post = await _context.Posts.FindAsync(id);
            if (Post == null)
            {
                return NotFound();
            }

            var currentNickname = _um.GetUserAsync(User).Result.Nickname;

            if (Post.User.Nickname == currentNickname)
            {
                return View(Post); 
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
            
            
        }

        // POST: Blog/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,Content")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    // Acquire nickname from user in database

                    
                    post.UserId= _um.GetUserId(User);

                    
                    // change to current time and date
                    post.Time = DateTime.Now;
                    
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
           if (id == null)
            {
                return NotFound();
            }
           var post = await _context.Posts
               .FirstOrDefaultAsync(m => m.Id == id);
           _context.Posts.Remove(post);
           _context.SaveChanges();

            return RedirectToAction("Index");
        }
        
        
        
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Recipes(string currentFilter, string searchString, int ? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            
            var recipies = from p in _context.Posts
                where p.Type.Contains("1")
                select p;
            if(!String.IsNullOrEmpty(searchString)){
                recipies = (from p in _context.Posts
                    where
                        p.Content.ToUpper().Contains(searchString.ToUpper())
                        || p.Title.ToUpper().Contains(searchString.ToUpper())
                        || p.Summary.ToUpper().Contains(searchString.ToUpper())
                        || p.User.Nickname.ToUpper().Contains(searchString.ToUpper())
                    select p);
            }
            int pageSize = 9;
            return View(await PaginatedList<Post>.CreateAsync(recipies.AsNoTracking(), pageNumber ?? 1, pageSize));
            
        }
        [HttpGet]
        public async Task<IActionResult> Experiences(string currentFilter, string searchString, int ? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var experiences = from p in _context.Posts
                where p.Type.Contains("2")
                select p;
                
            if(!String.IsNullOrEmpty(searchString)){
                experiences = (from p in _context.Posts
                    where
                        p.Content.ToUpper().Contains(searchString.ToUpper())
                        || p.Title.ToUpper().Contains(searchString.ToUpper())
                        || p.Summary.ToUpper().Contains(searchString.ToUpper())
                        || p.User.Nickname.ToUpper().Contains(searchString.ToUpper())
                    select p);
            }
            int pageSize = 9;
            return View(await PaginatedList<Post>.CreateAsync(experiences.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> Resturants(string currentFilter, string searchString, int ? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
             var restaurants = from p in _context.Posts
                            where p.Type.Contains("3")
                            select p;
             if(!String.IsNullOrEmpty(searchString)){
                 restaurants = (from p in _context.Posts
                     where
                         p.Content.ToUpper().Contains(searchString.ToUpper())
                         || p.Title.ToUpper().Contains(searchString.ToUpper())
                         || p.Summary.ToUpper().Contains(searchString.ToUpper())
                         || p.User.Nickname.ToUpper().Contains(searchString.ToUpper())
                     select p);
             }
             int pageSize = 9;
             return View(await PaginatedList<Post>.CreateAsync(restaurants.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        
     
    }
}



