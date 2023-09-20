namespace Epoche.BlockchainClients;
class RateLimiter
{
    readonly object LockObject = new();
    TimeSpan minInterval;
    DateTime NextCall;

    public TimeSpan MinInterval
    {
        get { lock (LockObject) { return minInterval; } }
        set
        {
            lock (LockObject)
            {
                minInterval = value;
                var max = DateTime.UtcNow.Add(MinInterval);
                if (NextCall > max)
                {
                    NextCall = max;
                }
            }
        }
    }

    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TimeSpan wait;
            lock (LockObject)
            {
                if (MinInterval == TimeSpan.Zero) { return; }
                wait = NextCall - DateTime.UtcNow;
                if (wait <= TimeSpan.Zero)
                {
                    NextCall = DateTime.UtcNow.Add(MinInterval);
                    return;
                }
            }
            await Task.Delay(wait, cancellationToken);
        }
    }
}
