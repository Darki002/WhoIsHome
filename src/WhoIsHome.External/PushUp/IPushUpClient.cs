namespace WhoIsHome.External.PushUp;

public interface IPushUpClient
{
    void PushEventUpdate(PushUpEventUpdateCommand command, CancellationToken cancellationToken);
}