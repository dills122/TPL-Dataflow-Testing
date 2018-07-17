using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessingPipeline.Transforms
{
    public class TransformOrder
    {
        public Order order { get; private set; }

        public TransformOrder(Order order)
        {
            this.order = order;
        }
    }
}
