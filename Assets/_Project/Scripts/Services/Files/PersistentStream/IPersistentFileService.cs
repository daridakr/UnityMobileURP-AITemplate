namespace Services.Files
{
    // Marker interface for the file service implementation that keeps the stream open persistently.
    public interface IPersistentFileService : IFileService { }
}