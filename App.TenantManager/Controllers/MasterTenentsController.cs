using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Master.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.TenantManager.Controllers
{
    [Authorize]
    public class MasterTenentsController : Controller
    {
        private readonly TenantMasterDbContext _context;

        public MasterTenentsController(TenantMasterDbContext context)
        {
            _context = context;
        }

        // GET: MasterTenents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tenants.ToListAsync());
        }

        // GET: MasterTenents/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTenent = await _context.Tenants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (masterTenent == null)
            {
                return NotFound();
            }

            return View(masterTenent);
        }

        // GET: MasterTenents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MasterTenents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Hostname,ConnectionString")] MasterTenent masterTenent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(masterTenent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(masterTenent);
        }

        // GET: MasterTenents/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTenent = await _context.Tenants.FindAsync(id);
            if (masterTenent == null)
            {
                return NotFound();
            }
            return View(masterTenent);
        }

        // POST: MasterTenents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Hostname,ConnectionString")] MasterTenent masterTenent)
        {
            if (id != masterTenent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(masterTenent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MasterTenentExists(masterTenent.Id))
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
            return View(masterTenent);
        }

        // GET: MasterTenents/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTenent = await _context.Tenants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (masterTenent == null)
            {
                return NotFound();
            }

            return View(masterTenent);
        }

        // POST: MasterTenents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var masterTenent = await _context.Tenants.FindAsync(id);
            _context.Tenants.Remove(masterTenent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MasterTenentExists(string id)
        {
            return _context.Tenants.Any(e => e.Id == id);
        }
    }
}
