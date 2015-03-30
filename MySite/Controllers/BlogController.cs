using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySite.Models;
using System.Text;

namespace MySite.Controllers
{
    public class BlogController : Controller
    {
        private BlogModel model = new BlogModel();

        public ActionResult Index()
        {
            IEnumerable<Post> posts = from post in model.Posts
                                       where post.DateTime < DateTime.Now
                                       orderby post.DateTime descending
                                       select post;

            return View(posts);
        }

        public ActionResult Create()
        {
            Post newPost = new Post();
            return View(newPost);
        }

        public ActionResult Delete(int? id)
        {
            Post post = GetPost(id);
            return View(post);
        }

        public ActionResult DeletePost(int? id)
        {
            Post post = GetPost(id);
            model.Posts.Remove(post);
            model.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Post post = GetPost(id);
            StringBuilder tagList = new StringBuilder();
            foreach (Tag tag in post.Tags)
            {
                tagList.AppendFormat("{0}", tag.Name);
            }
            return View(post);
        }

        [ValidateInput(false)]
        public ActionResult Update(int? id, string title, string body, DateTime dateTime, string tags)
        {
            Post post = GetPost(id);
            post.Title = title;
            post.Body = body;
            post.DateTime = dateTime;
            post.Tags.Clear();
            post.UserId = 1;

            tags = tags ?? string.Empty;
            string[] tagNames = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tagName in tagNames)
            {
                post.Tags.Add(GetTag(tagName));
            }

            if (!id.HasValue)
            {
                model.Posts.Add(post);
            }
            model.SaveChanges();
            return RedirectToAction("Index");
        }

        private Tag GetTag(string tagName)
        {
            return model.Tags.Where(x => x.Name == tagName).FirstOrDefault() ?? new Tag() { Name = tagName };
        }

        private Post GetPost(int? id)
        {
            return id.HasValue ? model.Posts.Where(x => x.ID == id).First() : new Post() { ID = -1 };
        }
    }
}
