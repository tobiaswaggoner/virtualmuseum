using System;

namespace Server.Data
{

    /// <summary>
    /// A MediaFile represents a file that can be displayed in virtual reality
    /// Possible types are
    /// 2DImage, 3DImage, 360DegreeImage, 2DVideo, 3DVideo, 360DegreeVideo
    /// MP3, Narration, Text, PDF
    /// The binary data can be downloaded from the Url
    /// </summary>
    public class MediaFile
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
    }
}