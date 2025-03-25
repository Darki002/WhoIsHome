using WhoIsHome.External.PushUp;

namespace WhoIsHome.Test.Application;

public class PushUpContextFake : IPushUpContext
{
    public PushUpEventUpdateCommand? Command { get; private set; }
    
    public void PushEventUpdate(PushUpEventUpdateCommand command, CancellationToken cancellationToken)
    {
        Command = command;
    }
}