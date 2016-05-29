using System;
using CavemanTools.Logging;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class BoostrapTests
    {
        private MyConfig _sut;


        public BoostrapTests(ITestOutputHelper o)
        {
            TestConfigs.Tasks.Clear();
            LogManager.OutputTo(o.WriteLine);
            _sut = StartIt.Up<MyConfig>();
        }

      
        [Fact]
        public void logging()
        {
           _sut.Result.ToString().Should().Be("log");
            _sut.Settings.Should().BeTrue();
        }


        [Fact]
        public void tasks_are_executed_in_proper_order()
        {
           
            
            TestConfigs.Tasks.ShouldAllBeEquivalentTo(new[] {typeof(ConfigureLogging),typeof(ConfigTask_1_Second),typeof(ConfigTask_2_First) });
        }

      
    }
}