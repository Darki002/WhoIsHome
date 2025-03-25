namespace WhoIsHome.External.PushUp;

public interface IPushUpContext
{
    void PushEventUpdate(PushUpEventUpdateCommand command, CancellationToken cancellationToken);
}