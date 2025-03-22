namespace WhoIsHome.Shared.PushUp;

public interface IPushUpService
{
    Task Created(PushUpCreateCommand command, CancellationToken cancellationToken);
    
    Task Updated(PushUpUpdateCommand command, CancellationToken cancellationToken);
}