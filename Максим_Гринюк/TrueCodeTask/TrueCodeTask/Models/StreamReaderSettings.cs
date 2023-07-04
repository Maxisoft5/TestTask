namespace TrueCodeTask.Models
{
    public sealed class StreamReaderSettings
    {
        public int? MaxBufferSize { get; set; }
        public char? LastMessageSymbol { get; set; }
        public bool? PartialReading { get; set; }
    }
}
