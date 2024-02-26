using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
        private readonly IRedisConnector _dbConnector;

    public SummaryModel(ILogger<SummaryModel> logger, IRedisConnector dbConnector)
    {
        _logger = logger;
        _dbConnector = dbConnector;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
        Rank = (double)_dbConnector.GetValueByKey("RANK-" + id);
        Similarity = (double)_dbConnector.GetValueByKey("SIMILARITY-" + id);
    }
}
