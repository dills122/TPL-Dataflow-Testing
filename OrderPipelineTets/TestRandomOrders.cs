using OrderProcessingPipeline;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OrderPipelineTets
{
    public class TestRandomOrders
    {
        [Fact]
        public void TestRandomObjectCreation()
        {
            Order order = new Order().RandomOrder();

            Assert.True(order != null);
        }
    }
}
