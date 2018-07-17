using OrderProcessingPipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPipelineTets
{
    public class TestPipeline
    {
        Pipeline Pipeline;

        public void TestPipelineComplete()
        {
            Pipeline = new Pipeline();
            Order order = new Order().RandomOrder();

            Pipeline.StartPipeline();
        }
    }
}
