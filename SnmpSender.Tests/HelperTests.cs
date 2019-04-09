using Lextm.SharpSnmpLib;
using System;
using System.Collections.Generic;
using Xunit;

namespace SnmpSender.Tests
{
    public class HelperTests
    {
        [Fact]
        public void GivenIntShouldReturnVersion()
        {
            Assert.Equal(VersionCode.V3, SnmpHelpers.GetVersion(3));
        }
        [Fact]
        public void GivenInvalidIntShouldReturnVersion2()
        {
            Assert.Equal(VersionCode.V2, SnmpHelpers.GetVersion(50));
        }
        [Theory]
        [InlineData("1.2.3.4,s,hello")]
        [InlineData("1.2.3.4,s,he,llo")]
        [InlineData("1.2.3.4,i,100")]
        [InlineData("1.2.3.4,u,100")]

        public void ShouldParseVariablesWithDifferentTypes(string data)
        {
            var ls = new List<string>() { data };
            var parsedData = SnmpHelpers.ParseVariables(ls);
            Assert.NotEmpty(parsedData);
        }
        [Theory]
        [InlineData("1.2.3.4")]
        [InlineData("1.2.3.4,i,hello")]
        [InlineData("1.2.3.4,i")]
        public void ShouldThrowExceptionWhenIncorrectVariables(string data)
        {
            var ls = new List<string>() { data };
            Assert.Throws<ArgumentException>(() => SnmpHelpers.ParseVariables(ls));
        }
    }
}
