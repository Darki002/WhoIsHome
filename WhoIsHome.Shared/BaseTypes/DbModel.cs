using System.ComponentModel.DataAnnotations;

namespace WhoIsHome.Shared.BaseTypes;

public abstract class DbModel
{
    [Key]
    public int Id { get; set; }
}