using Microsoft.Extensions.Options;
using System.Text;
using TrueCodeTask.Interfaces;
using TrueCodeTask.Models;

namespace TrueCodeTask.Services.FileReaders
{
    public class TxtFileReaderService : IStreamReaderService
    {
        private readonly MemoryStream _stream;
        private readonly IOptions<StreamReaderSettings> _readerSettings;
        private readonly ILogger<TxtFileReaderService> _logger;

        public TxtFileReaderService(IOptions<StreamReaderSettings> readerSettings,
            ILogger<TxtFileReaderService> logger, MemoryStream stream = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _readerSettings = readerSettings ?? throw new ArgumentNullException(nameof(readerSettings));
        }

        public string GetFileContent()
        {
            using var reader = new StreamReader(_stream);
            var maxSize = _readerSettings.Value?.MaxBufferSize;
            var partialReading = _readerSettings.Value?.PartialReading;
            var lastSymbol = _readerSettings.Value?.LastMessageSymbol;
            try
            {
                if (maxSize.HasValue && _stream.Length <= maxSize.Value)
                {
                    return ReadAllFromFile(reader, lastSymbol);
                }
                if (maxSize.HasValue && _stream.Length > maxSize.Value && partialReading.HasValue && partialReading.Value)
                {
                    _logger.LogWarning($"Reading a file which size greater than program settings size. The file size {_stream.Length} " +
                       $"and your settings size {maxSize.Value}");
                    return GetAllowedSizeContentFile(reader, maxSize.Value, lastSymbol);
                }
                if (maxSize.HasValue && _stream.Length > maxSize.Value && partialReading.HasValue && !partialReading.Value)
                {
                    _logger.LogWarning($"Reading a file which size greater than program settings size. The file size {_stream.Length} " +
                     $"and your settings size {maxSize.Value} without partial reading mode");
                    return lastSymbol.HasValue ? lastSymbol.Value.ToString() : "";
                }
                if (!maxSize.HasValue)
                {
                    _logger.LogWarning($"Reading a file without size restrictions with size {_stream.Length}");
                    return ReadAllFromFile(reader, lastSymbol);
                }
            }
            catch (OutOfMemoryException ex)
            {
                _logger.LogError($"Out of Memory error | Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, $"IO Reading error | Error: {ex.Message}");
            }

            return "";
        }

        private string GetAllowedSizeContentFile(StreamReader reader, int maxSize, char? lastMessageSymbol)
        {
            int currentSize = 0;
            var sb = new StringBuilder();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var lineSize = Encoding.Unicode.GetByteCount(line);
                currentSize += lineSize;
                if (currentSize < maxSize)
                {
                    sb.AppendLine(line);
                }
                else
                {
                    if (lastMessageSymbol.HasValue && line[line.Length - 1] != lastMessageSymbol.Value)
                    {
                        sb.Remove(sb.Length-2, 2); // new line symbols
                        sb.Append(lastMessageSymbol.Value);
                    }
                    break;
                }
            }


            return sb.ToString();
        }

        private string ReadAllFromFile(StreamReader reader, char? lastMessageSymbol)
        {
            var allContent = reader.ReadToEnd();
            _logger.LogInformation($"All file with size {_stream.Length} was read succesfully");
            if (lastMessageSymbol.HasValue && allContent[allContent.Length - 1] != lastMessageSymbol.Value)
            {
                allContent = string.Concat(allContent, lastMessageSymbol.Value);
            }
            return allContent;
        }
    }
}
