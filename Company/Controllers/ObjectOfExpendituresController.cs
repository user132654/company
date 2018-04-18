using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Company.Data;
using Microsoft.AspNetCore.Identity;
using Company.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Company.Models.BusinessLogic;

namespace Company.Controllers
{
    public class ObjectOfExpendituresController : Controller
    {
        private readonly CompanyContext _context;

        public ObjectOfExpendituresController(CompanyContext context)
        {
            _context = context;

        }

        // GET: ObjectOfExpenditures
        public async Task<IActionResult> Index()
        {
            var companyContext = _context.ObjectOfExpenditures.Include(o => o.Material).Include(o => o.Provider);
            return View(await companyContext.ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Менеджер")]
        public IActionResult CountMonth()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name");
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Менеджер")]
        public async Task<IActionResult> CountMonth(CountMonthFormModel model)
        {
            if (ModelState.IsValid)
            {
                var ObjectsOfExpenditureMatched = _context.ObjectOfExpenditures
                                                          .AsNoTracking()
                                                          .Where(e => (e.Date >= model.From && e.Date <= model.To
                                                                        && e.MaterialId == model.MaterialId
                                                                        && e.ProviderId == model.ProviderId));

                int OverallAmount = 0;
                await ObjectsOfExpenditureMatched
                    .ForEachAsync(e => OverallAmount += e.Amount);

                var ViewModel = new CountMonthReportViewModel
                {
                    OverallAmount = OverallAmount,
                    ObjectOfExpenditures = await ObjectsOfExpenditureMatched
                                                .Include(o => o.Material).Include(o => o.Provider)
                                                .ToListAsync()

                };

                return View("CountMonthReport", ViewModel);
            }

            return NotFound();
        }
        // GET: ObjectOfExpenditures/Create

        [Authorize(Roles = "Учётчик")]
        public IActionResult Create()
        {
            
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name");
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Учётчик")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaterialId,Amount,ProviderId")] ObjectOfExpenditure objectOfExpenditure)
        {
            objectOfExpenditure.Date = DateTime.Now;
            objectOfExpenditure.UserId = User.Identity.Name;
            if (ModelState.IsValid)
            {
                _context.Add(objectOfExpenditure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name", objectOfExpenditure.Provider);
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name", objectOfExpenditure.ProviderId);
            return View(objectOfExpenditure);
        }

        [Authorize(Roles = "Менеджер")]
        // GET: ObjectOfExpenditures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectOfExpenditure = await _context.ObjectOfExpenditures
                .Include(o => o.Material)
                .Include(o => o.Provider)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (objectOfExpenditure == null)
            {
                return NotFound();
            }

            return View(objectOfExpenditure);
        }

        // POST: ObjectOfExpenditures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectOfExpenditure = await _context.ObjectOfExpenditures.SingleOrDefaultAsync(m => m.Id == id);
            _context.ObjectOfExpenditures.Remove(objectOfExpenditure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjectOfExpenditureExists(int id)
        {
            return _context.ObjectOfExpenditures.Any(e => e.Id == id);
        }
    }
}
