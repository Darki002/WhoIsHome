namespace WhoIsHome.External.PushUp;

public interface IPushUpContext
{
    Task PushEventUpdateAsync(PushUpEventUpdateCommand command);
}