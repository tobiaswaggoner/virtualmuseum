using Microsoft.AspNetCore.Mvc;

namespace virtualmuseum.web.api.Services;

public class MediaService : IMediaService
{
    public byte[] GetMedia(Guid id)
    {
        return File.ReadAllBytes( Path.GetFullPath("./InputData/Media/Spaziergang1.jpg"));
    }
    
    public FileResult GetMediaAsFile(Guid id)
    {
        return new PhysicalFileResult(Path.GetFullPath("./InputData/Media/Spaziergang1.jpg"), "image/jpeg");
    }
}