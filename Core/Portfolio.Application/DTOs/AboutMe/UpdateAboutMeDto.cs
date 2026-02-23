using System;
using Portfolio.Domain.Entities;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.DTOs.AboutMe;
public class UpdateAboutMeDto
{
    // --- 1. KİŞİSEL BİLGİLER ---
    public string FullName { get; set; }
    public string JobTitle { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    // --- 2. ÇOKLU DİL DESTEKLİ ALANLAR ---
    // Eğer Biyografi için önceden MultiLanguageString sınıfı yaptıysan onu kullan
    public MultiLanguageString Biography { get; set; } 
    
    // Formda Title (Ünvan) için çoklu dil tab'i göremedim, JobTitle var. 
    // Eğer JobTitle çok dilli olacaksa onu da MultiLanguageString yapmalısın.

    // --- 3. BECERİLER (SKILLS) LİSTESİ ---
    public List<Skill> Skills { get; set; } = new List<Skill>();

    // --- 4. GÖRSEL YOLLARI (Sadece string yollar) ---
    public string? HeroImageUrl { get; set; } 
    public string? ImageUrl { get; set; }
}

