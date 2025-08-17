using PublicityApp.Abstractions;

namespace PublicityApp.Infrastructure;

public class InMemoryDictionary : IInMemoryDb
{
    private Dictionary<string, List<string>> db;

    private SemaphoreSlim semaphore;    //gate to control only one thread is writing

    private ILogger<InMemoryDictionary> _logger;
    
    public InMemoryDictionary(ILogger<InMemoryDictionary> logger)
    {
        db = new Dictionary<string, List<string>>();
        semaphore = new SemaphoreSlim(1);
        _logger = logger;
    }
    public async Task<IEnumerable<string>>? GetByKeyAsync(string key)
    {
        //know thats a shitty workaround, but haven't found anything more accurate
        while (semaphore.CurrentCount < 1)
        {
            await Task.Delay(100);
        }

        List<string> result;
        var contains = db.TryGetValue(key, out result);
        
        if (contains)
        {
            _logger.LogDebug($"Key {key} has been found in the dictionnary");
            return result!;
        }

        _logger.LogDebug($"Key {key} has not been found in the dictionnary");
        return default!;
    }

    public async Task LoadFromStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);    //clears out all resources of reader, including stream itself

        int countEntries = 0;
        string? line;

        await semaphore.WaitAsync();    //concurrent writes can destroy our dictionary

        while ((line = await reader.ReadLineAsync()) != null)
        {
            string[] info = line.Split(':');
            string agent = info[0].Trim();
            string[] areas = info[1].Split(',');
            foreach(var area in areas)
            {
                var areaName = area.Trim(); //proper name, to suppress whitespace after ","
                List<string>? agents;
                var contains = db.TryGetValue(area, out agents);

                if (contains)
                {
                    if(!agents.Contains(agent))
                        db[areaName].Add(agent);
                }
                else
                {
                    agents = new List<string>() { agent };
                    db.Add(areaName, agents);
                }
            }
            countEntries++;
        }

        semaphore.Release();

        _logger.LogDebug($"{countEntries} companies has been registred into dictionary");
        _logger.LogInformation("Populating dictionary is now finished");
        return;
    }
}

/* Postman load tests perfomance:
 *      get: 3ms average response time
 *          4ms while loading small files
 */