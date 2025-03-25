using WhoIsHome.External.PushUp;

namespace WhoIsHome.Test.Application;

public class PushUpClientFake : IPushUpClient
{
    public PushUpEventUpdateCommand? Command { get; private set; }
    
    public void PushEventUpdate(PushUpEventUpdateCommand command, CancellationToken cancellationToken)
    {
        Command = command;
    }
}