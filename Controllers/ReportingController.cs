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

        // Check if user exists
        var user = await _db.ZkUsers.FirstOrDefaultAsync(u => u.AccessNumber == ID);
        if (user == null)
        {
            ViewBag.Message = "Invalid ID";
            ViewBag.Checker = false;
            return View();
        }

        var now = DateTimeOffset.Now;

        // Get the most recent log
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

        // Alternate log type: IN → OUT → IN ...
        string nextLogType = recentLog != null && recentLog.LogType == "IN" ? "OUT" : "IN";

        var newLog = new TimeLog
        {
            Id = Guid.NewGuid(),
            AccessNumber = ID,
            TimeLogStamp = now,
            RecordDate = DateTime.Now,
            DateCreated = now,
            IsDeleted = false,
            LogType = nextLogType,
            DeviceSerialNumber = "JHT4243000082",
            VerifyMode = "4",
            Location = "Drop Off",
            Checksum = string.Empty,
            ZkUsers = user
        };

        _db.TimeLogs.Add(newLog);
        await _db.SaveChangesAsync();

        // Mask ID for display
        string visible = Regex.Match(ID, "[0-9]{3}", RegexOptions.RightToLeft).Value;
        string masked = new string('*', ID.Length - visible.Length) + visible;

        ViewBag.Message = $"Time {newLog.LogType} recorded for ID: {masked}<br>Timestamp: {newLog.TimeLogStamp}";
        ViewBag.Checker = true;

        return View();
    }

    [HttpGet]
    public IActionResult Tap()
    {
        return View();
    }
}
