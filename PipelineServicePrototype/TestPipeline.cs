using Gridsum.DataflowEx;
using PipelineServicePrototype.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineServicePrototype
{
    public class TestPipeline : Dataflow<User>, IPipeline<User, Response>
    {
        private TransformBlock<User, User> _inputBlock;
        private ActionBlock<User> _resultBlock;

        public override ITargetBlock<User> InputBlock { get { return _inputBlock; } }

        public List<User> Result { get { return _results; } }

        private List<User> _results { get; set; }

        public TestPipeline() : base(DataflowOptions.Default)
        {
            _results = new List<User>();

            _inputBlock = new TransformBlock<User, User>(obj => ProccessUser(obj));
            _resultBlock = new ActionBlock<User>(obj => _results.Add(PrintFinalInfo(obj)));

            _inputBlock.LinkTo(_resultBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            RegisterChild(_inputBlock);
            RegisterChild(_resultBlock);
        }

        private User ProccessUser(User user)
        {
            user.Active = true;
            user.Created = DateTime.Now;
            return user;
        }

        private User PrintFinalInfo(User user)
        {
            Console.WriteLine("User finished setup: {0}", user.Created);
            Console.WriteLine("Username : {0}", user.Username);
            Console.WriteLine("Name: {0}", user.Name);
            return user;
        }

        public Task Complete()
        {
            InputBlock.Complete();
            return Task.CompletedTask;
        }

        public Task Post(User input)
        {
            InputBlock.Post(input);
            return Task.CompletedTask;
        }

        public async Task WaitForResultsAsync()
        {
            var test = this.CompletionTask.Status;
            await this.CompletionTask;
        }

        public Task Flush()
        {
            _results = new List<User>();
            return Task.CompletedTask;
        }

        public Task<List<User>> GetResults()
        {
            return Task.FromResult(_results);
        }

        Task<List<Response>> IPipeline<User, Response>.GetResults()
        {
            throw new NotImplementedException();
        }
    }
}
