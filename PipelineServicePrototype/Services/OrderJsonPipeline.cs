using Gridsum.DataflowEx;
using PipelineServicePrototype.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineServicePrototype.Services
{
    public class OrderJsonPipeline : Dataflow<Order>, IPipeline<Order, Response>
    {
        private TransformBlock<Order, Order> _inputBlock;
        private ActionBlock<Order> _resultBlock;

        public override ITargetBlock<Order> InputBlock { get { return _inputBlock; } }

        public List<Response> Result { get { return _results; } }

        private List<Response> _results { get; set; }

        public OrderJsonPipeline() : base(DataflowOptions.Default)
        {
            _results = new List<Response>();
        }

        public Task Flush()
        {
            _results = new List<Response>();
            return Task.CompletedTask;
        }

        public Task<List<Response>> GetResults()
        {
            return Task.FromResult(Result);
        }

        public Task Post(Order input)
        {
            InputBlock.Post(input);
            return Task.CompletedTask;
        }

        public async Task WaitForResultsAsync()
        {
            await this.CompletionTask;
        }

        Task IPipeline<Order, Response>.Complete()
        {
            InputBlock.Complete();
            return Task.CompletedTask;
        }
    }
}
