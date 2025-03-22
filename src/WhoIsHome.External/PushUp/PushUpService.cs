using WhoIsHome.Shared.PushUp;

namespace WhoIsHome.External.PushUp;

public class PushUpService : IPushUpService
{
    public Task Created(PushUpCreateCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Updated(PushUpUpdateCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}