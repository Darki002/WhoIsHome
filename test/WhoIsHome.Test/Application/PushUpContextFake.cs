using WhoIsHome.External.PushUp;

namespace WhoIsHome.Test.Application;

public class PushUpContextFake : IPushUpContext
{
    public PushUpCommand? Command { get; private set; }

    public async Task PushEventUpdateAsync(PushUpCommand command)
    {
        Command = command;
    }
}