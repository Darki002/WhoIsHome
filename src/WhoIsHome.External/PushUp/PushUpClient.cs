using WhoIsHome.External.PushUp.ApiClient;

namespace WhoIsHome.External.PushUp;

public class PushUpClient(PushApiClient client) : IPushUpClient
{
    public Task Push(PushUpCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}