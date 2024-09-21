using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared;

namespace WhoIsHome.DataAccess.Models;

[Table("DinnerTime")]
public class DinnerTimeModel : DbModel
{
    [DefaultValue(PresentsType.Unknown)]
    public PresentsType PresentsType { get; set; }
    
    public TimeOnly? Time { get; set; }
}