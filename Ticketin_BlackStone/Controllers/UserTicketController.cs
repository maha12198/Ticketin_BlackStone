using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ticketin_BlackStone.Data;
using Ticketin_BlackStone.Models;
using Ticketin_BlackStone.Models.ViewModels;

// controller for UserBasic user role (the default role of the system)
namespace Ticketin_BlackStone.Controllers
{
    [Authorize(Roles = "BasicUser")]
    public class UserTicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        //to get/pass the User id from the view 
        private readonly UserManager<IdentityUser> _user;


        //inject services in the constructor
        public UserTicketController(ApplicationDbContext context, UserManager<IdentityUser> user)
        {
            _db = context;
            _user = user;
        }

        //Show All Tickets
        public async Task<IActionResult> Index()
        {
            var applicationdbContext = _db.Tickets.Include(t => t.Project);

            return View(await applicationdbContext.ToListAsync());
        }


        //Show only user's Tickets
        public async Task<IActionResult> ShowUsersTickets()
        {
            string LoggedinUserId = (await _user.GetUserAsync(HttpContext.User)).Id;

            var applicationdbContext = _db.Tickets.Where(t => t.UserId == LoggedinUserId).Include(t => t.Project);

            return View(await applicationdbContext.ToListAsync());
        }


        // show ticket details
        public async Task<IActionResult> DetailsAsync(int? id)
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

            // pass TicketID value from action to action method 
            TempData["PassedTicketID"] = TicketVm.Ticket_Id;

            // pass user id value to view
            string CurrentLoginUserId = (await _user.GetUserAsync(HttpContext.User)).Id;
            TempData["PassedUserID"] = CurrentLoginUserId;

            return View(TicketVm);
        }


        // get method of creating a new ticket
        public IActionResult Create()
        {
            //to pass,render the project items that will be displayed in the select html element to the user
            ViewData["ProjectId"] = new SelectList(_db.Projects, "Id", "Name");
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "Email");

            return View();
            
        }

        // create new ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            ticket.UserId = (await _user.GetUserAsync(HttpContext.User)).Id;
            ticket.Status = true;

            // for attaching the files
            var files = HttpContext.Request.Form.Files;
            foreach (var file in files)
            {
                var dbPath = Path.Combine("\\AttachedFiles\\");
                var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\AttachedFiles\\");
                bool basePathExists = System.IO.Directory.Exists(basePath);
                if (!basePathExists) Directory.CreateDirectory(basePath);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = Path.Combine(basePath, file.FileName);
                dbPath = Path.Combine(dbPath, file.FileName);
                var extension = Path.GetExtension(file.FileName);
                if (!System.IO.File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    Models.File t = new Models.File();
                    t.Path = dbPath;

                    ticket.Files.Add(t);
                }
            }
            if (ModelState.IsValid)
            {
                _db.Add(ticket);
                await _db.SaveChangesAsync();
                return RedirectToAction("ShowUsersTickets");
            }

            ViewData["ProjectId"] = new SelectList(_db.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "Id", ticket.UserId);

            return View(ticket);
            
        }

        // add a new comment to the ticket
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

        // close ticket
        public async Task<IActionResult> CloseTicket(Ticket closedTicket)
        {
            if (closedTicket is null)
            {
                throw new ArgumentNullException(nameof(closedTicket));
            }

            closedTicket.Id = Convert.ToInt32(TempData["PassedTicketID"]);
            Ticket updatedTicket = _db.Tickets.Where(t => t.Id == closedTicket.Id).FirstOrDefault();
            updatedTicket.ClosingReason = closedTicket.ClosingReason;
            if (updatedTicket.ClosingReason != null)
            {
                updatedTicket.Status = false;
                _db.Update(updatedTicket);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = updatedTicket.Id });
        }



    }
}
