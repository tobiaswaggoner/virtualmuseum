using Microsoft.AspNetCore.Mvc;
using virtualmuseum.web.api.Services;

namespace virtualmuseum.web.api.Api;

[ApiController]
[Route("api")]
public class MediaController
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }
    [HttpGet("media/{id}")]
    public byte[] Get(Guid id)
    {
        return _mediaService.GetMedia(id);
    }
    
    // Return file as a mime type
    [HttpGet("media/{id}/display")]
    public FileResult GetDisplay(Guid id)
    {
        return _mediaService.GetMediaAsFile(id);
    }
}