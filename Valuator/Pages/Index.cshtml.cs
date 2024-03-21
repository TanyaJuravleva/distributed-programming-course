using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisConnector _dbConnector;

    public IndexModel(ILogger<IndexModel> logger, IRedisConnector dbConnector)
    {
        _logger = logger;
        _dbConnector = dbConnector;
    }

    public void OnGet()
    {

    }

    private double GetSimilarity(string patternKey, string key)
    {
        if (IsSimilarValueString(patternKey, key))
            return 1;
        return 0;
    }

    public bool IsSimilarValueString(string patternKey, string key)
    {
        string value = (string)_dbConnector.GetValueByKey(key);
        if (String.IsNullOrEmpty(value))
            return false;

        foreach(var keyCheck in _dbConnector.GetKeysByPattern(patternKey)) 
        {
            string valueCheck = (string)_dbConnector.GetValueByKey(keyCheck);
            if (String.Equals(valueCheck, value) && keyCheck != key)
                return true;
        }
        return false;
    }

    static async Task ProduceAsync(CancellationToken ct, string id)
    {
        ConnectionFactory cf = new ConnectionFactory();

        using (IConnection c = cf.CreateConnection())
        {
            byte[] data = Encoding.UTF8.GetBytes(id);
            c.Publish("valuator.processing.rank", data);
            await Task.Delay(1000);
            c.Drain();

            c.Close();
        }
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        //TODO: сохранить в БД text по ключу textKey
        _dbConnector.SetValueByKey(textKey, text);

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
        double similarity = GetSimilarity("TEXT-*", textKey);
        _dbConnector.SetValueByKey(similarityKey, similarity);

        // string rankKey = "RANK-" + id;
        // //TODO: посчитать rank и сохранить в БД по ключу rankKey
        // double rank = GetRank(text);
        // _dbConnector.SetValueByKey(rankKey, rank);
        CancellationTokenSource cts = new CancellationTokenSource();
        ProduceAsync(cts.Token, id);
        cts.Cancel();

        return Redirect($"summary?id={id}");
    }
}
