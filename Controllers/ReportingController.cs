using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ReportingController : Controller
{
    private readonly AppDbContext _db;

    public ReportingController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Tap(string ID)
{
    if (string.IsNullOrWhiteSpace(ID))
    {
        ViewBag.Message = "Please enter an ID.";
        ViewBag.Checker = false;
        return View();
    }

    var user = await _db.ZkUsers.FirstOrDefaultAsync(u => u.AccessNumber == ID);
    
    var imageFileName = $"{ID}.png";

    var imagePath = Path.Combine("wwwroot", "images", imageFileName);

    if (System.IO.File.Exists(imagePath))
        {
            ViewBag.ImageUrl = $"/images/{imageFileName}";
        }
    else
        {
            // fallback to default image
            ViewBag.ImageUrl = "/images/default.png";
        }
        
    if (user == null)
        {
            ViewBag.Message = "Invalid ID";
            ViewBag.Checker = false;
            return View();
        }

    var now = DateTimeOffset.Now;

    var recentLog = await _db.TimeLogs
        .Where(t => t.AccessNumber == ID)
        .OrderByDescending(t => t.TimeLogStamp)
        .FirstOrDefaultAsync();

    // Prevent repeat taps within 2 minutes
    bool preventRepeat = recentLog != null && (now < recentLog.TimeLogStamp.AddMinutes(2));
    if (preventRepeat)
    {
        ViewBag.Message = "You cannot Tap your ID Multiple Times.";
        ViewBag.Checker = false;
        return View();
    }

    string visible = Regex.Match(ID, "[0-9]{3}", RegexOptions.RightToLeft).Value;
    string masked = new string('*', ID.Length - visible.Length) + visible;

    if (recentLog == null || recentLog.LogType == "OUT" || recentLog.TimeLogStamp.Date != now.Date)
    {
        // Insert new "IN" record
        var newLog = new TimeLog
        {
            Id = Guid.NewGuid(),
            AccessNumber = ID,
            TimeLogStamp = now,
            RecordDate = DateTime.Now,
            DateCreated = now,
            IsDeleted = false,
            LogType = "IN",
            DeviceSerialNumber = "JHT4243000082",
            VerifyMode = "4",
            Location = "Drop Off",
            Checksum = string.Empty,
            ZkUsers = user
        };

        _db.TimeLogs.Add(newLog);
        ViewBag.Message = $"Time IN recorded for ID: {masked}<br>Timestamp: {newLog.TimeLogStamp.DateTime.ToString("dd/MM/yyyy hh:mm:ss tt")}";
    }
    else
    {
        // Update existing "IN" record to "OUT"
        recentLog.TimeLogStamp = now;
        recentLog.LogType = "OUT";
        recentLog.RecordDate = DateTime.Now;

        _db.TimeLogs.Update(recentLog);
        ViewBag.Message = $"Time OUT recorded for ID: {masked}<br>Timestamp: {recentLog.TimeLogStamp.DateTime.ToString("dd/MM/yyyy hh:mm:ss tt")}";
    }

    ViewBag.Checker = true;

    await _db.SaveChangesAsync();

    return View();
}


    [HttpGet]
    public IActionResult Tap()
    {
        return View();
    }
}
