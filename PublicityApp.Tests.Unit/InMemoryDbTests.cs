using Microsoft.Extensions.Logging;
using Moq;
using PublicityApp.Abstractions;
using PublicityApp.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicityApp.Tests.Unit;

public class InMemoryDbTests
{
    public InMemoryDictionary dictDb { get; init; }
    public InMemoryConcurrentDictionary dictConcurrentDb { get; init; }

    public InMemoryDbTests()
    {
        var mockLoggerDict = new Mock<ILogger<InMemoryDictionary>>();
        dictDb = new InMemoryDictionary(mockLoggerDict.Object);

        var mockLoggerConcurrent = new Mock<ILogger<InMemoryConcurrentDictionary>>();
        dictConcurrentDb = new InMemoryConcurrentDictionary(mockLoggerConcurrent.Object);
    }

    public class LoadAndGetData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var mockLoggerDict = new Mock<ILogger<InMemoryDictionary>>();
            var dictDb = new InMemoryDictionary(mockLoggerDict.Object);

            var mockLoggerConcurrent = new Mock<ILogger<InMemoryConcurrentDictionary>>();
            var dictConcurrentDb = new InMemoryConcurrentDictionary(mockLoggerConcurrent.Object);

            yield return new object[] { dictDb };
            yield return new object[] { dictConcurrentDb };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Theory]
    [ClassData(typeof(LoadAndGetData))]
    public async Task LoadAndGet(IInMemoryDb db)
    {
        var content = @"Яндекс.Директ:/ru
            Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
            Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
            Крутая реклама:/ru/svrd";
        
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        await db.LoadFromStreamAsync(stream);
        writer.Close();

        Assert.Contains("Яндекс.Директ", await db.GetByKeyAsync("/ru"));
        Assert.Contains("Газета уральских москвичей", await db.GetByKeyAsync("/ru/msk"));
        Assert.Contains("Газета уральских москвичей", await db.GetByKeyAsync("/ru/chelobl"));
        Assert.True((await db.GetByKeyAsync("/ru/msk")).Count() == 1);
        Assert.True((await db.GetByKeyAsync("/ru/chelobl")).Count() == 1);
        Assert.True((await db.GetByKeyAsync("/ru/svrd")).Count() == 1);
    }
}
