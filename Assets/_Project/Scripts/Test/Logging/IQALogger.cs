#if TEST || ENABLE_QA_LOGGING
using System;
using System.Threading.Tasks;

namespace Test.Logging
{
    public interface IQALogger :
        IDisposable
    {
        public Task InitializeAsync();
        
        public Task LogQaAsync(
            string prompt,
            string responseText,
            bool isSuccess,
            long? overallLatency = null,
            string errorMessage = null);
    }
}
#endif