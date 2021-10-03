using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FoodBlog.Models
{
    public class Post : IEnumerable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Type { get; set; }
        
        public EnumModel.PostReview Review { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }

        
        public string ImageRootPath { get; set; }
  
        public string ImageWebPath { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public List<PostComment> PostComments { get; set; }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}