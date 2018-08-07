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

        public Order()
        {
            OrderTime = DateTime.Now;
            CreateOrderId();
        }
        public Order(OrderType orderType)
        {
            OrderTime = DateTime.Now;
            RandomOrder(orderType);
        }

        public void RandomOrder(OrderType orderType)
        {
            const int NAMES = 5;
            string[] firstNames = { "John", "Jane", "Joe", "Jen", "Bob", "Beth" };
            string[] lastNames = { "Smith", "Stone", "Doe", "Miller", "Davis", "Wilson" };
            string[] guarantorNames = { "Sallie Mae", "Citizens Bank", "Ally Bank", "USAA", "Wells Fargo" };

            Random random = new Random();
            this.orderType = orderType;

            ClientFName = firstNames[random.Next(NAMES)];
            ClientLName = lastNames[random.Next(NAMES)];

            ClientCreditScore = random.Next(400, 850);

            GuarantorName = guarantorNames[random.Next(NAMES)];

            LoanTotal = random.Next(10000, 4000000);
            LoanDownPayment = random.Next(0, (int)(LoanTotal * (decimal).50));
            LoanFees = LoanTotal * (decimal).10;
            LoanTotal += LoanFees;

            OutputType = random.Next(550) % 2 == 0 ? "Xml" : "Json";

            CreateOrderId();
        }
        private void CreateOrderId()
        {
            Random random = new Random();
            OrderId = random.Next(100000000, 999999999);
        }
    }
}
