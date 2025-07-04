using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Reporting
{
    [Key]
    public string? ID { get; set; }

    [DisplayName("BED or TED")]
    public string? Category { get; set; }

    [DisplayName("Time In/Out")]
    public DateTime? TimeIn { get; set; }

    public string? Status { get; set; }

    [DisplayName("Is In/Out")]
    public bool IsIn { get; set; }

    [DisplayName("ID Picture")] 
    public byte[]? Picture { get; set; }
}
