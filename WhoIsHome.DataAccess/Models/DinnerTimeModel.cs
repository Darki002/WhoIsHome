using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared;
using WhoIsHome.Shared.BaseTypes;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.DataAccess.Models;

[Table("DinnerTime")]
public class DinnerTimeModel : DbModel
{
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }
    
    public TimeOnly? Time { get; set; }
}