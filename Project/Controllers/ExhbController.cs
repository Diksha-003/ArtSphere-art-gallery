using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using newUser.Data;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

[Authorize(Roles = "Admin,User,Artist")]
public class ExhbController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExhbController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var exhibitions = await _context.exhb.ToListAsync();
        return View(exhibitions);
    }

    // GET: Exhb/Edit/5
    [Authorize(Roles = "Admin")] // Only allow Admin role to edit exhibitions
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound(); // If id is null, return Not Found
        }

        var exhibition = await _context.exhb.FindAsync(id);

        if (exhibition == null)
        {
            return NotFound(); // If exhibition with given id is not found, return Not Found
        }

        return View(exhibition);
    }

    // POST: Exhb/Edit/5
    [HttpPost]
    
    [Authorize(Roles = "Admin")] // Only allow Admin role to edit exhibitions
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Venue,ImageFilePath")] Exhb exhb)
    {
        if (id != exhb.Id)
        {
            return NotFound(); // If id in URL doesn't match id in the posted model, return Not Found
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(exhb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExhibitionExists(exhb.Id))
                {
                    return NotFound(); // If exhibition doesn't exist, return Not Found
                }
                else
                {
                    throw; // Throw exception if there's a concurrency issue
                }
            }
            return RedirectToAction(nameof(Index)); // Redirect to index action after successful edit
        }
        return View(exhb);
    }

    // POST: Exhb/Delete/5
    [HttpPost, ActionName("DeleteExhibition")]
    
    [Authorize(Roles = "Admin")] // Only allow Admin role to delete exhibitions
    public IActionResult DeleteExhibition(int exhibitionId)
    {
        var exhibition = _context.exhb.Find(exhibitionId);

        if (exhibition == null)
        {
            return NotFound(); // Exhibition not found
        }

        _context.exhb.Remove(exhibition);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private bool ExhibitionExists(int exhibitionId)
    {
        return _context.exhb.Any(e => e.Id == exhibitionId);
    }
}
