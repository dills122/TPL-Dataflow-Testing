using System;
using System.Collections.Generic;
using System.Text;

namespace StockReportPipeline.Models
{
    public class Report
    {
        public List<Dividend> dividends { get; set; }
        public List<decimal> changeIntervals { get; set; }
        public KeyStats keyStats { get; set; }

        public Report()
        {
            dividends = new List<Dividend>();
            changeIntervals = new List<decimal>();
            keyStats = new KeyStats();
        }
    }
}
