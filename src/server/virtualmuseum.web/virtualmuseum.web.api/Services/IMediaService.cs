using Microsoft.AspNetCore.Mvc;

namespace virtualmuseum.web.api.Services;

public interface IMediaService
{
    byte[] GetMedia(Guid id);
    FileResult GetMediaAsFile(Guid id);
}