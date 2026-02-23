using System;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

public class ResumeFile : File
{
    public ResumeLanguage resumeLanguage {get; set;} 
}
