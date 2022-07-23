using IdentityPractise.DAL;
using IdentityPractise.Models;
using IdentityPractise.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPractise.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Detail Action
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null) return NotFound();

            Blog blog = await _context.Blogs.Include(b => b.Comments).FirstOrDefaultAsync(b => b.Id == Id);
            if (blog == null)
            {
                return NotFound();
            }


            BlogDetailVM blogVM = new BlogDetailVM
            {
                Blog = blog,
                Blogs = await _context.Blogs.OrderByDescending(b => b.Id).Take(5).ToListAsync(),
            };

            ViewBag.AppUserId = "";
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.AppUserId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            }
            return View(blogVM);
        }

        /// <summary>
        /// Comment Post Action
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="BlogId"></param>
        /// <returns></returns>
        
        public async Task<IActionResult> AddComment(Comment comment, int BlogId)
        {
            if (!ModelState.IsValid) return View();
            AppUser appUser = new AppUser();

            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            Comment NewComment = new Comment
            {
                Message = comment.Message,
                BlogId = BlogId,
                AppUserId = appUser.Id,
                CreateAt = DateTime.Now
            };
            await _context.AddAsync(NewComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("detail", new { id = BlogId });
        }
        /// <summary>
        /// Delete Comment Action 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="BlogId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteComment(int? Id,int BlogId)
        {
            Comment comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == Id);
            if (comment == null) return NotFound();
             _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("detail",new {id= BlogId});
        }
    }
}
