namespace WhoIsHome.External.PushUp;

public interface IPushUpClient
{
    Task Push(PushUpCommand command, CancellationToken cancellationToken);
}