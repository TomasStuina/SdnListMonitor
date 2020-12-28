using SdnListMonitor.Core.Abstractions.Configuration;
using Shouldly;
using Xunit;

namespace SdnListMonitor.Core.Abstractions.Tests.Configuration
{
    public class SdnMonitorOptionsTests
    {
        [Fact]
        public void MonitoringInterval_WhenValueNotSet_ShouldReturn60SecondsByDefault ()
        {
            // Act & Assert
            new SdnMonitorOptions ().MonitoringInterval.TotalSeconds.ShouldBe (60);
        }
    }
}
