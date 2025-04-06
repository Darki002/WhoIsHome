using WhoIsHome.External.PushUp;

namespace WhoIsHome.Test.Application;

public class PushUpContextFake : IPushUpContext
{
    public PushUpEventUpdateCommand? Command { get; private set; }

    public async Task PushEventUpdateAsync(PushUpEventUpdateCommand command)
    {
        Command = command;
    }
}