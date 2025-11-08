namespace WhoIsHome.Validations;

public record ValidationResult<TResult>
{
    public TResult? Value { get; set; }

    public List<ValidationError> ValidationErrors { get; private init; } = [];
    
    public bool HasErrors => ValidationErrors.Count > 0;

    public TResult Result => Value!;

    public static ValidationResult<TResult> Success(TResult result) => new() { Value = result };

    public static ValidationResult<TResult> Error(string message)
    {
        return new ValidationResult<TResult>
        {
            ValidationErrors = [new ValidationError(message)]
        };
    }
}