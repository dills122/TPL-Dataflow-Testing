using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProcessingPrototype;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProcessingPipelineTests
{
    public class FileReaderTest
    {
        [Theory]
        [InlineData(".\\EntryInformation.json")]
        //[InlineData(".\\EntryInformations.json")]
        public async Task TestFileReader(string FilePath)
        {
            Processing processing = new Processing();
            string returnVal = await processing.ReadJson(FilePath);
            Assert.True(string.IsNullOrEmpty(returnVal));
            
        }
    }
}
