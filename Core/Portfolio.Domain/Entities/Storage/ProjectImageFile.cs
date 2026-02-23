using System;

namespace Portfolio.Domain.Entities;

public class ProjectImageFile : File
{
    public ICollection<Project> Projects { get; set; }
}
