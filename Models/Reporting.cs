using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ZkUsers")]
public class ZkUsers
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public string? AccessNumber { get; set; }

    public ICollection<TimeLog> TimeLogs { get; set; }
}

[Table("TimeLogs")]
public class TimeLog
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset DateCreated { get; set; } 

    public bool IsDeleted { get; set; }  = false;

    public DateTimeOffset TimeLogStamp { get; set; }

    public DateTime RecordDate { get; set; }

    public string? LogType { get; set; }

    [ForeignKey("ZkUsers")]
    public string? AccessNumber { get; set; }

    public string? DeviceSerialNumber { get; set; }

    public string VerifyMode { get; set; }

    public string? Location { get; set; }

    public string? Checksum { get; set; }

    [Required]
    public ZkUsers ZkUsers { get; set; }
}


