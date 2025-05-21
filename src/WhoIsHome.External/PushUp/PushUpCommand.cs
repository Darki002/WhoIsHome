using WhoIsHome.External.Translation;

namespace WhoIsHome.External.PushUp;

public sealed record PushUpCommand(TranslatableString Title, TranslatableString Body, int[] UserIds);