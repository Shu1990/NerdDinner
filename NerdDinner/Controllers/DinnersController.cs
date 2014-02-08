using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    public class DinnersController : Controller
    {
        private DinnerRepository dinnerRepository = new DinnerRepository();

        //
        // GET: /Dinners/
        public ActionResult Index(int? page)
        {
            const int pageSize = 1;

            var upcomingDinners = dinnerRepository.FindUpcomingDinners();
            var paginatedDinners = new PaginatedList<Dinner>(upcomingDinners,
                                                            page ?? 0,
                                                            pageSize);

            return View(paginatedDinners);
        }

        //
        // GET: /Dinners/Details/2
        public ActionResult Details(int id)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);

            if (dinner == null)
                return View("NotFound");
            
            return View(dinner);
        }


        //
        // GET: /Dinners/Edit/2
        [Authorize]
        public ActionResult Edit(int id)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);

            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            ViewData["Countries"] = new SelectList(PhoneValidator.Countries,dinner.Country);

            return View(dinner);
        }

        //
        // POST: /Dinners/Edit/2
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Edit(int id, FormCollection formValues)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);

            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            try
            {
                UpdateModel(dinner);
                dinnerRepository.Save();
                return RedirectToAction("Details", new { id = dinner.DinnerID });
            }
            catch
            {
                ModelState.AddRuleViolations(dinner.GetRuleViolations());

                ViewData["countries"] = new SelectList(PhoneValidator.Countries,dinner.Country);

                return View(dinner);
            }
        }


        //
        // GET: /Dinners/Create
        [Authorize]
        public ActionResult Create()
        {
            Dinner dinner = new Dinner()
            {
                EventDate = DateTime.Now.AddDays(7)
            };

            ViewData["Countries"] = new SelectList(PhoneValidator.Countries, dinner.Country);

            return View(dinner);
        }

        //
        // POST: /Dinners/Create
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Create(Dinner dinner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dinner.HostedBy = User.Identity.Name;
                    RSVP rsvp = new RSVP();
                    rsvp.AttendeeName = User.Identity.Name;
                    dinner.RSVPs.Add(rsvp);
                    dinnerRepository.Add(dinner);
                    dinnerRepository.Save();
                    return RedirectToAction("Details", new { id = dinner.DinnerID });
                }
                catch
                {
                    ModelState.AddRuleViolations(dinner.GetRuleViolations());
                    ViewData["Countries"] = new SelectList(PhoneValidator.Countries, dinner.Country);
                }
            }
            return View(dinner);
        }

        //
        // HTTP GET: /Dinners/Delete/1
        public ActionResult Delete(int id)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);
            if (dinner == null)
                return View("NotFound");
            else
                return View(dinner);
        }

        //
        // HTTP POST: /Dinners/Delete/1
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, string confirmButton)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);
            if (dinner == null)
                return View("NotFound");
            dinnerRepository.Delete(dinner);
            dinnerRepository.Save();
            return View("Deleted");
        }

    }

    public static class ControllerHelpers
    {
        public static void AddRuleViolations(this ModelStateDictionary modelState,
        IEnumerable<RuleViolation> errors)
        {
            foreach (RuleViolation issue in errors)
            {
                modelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
            }
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int) Math.Ceiling(TotalCount/(double) PageSize);
            this.AddRange(source.Skip(PageIndex*PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
