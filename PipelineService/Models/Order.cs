using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineService.Models
{
    public enum OrderType
    {
        HomeLoan,
        CarLoan,
        PersonalLoan,
        OtherLoan
    }
    public class Order
    {
        public int OrderId { get; set; }
        public string ClientFName { get; set; }
        public string ClientLName { get; set; }
        public OrderType orderType { get; set; }
        public string GuarantorName { get; set; }
        public DateTime OrderTime { get;  set; }
        public int ClientCreditScore { get; set; }
        public decimal LoanTotal { get; set; }
        public decimal LoanDownPayment { get; set; }
        public decimal LoanFees { get; set; }

        public string OutputType { get; set; }
        public string OutputMethod { get; set; }
        public string OutputLocation { get; set; }

        public bool Approval { get; set; }

        public object Reject()
        {
            this.Approval = false;
            return this;
        }
    }
}
