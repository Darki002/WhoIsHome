﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.BaseTypes;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.External.Models;

[Table("RepeatedEvent")]
public class RepeatedEventModel : DbModel
{
    [Required] 
    [MaxLength(50)] 
    public string Title { get; set; } = null!;
    
    [Required]
    public DateOnly FirstOccurrence { get; set; }
    
    public DateOnly? LastOccurrence { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly? EndTime { get; set; }
    
    [Required]
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }
    
    public TimeOnly? DinnerTime { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}