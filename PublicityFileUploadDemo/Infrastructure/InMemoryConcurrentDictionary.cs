using PublicityFileUploadDemo.Abstractions;
using System.Collections.Concurrent;
using System.Threading;

namespace PublicityFileUploadDemo.Infrastructure;

public class InMemoryConcurrentDictionary : IInMemoryDb
{
    private ConcurrentDictionary<string, List<string>> db;
    private ConcurrentDictionary<string, object> lockers;

    private ILogger<InMemoryConcurrentDictionary> _logger;

    public InMemoryConcurrentDictionary(ILogger<InMemoryConcurrentDictionary> logger)
    {
        db = new ConcurrentDictionary<string, List<string>>();
        lockers = new ConcurrentDictionary<string, object>();

        _logger = logger;
    }
    public async Task<IEnumerable<string>>? GetByKeyAsync(string key)
    {
        List<string> result;

        //here we don't have to use lockers, as the operation of reading is by fact
        //  retreiving a link to a list, without modifying it
        //  however, if writing a library, may be better to copy its contents, rather than
        //  returning it by link, as the user of db can than do whatever he wants with it
        if(db.TryGetValue(key, out result))
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

        while ((line = await reader.ReadLineAsync()) != null)
        {
            string[] info = line.Split(':');
            string agent = info[0].Trim();
            string[] areas = info[1].Split(',');
            foreach (var area in areas)
            {
                var areaName = area.Trim(); //proper name, to suppress whitespace after ","

                object locker = lockers.GetOrAdd(area, new object());

                lock (locker)
                {
                    db.AddOrUpdate(areaName,
                    new List<string>() { agent },
                    (_, agents) =>
                    {
                        if (!agents.Contains(agent))
                            agents.Add(agent);
                        return agents;
                    });
                }
            }
            countEntries++;
        }

        _logger.LogDebug($"{countEntries} companies has been registred into dictionary");
        _logger.LogInformation("Populating dictionary is now finished");
    }
}

/* Postman load tests perfomance:
 *      get: 3ms average response time
 *          No changes while loading small-size files
 */