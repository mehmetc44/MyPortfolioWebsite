using System;

namespace Portfolio.Domain.Entities;

public class File:BaseEntity
{
    public string FileName {get; set;} = null!;
    public string Path {get; set;} = null!;

}
