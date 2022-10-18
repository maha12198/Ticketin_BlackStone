using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketin_BlackStone.Data;
using Ticketin_BlackStone.Models;
using Ticketin_BlackStone.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Ticketin_BlackStone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        //to get/pass the User id from the view 
        private readonly UserManager<IdentityUser> _user;

        //inject service in the constructor
        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> user)
        {
            _db = context;
            _user = user;
        }

        // show list of all tickets
        public async Task<IActionResult> Index()
        {
            var applicationdbContext = _db.Tickets.Include(t => t.Project);

            return View(await applicationdbContext.ToListAsync());
        }

        //show details of the ticket
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var t = _db.Tickets.Include(t => t.Project).Include(t => t.User).FirstOrDefault(t => t.Id == id);
            t.Comments = _db.Comments.Where(t => t.TicketId == id).ToList();
            t.Files = _db.Files.Where(t => t.TicketId == id).ToList();

            TicketsVM TicketVm = new TicketsVM()
            {
                Ticket_Id = t.Id,
                Ticket_Description = t.Description,
                Project_Name = t.Project.Name,
                User_Id = t.User.Id,
                Ticket_Status = t.Status,
                Ticket_ClosingReason = t.ClosingReason

            };

            TicketVm.Comments = new List<Comment>();
            foreach (var item in t.Comments)
            {
                TicketVm.Comments.Add(item);
            }

            TicketVm.Files = new List<Models.File>();
            foreach (var item in t.Files)
            {
                TicketVm.Files.Add(item);
            }

            //pass value from action to action method 
            TempData["PassedTicketID"] = TicketVm.Ticket_Id;
            //TempData["PassedUserID"] = TicketVm.User_Id;
            return View(TicketVm);
        }

        // add new comment 
        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment newComment)
        {

            newComment.UserId = (await _user.GetUserAsync(HttpContext.User)).Id;
            newComment.TicketId = Convert.ToInt32(TempData["PassedTicketID"]);
            newComment.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _db.Add(newComment);
                await _db.SaveChangesAsync();
            }
            ViewData["TicketId"] = new SelectList(_db.Tickets, "Id", "Id", newComment.TicketId);
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "Id", newComment.UserId);

            return RedirectToAction("Details", new { id = newComment.TicketId });

        }

    }
}
