using System;

namespace Portfolio.Domain.Entities;

public class ProjectImageFile : File
{
    public Project Projects { get; set; }
}
