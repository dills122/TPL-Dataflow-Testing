using StockReportPipeline;
using System;
using Xunit;

namespace StockReportTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("F")]
        [InlineData("XBA")]
        [InlineData("AGFS")]
        [InlineData("FB")]
        public void CompanyInfoTest(string symbol)
        {
            Pipeline pipeline = new Pipeline();

            var compInfo = pipeline.RetrieveCompanyInfo(symbol);

            Assert.True(compInfo != null);
        }

        [Theory]
        [InlineData("F")]
        [InlineData("XBA")]
        [InlineData("AGFS")]
        [InlineData("FB")]
        public void DividendInfoTest(string symbol)
        {
            Pipeline pipeline = new Pipeline();

            var dividendReports = pipeline.RetrieveDividendInfo(symbol);

            Assert.True(dividendReports != null);
        }

        [Theory]
        [InlineData("F")]
        [InlineData("XBA")]
        [InlineData("AGFS")]
        [InlineData("FB")]
        public void KeyStatsTest(string symbol)
        {
            Pipeline pipeline = new Pipeline();

            var keyStats = pipeline.RetrieveKeyStats(symbol);

            Assert.True(keyStats != null);
        }

        [Theory]
        [InlineData("F", 30)]
        [InlineData("XBA",30)]
        [InlineData("AGFS",20)]
        [InlineData("FB", 60)]
        public void IntervalsTest(string symbol, int intervalValue)
        {
            Pipeline pipeline = new Pipeline();

            var intervals = pipeline.RetrieveIntervals(symbol, intervalValue);

            Assert.True(intervals != null);
        }
    }
}
