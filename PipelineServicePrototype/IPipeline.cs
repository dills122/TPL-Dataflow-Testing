using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineServicePrototype
{
    public interface IPipeline<T, I> where T : class
    {
        Task Post(T input);
        Task Complete();
        Task Flush();
        Task WaitForResultsAsync();
        Task<List<I>> GetResults();
    }
}
