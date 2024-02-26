using StackExchange.Redis;

public interface IRedisConnector
{
    void SetValueByKey(string key, RedisValue value);
    RedisValue GetValueByKey(string key);
    List<RedisKey> GetKeysByPattern(string pattern);
}