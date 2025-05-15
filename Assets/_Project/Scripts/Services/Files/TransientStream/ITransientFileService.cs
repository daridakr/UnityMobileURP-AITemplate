namespace Services.Files
{
    // Marker interface for the file service implementation that opens/closes the stream for each operation.
    public interface ITransientFileService : IFileService { }
}