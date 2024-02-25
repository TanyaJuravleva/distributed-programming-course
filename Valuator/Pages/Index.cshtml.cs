using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    private static bool IsLetter(Char ch)
    {
        if (((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'а') && (ch <= 'я')) || ((ch >= 'А') && (ch <= 'Я')) || (ch == 'Ё') || (ch == 'ё')) 
            return true;
        return false;
    }

    private double GetRank(string text)
    {
        if (String.IsNullOrEmpty(text))
            return 0;

        double numberOfNonAlphabeticCharacters = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (!IsLetter(text[i]))
                numberOfNonAlphabeticCharacters++;
        }
        return numberOfNonAlphabeticCharacters / text.Length;
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        //TODO: сохранить в БД text по ключу textKey
        DatabaseConnector dbConnector = new DatabaseConnector();
        dbConnector.SetValueByKey(textKey, text);

        string rankKey = "RANK-" + id;
        //TODO: посчитать rank и сохранить в БД по ключу rankKey
        double rank = GetRank(text);
        dbConnector.SetValueByKey(rankKey, rank);

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
        double similarity = 0;
        if (dbConnector.isSimilarValueString("TEXT-*", textKey))
            similarity = 1;
        dbConnector.SetValueByKey(similarityKey, similarity);

        return Redirect($"summary?id={id}");
    }
}
