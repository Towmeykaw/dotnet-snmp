using System;
using System.Collections.Generic;
using Xunit;

namespace SnmpSender.Tests
{
    public class SetCommandTests
    {
        [Fact]
        public void ShouldHaveVariablesOption()
        {
            var command = SetCommand.CreateCommand();
            Assert.True(command.Children.Contains("setVar"));
        }
    }
}
