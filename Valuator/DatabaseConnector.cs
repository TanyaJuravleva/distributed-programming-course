using StackExchange.Redis;

public class DatabaseConnector
{
    private IDatabase _db;
    private IServer _server;

    public DatabaseConnector()
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

    public bool isSimilarValueString(string patternKey, string key)
    {
        string value = (string)_db.StringGet(key);
        if (String.IsNullOrEmpty(value))
            return false;
            
        foreach(var keyCheck in _server.Keys(pattern: patternKey)) 
        {
            string valueCheck = (string)_db.StringGet(keyCheck);
            if (valueCheck.Contains(value) && keyCheck != key)
                return true;
        }
        return false;
    }
}