using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public class ReportingController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppDbContext _db;

    public ReportingController(AppDbContext db)
    {
        _db = db;
        _context = db;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tap(string ID)
    {
        if (string.IsNullOrWhiteSpace(ID))
        {
            ViewBag.Message = "Please enter an ID.";
            return View();
        }

        bool isDummy = false;

        var record = _db.Reporting.FirstOrDefault(r => r.ID == ID);
        if (record == null)
        {
            record = _db.Reporting.FirstOrDefault(r => r.ID == "NULL");

            ViewBag.Message = "Invalid ID";
            ViewBag.Checker = false;
        }

        ViewBag.ImageUrl = Url.Action("GetImage", "Reporting", new { id = record.ID });

        if (record.ID != "NULL")
        {
            if (record.IsIn == true && DateTime.Now < record.TimeIn.Value.AddMinutes(2))
            {
                ViewBag.Message = "You cannot Tap your ID Multiple Times";
                ViewBag.Checker = false;
                return View();
            }
            else if (record.IsIn == false && DateTime.Now < record.TimeIn.Value.AddMinutes(2))
            {
                ViewBag.Message = "You cannot Tap your ID Multiple Times";
                ViewBag.Checker = false;
                return View();
            }
        }

        string Visible = Regex.Match(record.ID, "[0-9]{3}", RegexOptions.RightToLeft).Value;
        int Length = record.ID.Length - Visible.Length;
        string Masking = new string('*', Length);
        string Display = Masking + Visible;

        record.TimeIn = DateTime.Now;

        _db.Update(record);
        await _db.SaveChangesAsync();
        if (record.ID == "NULL")
        {
            ViewBag.Message = "Invalid ID";
            return View();
        }
        else
        {
            ViewBag.Message = $"Updated record for ID: {Display} <br> Time In: {record.TimeIn}";
            return View();
        }
    }

    [HttpGet]
    public IActionResult Tap()
    {
        return View();
    }

    public IActionResult GetImage(string id)
    {
        var record = _db.Reporting.FirstOrDefault(r => r.ID == id);
        if (record?.Picture != null)
        {
            return File(record.Picture, "image/jpeg");
        }

        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blank-picture.png");
        var defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
        return File(defaultImageBytes, "image/png"); // No fallback or default image
    }
}

