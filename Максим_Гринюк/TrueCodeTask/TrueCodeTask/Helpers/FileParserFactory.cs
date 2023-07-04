using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrueCodeTask.Interfaces;
using TrueCodeTask.Models;
using TrueCodeTask.Services.FileReaders;

namespace TrueCodeTask.Helpers
{
    public class FileParserFactory
    {
        private readonly IServiceProvider _provider;
        public FileParserFactory(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(IServiceProvider));
        }

        public IStreamReaderService GetFileParser(MemoryStream stream, string filtName)
        {
            var splitName = filtName.Split('.');
            var options = _provider.GetService<IOptions<StreamReaderSettings>>();
            if (splitName.Length == 2)
            {
                switch (splitName[1])
                {
                    case "txt":
                    {
                        var txtLooger = _provider.GetService<ILogger<TxtFileReaderService>>();
                        return new TxtFileReaderService(options, txtLooger, stream);
                    }
                    default:
                    {
                        return null;
                    }
                }
            } 
            else
            {
                var txtLooger = _provider.GetService<ILogger<TxtFileReaderService>>();
                return new TxtFileReaderService(options, txtLooger, stream);
            }
        }
    }
}
