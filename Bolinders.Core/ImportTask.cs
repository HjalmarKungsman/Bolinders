using Bolinders.Core.Services;
using DNTScheduler.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core
{
    public class ImportTask : IScheduledTask
    {
        private readonly IXmlToDbService _xmlToDb;

        public ImportTask(IXmlToDbService xmlToDb)
        {
            _xmlToDb = xmlToDb;
        }

        public bool IsShuttingDown { get; set; }

        public async Task RunAsync()
        {
            if (this.IsShuttingDown)
            {
                return;
            }

            await _xmlToDb.Run();

            await Task.Delay(TimeSpan.FromMinutes(3)).ConfigureAwait(false);
        }
    }
}
