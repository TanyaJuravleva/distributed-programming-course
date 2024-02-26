using StackExchange.Redis;

public class RedisConnector : IRedisConnector
{
    private IDatabase _db;
    private IServer _server;

    public RedisConnector()
    {
        ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
        _db = connection.GetDatabase();
        _server = connection.GetServer("localhost", 6379);
    }

    public void SetValueByKey(string key, RedisValue value)
    {
        _db.StringSet(key, value);
    }

    public RedisValue GetValueByKey(string key)
    {
        return _db.StringGet(key);
    }

    public List<RedisKey> GetKeysByPattern(string patternStr)
    {
        return _server.Keys(pattern: patternStr).ToList();
    }
};