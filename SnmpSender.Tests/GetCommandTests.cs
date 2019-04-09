using Xunit;

namespace SnmpSender.Tests
{
    public class GetCommandTests
    {        
        [Theory]
        [InlineData("port")]
        [InlineData("community")]
        [InlineData("version")]
        [InlineData("address")]
        public void ShouldHaveCommonOptions(string option)
        {
            var command = GetCommand.CreateCommand();
            Assert.True(command.Children.Contains(option));
        }
    }
 }
