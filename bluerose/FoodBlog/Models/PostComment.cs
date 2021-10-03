using System;

namespace FoodBlog.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public int Rating { get; set; }
        public DateTime ThisDateTime { get; set; }
        public int PostId { get; set; }
        
        public Post Post { get; set; }
        
    }
}