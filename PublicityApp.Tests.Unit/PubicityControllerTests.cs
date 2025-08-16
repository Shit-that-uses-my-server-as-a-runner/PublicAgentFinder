using Microsoft.Extensions.Logging;
using Moq;
using PublicityFileUploadDemo.Abstractions;
using PublicityFileUploadDemo.Controllers;

namespace PublicityApp.Tests.Unit;

public class PublicityControllerTests
{
    [Fact]
    public async Task GetAgentsDevideLocationStringProperly()
    {
        var requestedLocation = "/fr/idf/yvl/vers";
        var expected = new List<string>() { "/fr/idf/yvl/vers", "/fr/idf/yvl", "/fr/idf", "/fr" };

        var actual = new List<string>();
        var mockDb = new Mock<IInMemoryDb>();
        mockDb.Setup(x => x.GetByKeyAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<IEnumerable<string>>(null))
            .Callback(new InvocationAction(a => actual.Add(a.Arguments[0] as string)));
        var mockLog = new Mock<ILogger<PublicityController>>();

        var controller = new PublicityController(mockLog.Object, mockDb.Object);

        await controller.GetAgents(requestedLocation);

        
        foreach(var call in actual)
        {
            Assert.Contains<string>(call, expected);
            expected.Remove(call);
        }
        Assert.True(expected.Count == 0);
    }
}
