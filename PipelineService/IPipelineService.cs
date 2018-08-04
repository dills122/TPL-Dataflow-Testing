using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineService
{
    public interface IPipelineService
    {
        Task FillPipeline(object inputObj);

        Task WaitForResults();

        Task<List<object>> GetResults();

        Task FlushPipeline();
    }
}
