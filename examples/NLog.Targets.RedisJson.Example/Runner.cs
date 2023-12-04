using Microsoft.Extensions.Logging;

namespace NLog.Targets.RedisJson.Example;

public class Runner
{
    private readonly ILogger<Runner> _logger;

    public Runner(ILogger<Runner> logger)
    {
        _logger = logger;
    }

    public void DoAction(string name)
    {
        _logger.LogDebug(20, "Doing hard work! {Action}", name);
    }

    public void DoError(string message)
    {
        try
        {
            throw new ArgumentNullException("something was wrong");

        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, message);
        }


    }
}