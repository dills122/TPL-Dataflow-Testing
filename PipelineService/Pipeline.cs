using Gridsum.DataflowEx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineService
{
    public class Pipeline : Dataflow<object>, IPipelineService
    {
        private TransformBlock<object, object> _inputBlock;
        private ActionBlock<object> _resultBlock;

        private List<object> _results { get; set; }

        public Pipeline() : base(DataflowOptions.Default)
        {
            _results = new List<object>();

            _inputBlock = new TransformBlock<object, object>(obj => Processing.Processing.ReceiveOrder(obj));
            _resultBlock = new ActionBlock<object>(obj => _results.Add(Processing.Processing.ReturnProcessedOrder(obj)));

            _inputBlock.LinkTo(_resultBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            RegisterChild(_inputBlock);
            RegisterChild(_resultBlock);
        }

        public Task FillPipeline(object inputObj)
        {
            InputBlock.Post(inputObj);
            return Task.CompletedTask;
        }

        public async Task WaitForResults()
        {
            await this.CompletionTask;
            //return _results;
        }

        public Task<List<object>> GetResults()
        {
            return Task.FromResult(_results);
        }

        public Task FlushPipeline()
        {
            _results = new List<object>();
            return Task.CompletedTask;
        }

        public override ITargetBlock<object> InputBlock { get { return _inputBlock; } }

        public object Result { get { return _results; } }
    }
}
