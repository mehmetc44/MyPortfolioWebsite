using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services
{
    public interface IDatabaseSeeder
    {
        void Seed();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly AppDbContext _db;

        public DatabaseSeeder(AppDbContext db)
        {
            _db = db;
        }

        public void Seed()
        {
            _db.Database.Migrate();

            SeedCategories();

            _db.SaveChanges();
        }

        private void SeedCategories()
        {
            if (_db.Categories.Any())
            {
                _db.Categories.RemoveRange(_db.Categories);
                _db.SaveChanges();
            }

            var sw = new CategoryEntity { Id = "software-engineering", Name_EN = "Software Engineering", Name_TR = "Yazılım Mühendisliği", Name_DE = "Software-Engineering", IsSubCategory = false, ParentId = null };
            var ds = new CategoryEntity { Id = "distributed-systems", Name_EN = "Distributed Systems", Name_TR = "Dağıtık Sistemler", Name_DE = "Verteilte Systeme", IsSubCategory = false, ParentId = null };
            var ai = new CategoryEntity { Id = "artificial-intelligence", Name_EN = "Artificial Intelligence", Name_TR = "Yapay Zeka", Name_DE = "Künstliche Intelligenz", IsSubCategory = false, ParentId = null };
            var sr = new CategoryEntity { Id = "search-retrieval", Name_EN = "Search & Retrieval", Name_TR = "Arama & Getirme", Name_DE = "Suchen & Abrufen", IsSubCategory = false, ParentId = null };
            var be = new CategoryEntity { Id = "backend-engineering", Name_EN = "Backend Engineering", Name_TR = "Arka Uç Mühendisliği", Name_DE = "Backend-Engineering", IsSubCategory = false, ParentId = null };

            _db.Categories.AddRange(sw, ds, ai, sr, be);

            _db.Categories.AddRange(
                new CategoryEntity { Id = "software-architecture", ParentId = sw.Id, IsSubCategory = true, Name_EN = "Software Architecture", Name_TR = "Yazılım Mimarisi", Name_DE = "Softwarearchitektur" },
                new CategoryEntity { Id = "design-patterns", ParentId = sw.Id, IsSubCategory = true, Name_EN = "Design Patterns", Name_TR = "Tasarım Desenleri", Name_DE = "Entwurfsmuster" },
                new CategoryEntity { Id = "domain-driven-design", ParentId = sw.Id, IsSubCategory = true, Name_EN = "Domain-Driven Design", Name_TR = "Etki Alanı Odaklı Tasarım", Name_DE = "Domain-Driven Design" },
                new CategoryEntity { Id = "system-design", ParentId = sw.Id, IsSubCategory = true, Name_EN = "System Design", Name_TR = "Sistem Tasarımı", Name_DE = "Systemdesign" },

                new CategoryEntity { Id = "service-communication", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Service Communication", Name_TR = "Servis İletişimi", Name_DE = "Dienstkommunikation" },
                new CategoryEntity { Id = "messaging", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Messaging", Name_TR = "Mesajlaşma", Name_DE = "Messaging" },
                new CategoryEntity { Id = "event-driven-architecture", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Event-Driven Architecture", Name_TR = "Olay Güdümlü Mimari", Name_DE = "Ereignisgesteuerte Architektur" },
                new CategoryEntity { Id = "distributed-transactions", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Distributed Transactions", Name_TR = "Dağıtık İşlemler", Name_DE = "Verteilte Transaktionen" },
                new CategoryEntity { Id = "reliability", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Reliability", Name_TR = "Güvenilirlik", Name_DE = "Zuverlässigkeit" },
                new CategoryEntity { Id = "observability", ParentId = ds.Id, IsSubCategory = true, Name_EN = "Observability", Name_TR = "Gözlemlenebilirlik", Name_DE = "Beobachtbarkeit" },

                new CategoryEntity { Id = "llm-engineering", ParentId = ai.Id, IsSubCategory = true, Name_EN = "LLM Engineering", Name_TR = "Büyük Dil Modeli Mühendisliği", Name_DE = "LLM-Engineering" },
                new CategoryEntity { Id = "ai-agents", ParentId = ai.Id, IsSubCategory = true, Name_EN = "AI Agents", Name_TR = "Yapay Zeka Ajanları", Name_DE = "KI-Agenten" },
                new CategoryEntity { Id = "rag", ParentId = ai.Id, IsSubCategory = true, Name_EN = "RAG", Name_TR = "RAG", Name_DE = "RAG" },
                new CategoryEntity { Id = "agent-infrastructure", ParentId = ai.Id, IsSubCategory = true, Name_EN = "Agent Infrastructure", Name_TR = "Ajan Altyapısı", Name_DE = "Agenten-Infrastruktur" },
                new CategoryEntity { Id = "evaluation", ParentId = ai.Id, IsSubCategory = true, Name_EN = "Evaluation", Name_TR = "Değerlendirme", Name_DE = "Bewertung" },

                new CategoryEntity { Id = "search-systems", ParentId = sr.Id, IsSubCategory = true, Name_EN = "Search Systems", Name_TR = "Arama Sistemleri", Name_DE = "Suchsysteme" },
                new CategoryEntity { Id = "lexical-search", ParentId = sr.Id, IsSubCategory = true, Name_EN = "Lexical Search", Name_TR = "Sözlüksel Arama", Name_DE = "Lexikalische Suche" },
                new CategoryEntity { Id = "vector-search", ParentId = sr.Id, IsSubCategory = true, Name_EN = "Vector Search", Name_TR = "Vektör Arama", Name_DE = "Vektorsuche" },
                new CategoryEntity { Id = "hybrid-search", ParentId = sr.Id, IsSubCategory = true, Name_EN = "Hybrid Search", Name_TR = "Hibrit Arama", Name_DE = "Hybride Suche" },
                new CategoryEntity { Id = "ranking-retrieval", ParentId = sr.Id, IsSubCategory = true, Name_EN = "Ranking & Retrieval", Name_TR = "Sıralama ve Getirme", Name_DE = "Ranking & Retrieval" },

                new CategoryEntity { Id = "api-design", ParentId = be.Id, IsSubCategory = true, Name_EN = "API Design", Name_TR = "API Tasarımı", Name_DE = "API-Design" },
                new CategoryEntity { Id = "databases", ParentId = be.Id, IsSubCategory = true, Name_EN = "Databases", Name_TR = "Veritabanları", Name_DE = "Datenbanken" },
                new CategoryEntity { Id = "aspnet-core", ParentId = be.Id, IsSubCategory = true, Name_EN = "ASP.NET Core", Name_TR = "ASP.NET Core", Name_DE = "ASP.NET Core" },
                new CategoryEntity { Id = "performance", ParentId = be.Id, IsSubCategory = true, Name_EN = "Performance", Name_TR = "Performans", Name_DE = "Leistung" },
                new CategoryEntity { Id = "security", ParentId = be.Id, IsSubCategory = true, Name_EN = "Security", Name_TR = "Güvenlik", Name_DE = "Sicherheit" }
            );
        }
    }
}
