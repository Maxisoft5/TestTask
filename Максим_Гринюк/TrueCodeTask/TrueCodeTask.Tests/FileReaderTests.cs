using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using TrueCodeTask.Helpers;
using TrueCodeTask.Models;
using TrueCodeTask.Services.FileReaders;
using TrueCodeTask_Tests.Abstract;

namespace TrueCodeTask_Tests
{
    public class FileReaderTests : BaseIntegrationTest
    {
        [Fact]
        public void FileParserCommonMessageTest()
        {
            // Arrange
            var webFactory = GetTestApplication();
            string message = "Common byte message";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(message);
            writer.Flush();
            stream.Position = 0;
            using (var scope = webFactory.Services.CreateAsyncScope())
            {
                var scopedServices = scope.ServiceProvider;
                var factory = scopedServices.GetRequiredService<FileParserFactory>();
                var options = scopedServices.GetRequiredService<IOptions<StreamReaderSettings>>();

                // Act Assert
                factory.ShouldNotBeNull();
                var parse = factory.GetFileParser(stream, message);
                parse.ShouldNotBeNull();
                parse.GetType().ShouldBe(typeof(TxtFileReaderService));
                var result = parse.GetFileContent();
                result.ShouldNotBeNull();
                result.ShouldBe(message + options.Value?.LastMessageSymbol);
            }
            DeleteDatabase(webFactory);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void LargeMessageSizeParsingTest(bool isPartialMessageOn)
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            var firstMessage = "A large byte message for tests |";
            sb.AppendLine(firstMessage);
            var lineSize = Encoding.Unicode.GetByteCount(firstMessage) + 1;
            for (int i = 0; i < 20; i++)
            {
                sb.AppendLine("A large byte message for tests |");
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
            var lastSymbol = 'T';
            var webFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<FileParserFactory>();
                    services.Configure<StreamReaderSettings>(settings =>
                    {
                        settings.LastMessageSymbol = lastSymbol;
                        settings.MaxBufferSize = isPartialMessageOn ? lineSize : Encoding.Unicode.GetByteCount(sb.ToString());
                        settings.PartialReading = isPartialMessageOn;
                    });
                });
            });


            using (var scope = webFactory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var factory = scopedServices.GetRequiredService<FileParserFactory>();
                var options = scopedServices.GetRequiredService<IOptions<StreamReaderSettings>>();


                // Act Assert
                factory.ShouldNotBeNull();
                var parse = factory.GetFileParser(stream, sb.ToString());
                parse.ShouldNotBeNull();
                parse.GetType().ShouldBe(typeof(TxtFileReaderService));
                var result = parse.GetFileContent();
                if (isPartialMessageOn)
                {
                    result.ShouldBe(firstMessage + lastSymbol);
                }
                else
                {
                    sb.Append(lastSymbol).ToString().ShouldBe(result);
                }
            }


        }


    }
}