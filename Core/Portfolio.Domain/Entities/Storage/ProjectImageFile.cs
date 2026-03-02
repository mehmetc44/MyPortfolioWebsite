using System;

namespace Portfolio.Domain.Entities;

public class ProjectImageFile : File
{
    public Project? Project { get; set; }
}
