using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WhoIsHome.DataAccess.Models;

public class DinnerTime
{
    [Key]
    public int Id { get; set; }

    [DefaultValue(PresentsType.Unknown)]
    public PresentsType PresentsType { get; set; }
    
    public TimeOnly? Time { get; set; }
}

public enum PresentsType
{
    Unknown,
    Default,
    Late,
    NotPresent
}