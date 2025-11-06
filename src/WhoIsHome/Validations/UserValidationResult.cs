using WhoIsHome.Entities;

namespace WhoIsHome.Validations;

public record UserValidationResult
{
    public User? User { get; set; }

    public List<ValidationError> ValidationErrors { get; } = [];
    
    public bool HasErrors => ValidationErrors.Count > 0;

    public User Value => User!;
}