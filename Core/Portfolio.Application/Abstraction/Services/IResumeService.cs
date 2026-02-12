using System;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.Services;

public interface IResumeService
{
    // --- SKILLS (Yetenekler) ---
    Task<List<Skill>> GetAllSkillsAsync();
    Task AddSkillAsync(Skill skill);
    Task DeleteSkillAsync(Guid id);

    // --- LANGUAGES (Diller) ---
    Task<List<Language>> GetAllLanguagesAsync();
    Task AddLanguageAsync(Language language);
    Task DeleteLanguageAsync(Guid id);

    // --- TIMELINE (Eğitim & Deneyim) ---
    Task<List<Timeline>> GetAllTimelinesAsync();
    Task AddTimelineAsync(Timeline timeline);
    Task DeleteTimelineAsync(Guid id);
}
