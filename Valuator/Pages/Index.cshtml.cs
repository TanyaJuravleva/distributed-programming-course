using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Valuator.Redis;

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

    private double GetRank(string text)
    {
        if (String.IsNullOrEmpty(text))
            return 0;

        double numberOfNonAlphabeticCharacters = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (!Char.IsLetter(text[i]))
                numberOfNonAlphabeticCharacters++;
        }
        return numberOfNonAlphabeticCharacters / text.Length;
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

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        //TODO: сохранить в БД text по ключу textKey
        _dbConnector.SetValueByKey(textKey, text);

        string rankKey = "RANK-" + id;
        //TODO: посчитать rank и сохранить в БД по ключу rankKey
        double rank = GetRank(text);
        _dbConnector.SetValueByKey(rankKey, rank);

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
        double similarity = GetSimilarity("TEXT-*", textKey);
        _dbConnector.SetValueByKey(similarityKey, similarity);

        return Redirect($"summary?id={id}");
    }
}
