using System.Collections.Generic;
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
            if (_db.Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                try
                {
                    _db.Database.ExecuteSqlRaw(@"
                        CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                            ""MigrationId"" character varying(150) NOT NULL,
                            ""ProductVersion"" character varying(32) NOT NULL,
                            CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                        );
                        
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_TR"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_EN"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_DE"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_TR"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_EN"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_DE"" text;

                        INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                        VALUES ('20260714132915_InitialCreate', '8.0.8')
                        ON CONFLICT (""MigrationId"") DO NOTHING;
                    ");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[Schema Check Warning] {ex.Message}");
                }
            }

            _db.Database.Migrate(); // Applies any pending migrations and creates db file if needed

            SeedProfile();
            SeedProjects();
            SeedArticles();
            SeedUsers();

            // Perform data format migration for existing dates & read times
            var oldProjects = _db.Projects.ToList();
            foreach (var proj in oldProjects)
            {
                if (proj.Date == "Haziran 2026") { proj.Date = "2026-06-15"; }
                else if (proj.Date == "Nisan 2026") { proj.Date = "2026-04-15"; }
                else if (proj.Date == "Şubat 2026") { proj.Date = "2026-02-15"; }
                else if (proj.Date == "Kasım 2025") { proj.Date = "2025-11-15"; }
                else if (proj.Date == "Ağustos 2025") { proj.Date = "2025-08-15"; }
                else if (proj.Date == "Mayıs 2025") { proj.Date = "2025-05-15"; }
                else if (proj.Date == "Mart 2025") { proj.Date = "2025-03-15"; }
                else if (proj.Date == "Ocak 2025") { proj.Date = "2025-01-15"; }
            }

            var oldArticles = _db.Articles.ToList();
            foreach (var art in oldArticles)
            {
                if (art.Date == "Haziran 2026") { art.Date = "2026-06-15"; }
                else if (art.Date == "Nisan 2026") { art.Date = "2026-04-15"; }
                else if (art.Date == "Mart 2026") { art.Date = "2026-03-15"; }
                else if (art.Date == "Şubat 2026") { art.Date = "2026-02-15"; }
                else if (art.Date == "Ocak 2026") { art.Date = "2026-01-15"; }
                else if (art.Date == "Ağustos 2025") { art.Date = "2025-08-15"; }

                if (art.ReadTime.EndsWith(" dk okuma"))
                {
                    art.ReadTime = art.ReadTime.Replace(" dk okuma", "").Trim();
                }
            }

            _db.SaveChanges();
        }

        private void SeedUsers()
        {
            if (!_db.Users.Any())
            {
                _db.Users.Add(new UserEntity
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("admin123")
                });
            }
        }

        private void SeedProfile()
        {
            if (!_db.Profiles.Any())
            {
                _db.Profiles.Add(new ProfileEntity
                {
                    Id = 1,
                    Name = "Mehmet",
                    Tag_TR = "Yazılım Mimarı",
                    Tag_EN = "Software Architect",
                    Tag_DE = "Softwarearchitekt",
                    Title_TR = "Kıdemli Full Stack Mühendisi & Teknoloji Lideri",
                    Title_EN = "Senior Full Stack Engineer & Tech Lead",
                    Title_DE = "Senior Full Stack Engineer & Tech Lead",
                    Bio_TR = "Dağıtık web servisleri geliştirmek, ölçeklenebilir bulut mimarileri tasarlamak ve yüksek doğruluğa sahip kullanıcı platformları oluşturmak.",
                    Bio_EN = "Engineering distributed web services, designing scalable cloud architectures, and crafting high-fidelity user platforms.",
                    Bio_DE = "Entwicklung verteilter Webdienste, Entwurf skalierbarer Cloud-Architekturen und Erstellung hochpräziser Benutzerplattformen.",
                    AvatarUrl = "assets/avatar_senior.png",
                    Repos = 14,
                    Pubs = 30,
                    Github = "https://github.com/mehmetc44",
                    Linkedin = "https://linkedin.com",
                    Instagram = "https://instagram.com",
                    Medium = "https://medium.com",
                    CvPdfUrl_TR = "assets/cv_tr.pdf",
                    CvPdfUrl_EN = "assets/cv_en.pdf",
                    CvPdfUrl_DE = "assets/cv_de.pdf",
                    CvText_TR = @"{
  ""experiences"": [
    {
      ""title"": ""Yazılım Teknolojileri Lideri (Tech Lead)"",
      ""org"": ""Platar Tech, İstanbul"",
      ""date"": ""2023 - Günümüz"",
      ""bullets"": [
        ""Gerçek zamanlı plaka tanıma ve yapay zeka entegrasyonu sunan WebUI projesinin mimari kurulumunu yönetti."",
        ""SignalR tabanlı eşzamanlı veri akış altyapısını kurarak sunucu-istemci gecikmesini %40 oranında azalttı."",
        ""5 kişilik kıdemli geliştirici ekibine liderlik etti, kod standartlarını belirledi ve CI/CD süreçlerini otomatikleştirdi.""
      ]
    },
    {
      ""title"": ""Kıdemli Tam Yığın Yazılım Mühendisi (Senior Full Stack Eng)"",
      ""org"": ""Nexus Systems, İstanbul (Uzaktan)"",
      ""date"": ""2020 - 2023"",
      ""bullets"": [
        ""Yüksek trafikli SaaS uygulamalarının C# .NET Core ve React bileşenlerini tasarladı ve geliştirdi."",
        ""Veri tabanı indeks optimizasyonları ve sorgu revizyonları gerçekleştirerek SQL okuma sürelerini %35 hızlandırdı."",
        ""Docker konteynerizasyon yapısını kurarak yerel geliştirme ve sunucu ortamları arasındaki farkları ortadan kaldırdı.""
      ]
    }
  ],
  ""educations"": [
    {
      ""title"": ""Bilgisayar Mühendisliği Lisans Derecesi (B.Sc.)"",
      ""org"": ""İstanbul Teknik Üniversitesi, İstanbul"",
      ""date"": ""2012 - 2017"",
      ""desc"": ""Sistem mimarisi, yapay zeka altyapıları, algoritma analizi ve yüksek ölçekli veri tabanları üzerine odaklı akademik mühendislik derecesi.""
    }
  ],
  ""certificates"": [
    {
      ""title"": ""AWS Çözüm Mimarı Sertifikası (Solutions Architect Associate)"",
      ""date"": ""2024""
    },
    {
      ""title"": ""Microsoft Sertifikalı Çözüm Geliştirici (MCSD: App Builder)"",
      ""date"": ""2021""
    },
    {
      ""title"": ""İleri Düzey SQL Sorgulama ve Optimizasyon Eğitimi"",
      ""date"": ""2019""
    }
  ],
  ""volunteering"": [
    {
      ""title"": ""Açık Kaynak Kod Mentorluğu & Topluluk Liderliği"",
      ""org"": ""KodAkademi Topluluğu, Çevrimiçi"",
      ""date"": ""2022 - Günümüz"",
      ""desc"": ""Yazılıma yeni başlayan geliştirici adaylarına ve üniversite öğrencilerine haftalık kod inceleme ve sistem tasarımı mentörlüğü sağlamaktadır.""
    }
  ],
  ""languages"": [
    {
      ""name"": ""Türkçe"",
      ""level"": ""Anadil"",
      ""percentage"": 100
    },
    {
      ""name"": ""İngilizce"",
      ""level"": ""İleri Düzey / Profesyonel"",
      ""percentage"": 90
    },
    {
      ""name"": ""Almanca"",
      ""level"": ""Temel Düzey"",
      ""percentage"": 40
    }
  ]
}",
                    CvText_EN = @"{
  ""experiences"": [
    {
      ""title"": ""Software Technology Lead (Tech Lead)"",
      ""org"": ""Platar Tech, Istanbul"",
      ""date"": ""2023 - Present"",
      ""bullets"": [
        ""Managed the architectural setup of the WebUI project featuring real-time license plate recognition."",
        ""Established SignalR-based concurrent data flow, reducing server-to-client latency by 40%."",
        ""Led a team of 5 senior developers, defined coding standards, and automated CI/CD pipelines.""
      ]
    },
    {
      ""title"": ""Senior Full Stack Software Engineer"",
      ""org"": ""Nexus Systems, Istanbul (Remote)"",
      ""date"": ""2020 - 2023"",
      ""bullets"": [
        ""Designed and developed C# .NET Core and React components for high-traffic SaaS applications."",
        ""Implemented database index optimizations, speeding up SQL query execution times by 35%."",
        ""Established Docker containerization workflow to align local development and staging environments.""
      ]
    }
  ],
  ""educations"": [
    {
      ""title"": ""Bachelor of Science in Computer Engineering (B.Sc.)"",
      ""org"": ""Istanbul Technical University, Istanbul"",
      ""date"": ""2012 - 2017"",
      ""desc"": ""Focused on system architecture, database optimization, and machine learning infrastructure.""
    }
  ],
  ""certificates"": [
    {
      ""title"": ""AWS Certified Solutions Architect Associate"",
      ""date"": ""2024""
    },
    {
      ""title"": ""Microsoft Certified Solutions Developer (MCSD)"",
      ""date"": ""2021""
    }
  ],
  ""volunteering"": [
    {
      ""title"": ""Open Source Mentorship & Tech Lead"",
      ""org"": ""KodAkademi Tech Community, Online"",
      ""date"": ""2022 - Present"",
      ""desc"": ""Provides weekly code review and system design mentorship to junior developers and college students.""
    }
  ],
  ""languages"": [
    {
      ""name"": ""Turkish"",
      ""level"": ""Native"",
      ""percentage"": 100
    },
    {
      ""name"": ""English"",
      ""level"": ""Advanced / Professional"",
      ""percentage"": 90
    },
    {
      ""name"": ""German"",
      ""level"": ""Elementary"",
      ""percentage"": 40
    }
  ]
}",
                    CvText_DE = @"{
  ""experiences"": [
    {
      ""title"": ""Leiter Software-Technologien (Tech Lead)"",
      ""org"": ""Platar Tech, Istanbul"",
      ""date"": ""2023 - Heute"",
      ""bullets"": [
        ""Leitung der Architektur eines Echtzeit-Kennzeichenerkennungssystems."",
        ""Aufbau eines SignalR-basierten Datenflusses, der Latenzen um 40% reduzierte."",
        ""Führung von 5 Entwicklern, Definition von Code-Standards und CI/CD-Automatisierung.""
      ]
    },
    {
      ""title"": ""Senior Full Stack Softwareentwickler"",
      ""org"": ""Nexus Systems, Istanbul (Remote)"",
      ""date"": ""2020 - 2023"",
      ""bullets"": [
        ""Entwicklung von C# .NET Core und React-Komponenten für SaaS-Anwendungen."",
        ""Datenbank-Optimierungen, wodurch SQL-Abfragezeiten um 35% verkürzt wurden."",
        ""Einführung von Docker zur Vereinheitlichung lokaler und Testumgebungen.""
      ]
    }
  ],
  ""educations"": [
    {
      ""title"": ""Bachelor in Informatik (B.Sc.)"",
      ""org"": ""Technische Universität Istanbul, Istanbul"",
      ""date"": ""2012 - 2017"",
      ""desc"": ""Studienschwerpunkte in Systemarchitektur, databaseoptimierung und maschinellem Lernen.""
    }
  ],
  ""certificates"": [
    {
      ""title"": ""AWS Certified Solutions Architect Associate"",
      ""date"": ""2024""
    },
    {
      ""title"": ""Microsoft Certified Solutions Developer (MCSD)"",
      ""date"": ""2021""
    }
  ],
  ""volunteering"": [
    {
      ""title"": ""Open-Source-Mentorschaft"",
      ""org"": ""KodAkademi Community, Online"",
      ""date"": ""2022 - Heute"",
      ""desc"": ""Wöchentliches Code-Review- und Systemdesign-Mentoring für Junior-Entwickler.""
    }
  ],
  ""languages"": [
    {
      ""name"": ""Türkisch"",
      ""level"": ""Muttersprache"",
      ""percentage"": 100
    },
    {
      ""name"": ""Englisch"",
      ""level"": ""Fortgeschritten / Professionell"",
      ""percentage"": 90
    },
    {
      ""name"": ""Deutsch"",
      ""level"": ""Grundkenntnisse"",
      ""percentage"": 40
    }
  ]
}"
                });
            }
        }

        private void SeedProjects()
        {
            if (!_db.Projects.Any())
            {
                _db.Projects.AddRange(new List<ProjectEntity>
                {
                    new ProjectEntity
                    {
                        Id = "platar-lpr",
                        Title_TR = "Platar LPR — Plaka Tanıma Sistemi",
                        Title_EN = "Platar LPR — License Plate Recognition System",
                        Title_DE = "Platar LPR — Kennzeichenerkennungssystem",
                        Category = "ml-dl",
                        Date = "2026-06-15",
                        Client = "Platar Tech",
                        SubTag_TR = "MAKİNE ÖĞRENİMİ & DERİN ÖĞRENME",
                        SubTag_EN = "MACHINE LEARNING & DEEP LEARNING",
                        SubTag_DE = "MASCHINELLES LERNEN & DEEP LEARNING",
                        Description_TR = "Yüksek hızlı kamera akışlarından plaka okuyup web arayüzünde canlı listeleyen yapay zeka entegrasyonu.",
                        Description_EN = "Artificial intelligence integration listing plates from high-speed camera streams in real-time.",
                        Description_DE = "Integration künstlicher Intelligenz zur Echtzeit-Erfassung von Kennzeichen aus Hochgeschwindigkeits-Kameraströmen.",
                        Tech = "C#, ONNX, YOLOv8, SignalR, JavaScript",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/platar-lpr",
                        ImagesJson = "[\"assets/project_slide1.png\", \"assets/project_placeholder.png\", \"assets/project_slide2.png\"]",
                        DetailText_TR = "<p>Platar LPR, yüksek hızlı kamera akışlarından gerçek zamanlı plaka okuması yapan ve bu verileri saniyeler içinde web paneline yansıtan akıllı bir plaka tanıma yazılımıdır.</p><p>Projenin arka ucunda, ONNX Runtime ile optimize edilmiş bir YOLOv8 modeli kullanılmıştır. Algılanan plakalar, karakter tanıma (OCR) algoritmalarından geçirilerek veri tabanına işlenmekte ve SignalR WebSockets hatları üzerinden canlı izleme paneline anlık olarak basılmaktadır.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Model çıkarım (inference) süreleri CUDA entegrasyonu ile çerçeve başına 15ms seviyesine düşürüldü.</li><li>SignalR üzerinde oluşturulan telemetri kuyruk yönetimi sayesinde saniyede 120 eşzamanlı kamera akışı hatasız bir şekilde işlendi.</li><li>Plaka sorgulamalarındaki SQL gecikmeleri, ClickHouse entegrasyonu ve log indekslemeleri ile %35 oranında azaltıldı.</li></ul>",
                        DetailText_EN = "<p>Platar LPR is an AI integration that reads license plates from high-speed camera streams in real-time.</p><p>The backend uses YOLOv8 optimized with ONNX Runtime. Detected plates are parsed through OCR algorithms and streamed via SignalR WebSockets lines directly to live telemetry dashboards.</p><h5>Key Engineering Milestones</h5><ul><li>Model inference duration reduced to 15ms per frame via CUDA acceleration.</li><li>WebSocket channels powered by SignalR handled 120 concurrent camera streams with zero delay.</li><li>SQL querying latencies reduced by 35% using indexing schemas.</li></ul>",
                        DetailText_DE = "<p>Platar LPR ist eine KI-Integration, die Kennzeichen aus Hochgeschwindigkeits-Kameraströmen in Echtzeit liest.</p><p>Das Backend nutzt YOLOv8, optimiert mit ONNX Runtime. Erkannte Schilder werden über OCR-Algorithmen analysiert und per SignalR WebSockets direkt in Echtzeit-Telemetrie-Dashboards gestreamt.</p><h5>Wesentliche Meilensteine</h5><ul><li>Modell-Inferenzzeit durch CUDA-Beschleunigung auf 15 ms pro Frame reduziert.</li><li>SignalR-WebSocket-Kanäle verarbeiteten 120 Kamera-Streams fehlerfrei in Echtzeit.</li><li>SQL-Abfrageverzögerungen durch Indexierungsschemata um 35 % verringert.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "nexus-crm",
                        Title_TR = "Nexus Enterprise SaaS CRM",
                        Title_EN = "Nexus Enterprise SaaS CRM",
                        Title_DE = "Nexus Enterprise SaaS CRM",
                        Category = "web",
                        Date = "2026-04-15",
                        Client = "Nexus Systems",
                        SubTag_TR = "KURUMSAL WEB PLATFORMLARI",
                        SubTag_EN = "ENTERPRISE WEB PLATFORMS",
                        SubTag_DE = "UNTERNEHMENS-WEBPLATTFORMEN",
                        Description_TR = "Eşzamanlı müşteri ve satış verilerini canlı grafikler ve çok kiracılı veritabanı yalıtımı ile yöneten platform.",
                        Description_EN = "Multi-tenant CRM SaaS platform managing real-time sales and customer telemetry graphs.",
                        Description_DE = "Mandantenfähige CRM-SaaS-Plattform zur Verwaltung von Echtzeit-Verkaufs- und Kundentelemetriedaten.",
                        Tech = ".NET Core, React, Docker, PostgreSQL, Tailwind",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/nexus-crm",
                        ImagesJson = "[\"assets/project_placeholder.png\", \"assets/project_slide1.png\", \"assets/project_slide2.png\"]",
                        DetailText_TR = "<p>Nexus Enterprise CRM, çok kiracılı veri yalıtımı ve yüksek eşzamanlılık gerektiren kurumsal seviyede bir SaaS müşteri yönetim platformudur.</p><p>Ön yüzde React.js ve Tailwind CSS kullanılarak akıcı bir kullanıcı deneyimi oluşturulmuş; arka yüzde ise .NET Core mimarisi, Entity Framework ve PostgreSQL kullanılarak modüler bir mimari kurulmuştur. Sistem, Docker konteynerizasyon altyapısı sayesinde kolayca ölçeklenebilmektedir.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Tenant-based PostgreSQL yalıtımı ile her müşterinin verisi fiziksel ve mantıksal olarak ayrıştırıldı.</li><li>Entity Framework Core veritabanı sorgularında lazy-loading optimizasyonları yapılarak bellek tüketimi %45 düşürüldü.</li><li>Docker Compose altyapısı ile yerel test ve staging sunucuları tamamen senkronize hale getirildi.</li></ul>",
                        DetailText_EN = "<p>Nexus Enterprise CRM is a multi-tenant CRM SaaS platform for high-concurrency client data operations.</p><p>React and Tailwind power the frontend UX, while .NET Core and PostgreSQL shape the modular backend data engine. The entire service is dockerized for automated scale-out triggers.</p><h5>Key Engineering Milestones</h5><ul><li>Tenant separation in PostgreSQL achieved at connection routing levels.</li><li>Memory occupancy reduced by 45% using lazy-loading database optimizations.</li><li>Docker Compose integrations synchronized development and production nodes.</li></ul>",
                        DetailText_DE = "<p>Nexus Enterprise CRM ist eine mandantenfähige CRM-SaaS-Plattform für hochgradig ausgelastete Client-Datenvorgänge.</p><p>React und Tailwind treiben die Frontend-UX an, während .NET Core und PostgreSQL die modulare Backend-Daten-Engine bilden. Die gesamte Anwendung ist für automatisierte Skalierung dockerisiert.</p><h5>Wesentliche Meilensteine</h5><ul><li>Mandantentrennung in PostgreSQL auf Verbindungsebene realisiert.</li><li>Speicherbelegung durch Lazy-Loading-Datenbankoptimierungen um 45 % reduziert.</li><li>Docker Compose synchronisierte Entwicklungs- und Produktionsknoten vollständig.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "docuquery-rag",
                        Title_TR = "Local DocuQuery RAG Agent",
                        Title_EN = "Local DocuQuery RAG Agent",
                        Title_DE = "Lokaler DocuQuery RAG Agent",
                        Category = "ai-rag",
                        Date = "2026-02-15",
                        Client = "Açık Kaynak Projesi",
                        SubTag_TR = "YAPAY ZEKA & RAG",
                        SubTag_EN = "ARTIFICIAL INTELLIGENCE & RAG",
                        SubTag_DE = "KÜNSTLICHE INTELLIGENZ & RAG",
                        Description_TR = "Kurumsal doküman arşivlerini semantik vektör uzayına aktararak yerel LLM üzerinden doğru veri sorgulaması sağlayan RAG projesi.",
                        Description_EN = "RAG project enabling semantic vector space document querying over local LLM instances.",
                        Description_DE = "RAG-Projekt zur semantischen Abfrage von Dokumenten über lokale LLM-Instanzen.",
                        Tech = "Python, LangChain, Llama-3, pgvector, FastAPI",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/docuquery-rag",
                        ImagesJson = "[\"assets/project_slide2.png\", \"assets/project_slide1.png\", \"assets/project_placeholder.png\"]",
                        DetailText_TR = "<p>Local DocuQuery, kurumsal arşivlerin semantik vektör uzayına aktarılması ve yerel LLM modelleri (Llama-3, Mistral) vasıtasıyla hassas verilerin şirket içinde güvenli bir şekilde sorgulanmasını sağlayan bir Retrieval-Augmented Generation (RAG) aracıdır.</p><p>LangChain kütüphanesi ve pgvector PostgreSQL eklentisi üzerine kurulan sistem, dokümanları parçalara (chunk) bölerek gömme (embedding) vektörleri oluşturur. Kullanıcı sorusu geldiğinde en yakın anlamlı parçaları çekip yerel modele bağlam (context) olarak iletir.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>pgvector üzerindeki HNSW indeks yapılandırmaları ile vektör arama süreleri 200ms'nin altına çekildi.</li><li>FastAPI arka ucu üzerinde asenkron veri alım hatları kuruldu.</li><li>Kurumsal veriler şirket dışına çıkmadan yerel GPU kümesi üzerinde çalışacak şekilde güvenlik uyumluluğu sağlandı.</li></ul>",
                        DetailText_EN = "<p>Local DocuQuery is a Retrieval-Augmented Generation (RAG) tool to search corporate document archives safely using local LLM models (Llama-3, Mistral).</p><p>Built on LangChain and pgvector PostgreSQL extension, it tokenizes documents to save semantic embedding vectors. User questions pull top matching contexts and feed them to the local model.</p><h5>Key Engineering Milestones</h5><ul><li>pgvector search latency reduced below 200ms using HNSW indices.</li><li>Asynchronous pipeline streams configured over FastAPI backend models.</li><li>Data kept fully localized on company GPU nodes for maximum security and compliance.</li></ul>",
                        DetailText_DE = "<p>Local DocuQuery is ein RAG-Tool (Retrieval-Augmented Generation) zur sicheren Durchsuchung von Dokumentenarchiven mittels lokaler LLM-Modelle (Llama-3, Mistral).</p><p>Basierend auf LangChain und der PostgreSQL-Erweiterung pgvector zerlegt es Dokumente in Segmente und speichert semantische Vektoren. Benutzerfragen rufen passende Kontexte ab und leiten diese an das lokale Modell weiter.</p><h5>Wesentliche Meilensteine</h5><ul><li>pgvector-Suchlatenz durch HNSW-Indizes unter 200 ms gesenkt.</li><li>Asynchrone Pipelines über FastAPI-Backend-Modelle konfiguriert.</li><li>Daten für maximale Sicherheit und Compliance vollständig auf lokalen GPU-Knoten gehalten.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "sera-telemetry",
                        Title_TR = "Sera ML Telemetri Tahminleyici",
                        Title_EN = "Greenhouse ML Telemetry Predictor",
                        Title_DE = "Gewächshaus-ML-Telemetrie-Prädiktor",
                        Category = "ml-dl",
                        Date = "2025-11-15",
                        Client = "AgriTech Ltd",
                        SubTag_TR = "MAKİNE ÖĞRENİMİ & TELEMETRİ",
                        SubTag_EN = "MACHINE LEARNING & TELEMETRY",
                        SubTag_DE = "MASCHINELLES LERNEN & TELEMETRIE",
                        Description_TR = "Sera sensörlerinden gelen anlık sıcaklık ve nem telemetry verilerini işleyip 24 saatlik tahmin modelleri çıkaran akıllı sistem.",
                        Description_EN = "Smart IoT system predicting greenhouse climate metrics for 24-hour intervals using machine learning.",
                        Description_DE = "Intelligentes IoT-System zur Vorhersage von Gewächshaus-Klimawerten mittels maschinellem Lernen.",
                        Tech = "Python, Scikit-Learn, WebSockets, InfluxDB",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/sera-telemetry",
                        ImagesJson = "[\"assets/project_slide1.png\", \"assets/project_slide2.png\", \"assets/project_placeholder.png\"]",
                        DetailText_TR = "<p>Sera ML Telemetri Tahminleyici, tarım seralarındaki mikroişlemcili sensörlerden gelen anlık verileri toplayan ve yapay zeka modelleri ile 24 saatlik klima/sulama tahminleri üreten akıllı bir IoT kontrol yazılımıdır.</p><p>Sistem, sensör akışlarını WebSockets üzerinden InfluxDB zaman serisi veri tabanına kaydeder. Scikit-Learn ile eğitilen regresyon modelleri, geçmiş hava modellerini analiz ederek sera içi iklim koruma önlemlerini otomatik devreye alır.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Dakikada 10.000 veri paketini kesintisiz işleyen yüksek dayanımlı WebSocket kanalları kuruldu.</li><li>Sensör anormalliklerini ve hatalı veri okumalarını tespit etmek için Kalman filtreleme algoritmaları entegre edildi.</li><li>Akıllı sulama vanası kontrolü ile seralarda ortalama %22 oranında su tasarrufu sağlandı.</li></ul>",
                        DetailText_EN = "<p>Greenhouse ML Telemetry Predictor compiles telemetry records from sensors (temperature, humidity, pH) and predicts climate trends for 24-hour ranges.</p><p>Sensor streams are written via WebSockets into InfluxDB. Scikit-Learn regression models evaluate climate metrics to automatically adjust greenhouse venting/irrigation systems.</p><h5>Key Engineering Milestones</h5><ul><li>High-throughput WebSocket nodes developed handling 10k packets per minute.</li><li>Kalman filters integrated to eliminate raw sensor noise.</li><li>Intelligent irrigation control produced a 22% reduction in water consumption.</li></ul>",
                        DetailText_DE = "<p>Gewächshaus-ML-Telemetrie-Prädiktor verarbeitet Messwerte aus Sensoren (Temperatur, Feuchtigkeit, pH) und prognostiziert Klimatrends für 24 Stunden.</p><p>Sensor-Streams werden über WebSockets in InfluxDB geschrieben. Scikit-Learn-Regressionsmodelle analysieren Klimadaten zur automatischen Steuerung von Gewächshauslüftung und Bewässerung.</p><h5>Wesentliche Meilensteine</h5><ul><li>Durchsatzstarke WebSocket-Knoten für 10.000 Pakete pro Minute entwickelt.</li><li>Kalman-Filter integriert, um Rohsensorrauschen zu eliminieren.</li><li>Intelligente Bewässerungssteuerung führte zu 22 % Wassereinsparung.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "fira-parser",
                        Title_TR = "AST Fira Parser Derleyici",
                        Title_EN = "AST Fira Parser Compiler",
                        Title_DE = "AST Fira Parser Compiler",
                        Category = "other",
                        Date = "2025-08-15",
                        Client = "Açık Kaynak Projesi",
                        SubTag_TR = "DERLEYİCİ TASARIMI & SİSTEMLER",
                        SubTag_EN = "COMPILER DESIGN & SYSTEMS",
                        SubTag_DE = "COMPILER DESIGN & SYSTEME",
                        Description_TR = "Özel betik dillerini soyut sözdizimi ağaçlarına (AST) çeviren, yüksek hızlı bellek yönetimli açık kaynaklı derleyici modülü.",
                        Description_EN = "Open-source high performance compiler parser generating Abstract Syntax Trees (AST).",
                        Description_DE = "Open-Source-Compiler-Parser zur Erzeugung abstrakter Syntaxbäume (AST) mit hoher Performance.",
                        Tech = "Rust, Parser, AST, CLI",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/fira-parser",
                        ImagesJson = "[\"assets/project_placeholder.png\", \"assets/project_slide2.png\", \"assets/project_slide1.png\"]",
                        DetailText_TR = "<p>AST Fira Parser, özel geliştirilmiş betik dillerini analiz edip soyut sözdizimi ağaçlarına (AST) dönüştüren, yüksek performanslı ve düşük bellek ayak izine sahip açık kaynaklı bir derleyici ön ucu modülüdür.</p><p>Rust dilinin sahiplik ve bellek güvenliği özelliklerinden yararlanılarak sıfırdan yazılmıştır. CLI aracılığı ile ham metin dosyalarını kabul ederek JSON veya binary formatta çıktı üretebilmektedir.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Hızlı Lexer tokenizer tasarımı sayesinde saniyede 1.5 milyon satır kod analiz etme kapasitesine ulaşıldı.</li><li>Sıfır kopyalama teknikleri kullanılarak bellek ayırma ihtiyaçları en aza indirildi.</li><li>Gelişmiş hata yakalama mekanizması ile sözdizimi hatalarında derlemeyi durdurmadan detaylı raporlama sağlandı.</li></ul>",
                        DetailText_EN = "<p>AST Fira Parser is a high performance compiler frontend written in Rust to parse script languages into Abstract Syntax Trees (AST).</p><p>Leveraging Rust ownership rules, it processes textual configurations and exports semantic AST graphs as binary or JSON representations via CLI tools.</p><h5>Key Engineering Milestones</h5><ul><li>Lexer tokenizer throughput optimized to parse 1.5 million lines of code per second.</li><li>Zero-copy parsing techniques implemented to minimize heap allocation calls.</li><li>Robust error recovery pipelines programmed to output verbose warnings without terminating execution.</li></ul>",
                        DetailText_DE = "<p>AST Fira Parser ist ein in Rust geschriebenes Compiler-Frontend zur Analyse von Skriptsprachen in abstrakte Syntaxbäume (AST).</p><p>Unter Ausnutzung der Rust Ownership-Regeln verarbeitet es Textkonfigurationen und exportiert AST-Graphen über CLI-Tools als Binär- oder JSON-Dateien.</p><h5>Wesentliche Meilensteine</h5><ul><li>Lexer-Tokenizer auf Durchsatz von 1,5 Millionen Zeilen Code pro Sekunde optimiert.</li><li>Zero-Copy-Parsing implementiert, um Heap-Allokationen zu minimieren.</li><li>Robuste Fehlerbehebungs-Pipelines programmiert, um detaillierte Warnungen auszugeben.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "logstream-pipeline",
                        Title_TR = "LogStream Dağıtık Telemetri Pipeline",
                        Title_EN = "LogStream Distributed Telemetry Pipeline",
                        Title_DE = "LogStream Verteilte Telemetriepipeline",
                        Category = "web",
                        Date = "2025-05-15",
                        Client = "Nexus Systems",
                        SubTag_TR = "DAĞITIK BULUT SERVİSLERİ",
                        SubTag_EN = "DISTRIBUTED CLOUD SERVICES",
                        SubTag_DE = "VERTEILTE CLOUD-DIENSTE",
                        Description_TR = "Saniyede on binlerce log verisini kabul ederek bufferlayan ve canlı hata analitiği paneline aktaran kuyruklu log boru hattı.",
                        Description_EN = "Distributed data pipeline buffering logs and piping real-time telemetry to ClickHouse.",
                        Description_DE = "Verteilte Pipeline zur Pufferung von Protokollen und Echtzeitübertragung an ClickHouse.",
                        Tech = "Node.js, Redis, Kafka, TypeScript, ClickHouse",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/logstream-pipeline",
                        ImagesJson = "[\"assets/project_slide2.png\", \"assets/project_slide1.png\", \"assets/project_placeholder.png\"]",
                        DetailText_TR = "<p>LogStream, mikroservis mimarisine sahip büyük sistemlerin ürettiği yapılandırılmamış log ve telemetri verilerini anlık olarak dinleyen, bufferlayan ve analitik veri tabanlarına dağıtan yüksek kapasiteli bir veri taşıma sistemidir.</p><p>Node.js, Apache Kafka ve Redis kullanılarak inşa edilen boru hattı, ClickHouse analitik veri tabanına veri akışını yönetir. Canlı gösterge panelleri bu altyapıyı sorgulayarak hata oranlarını saniyeler içinde raporlar.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Kafka consumer grupları dinamik olarak ölçeklenerek saniyede 45.000 log paketi yığılma olmadan işlendi.</li><li>Redis buffer katmanı ile clickhouse veri tabanına toplu kayıtlar yapılarak veritabanı G/Ç yükü %60 hafifletildi.</li><li>Dinamik kural tanımlama motoru sayesinde kritik hatalarda Slack/PagerDuty anlık alarm entegrasyonu sağlandı.</li></ul>",
                        DetailText_EN = "<p>LogStream is a telemetry ingestion and pipeline system designed to write log outputs into ClickHouse databases using Kafka brokers.</p><p>Node.js microservices parse textual log packets while Redis instances buffer transactions before database inserts, supplying dashboards with real-time health monitors.</p><h5>Key Engineering Milestones</h5><ul><li>Kafka consumer configurations auto-scaled to ingest 45k logs per second.</li><li>Redis caching layers reduced database input/output overhead by 60%.</li><li>Automated alerts integrated directly into Slack/PagerDuty notification loops.</li></ul>",
                        DetailText_DE = "<p>LogStream is ein Telemetrie- und Pipeline-Infrastruktursystem zur ClickHouse Integration mittels Kafka-Brokern.</p><p>Node.js-Mikroservices parsen Textprotokollpaket, während Redis-Instanzen Transaktionen vor Datenbankeinfügungen puffern und Dashboards mit Echtzeit-Zustandsmonitoren versorgen.</p><h5>Wesentliche Meilensteine</h5><ul><li>Kafka-Consumer-Konfigurationen skalierten automatisch auf 45.000 Protokolle pro Sekunde.</li><li>Redis-Caching-Ebenen reduzierten Datenbank-I/O-Overhead um 60 %.</li><li>Automatisierte Alarme direkt in Slack/PagerDuty-Benachrichtigungsschleifen integriert.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "ai-helper",
                        Title_TR = "AI Dev Helper Agent Orkestratörü",
                        Title_EN = "AI Dev Helper Agent Orchestrator",
                        Title_DE = "AI Dev Helper Agenten-Orchestrator",
                        Category = "ai-rag",
                        Date = "2025-03-15",
                        Client = "Açık Kaynak Projesi",
                        SubTag_TR = "YAPAY ZEKA & AJANLAR",
                        SubTag_EN = "ARTIFICIAL INTELLIGENCE & AGENTIC",
                        SubTag_DE = "KÜNSTLICHE INTELLIGENZ & AGENTEN",
                        Description_TR = "Otonom olarak kod analiz edip, hataları düzelten ve CI/CD hatlarına entegre olan çok etmenli (multi-agent) yapay zeka asistanı.",
                        Description_EN = "Multi-agent autonomous coding assistant fixing bugs and integrating with CI/CD.",
                        Description_DE = "Autonomer Codierassistent zur Fehlerbehebung und CI/CD-Integration.",
                        Tech = "Gemini API, TypeScript, Agentic, Node.js",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/ai-helper",
                        ImagesJson = "[\"assets/project_slide1.png\", \"assets/project_slide2.png\", \"assets/project_placeholder.png\"]",
                        DetailText_TR = "<p>AI Dev Helper, yazılım projelerindeki hataları otonom olarak analiz eden, kod değişiklik önerileri sunan, testleri koşan ve sürüm kontrollerine entegre olan çok etmenli bir yapay zeka asistanıdır.</p><p>Gemini 1.5 Pro API'si ve Node.js agentic runtime üzerine kurulan sistem, kod deposunu semantik olarak tarar. Karar alma modülü, her bir ajanı belirli görevlerle tetikleyerek ortak akıl yürütme sağlar.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Düşünme zinciri yönlendirmesiyle karmaşık kod tabanlarında hata bulma isabet oranı %80'e ulaştı.</li><li>Kod güvenlik analizi eklentisiyle OWASP Top 10 açıklarını tarayan özel ajan kural seti tanımlandı.</li><li>GitHub Actions entegrasyonu ile PR kod incelemesi ve düzeltme commit'leri otomatikleştirildi.</li></ul>",
                        DetailText_EN = "<p>AI Dev Helper orchestrates Gemini API agents to audit codebase repositories dynamically, suggesting PR changes and validating test suites.</p><p>Running on Node.js agentic runtimes, the assistant assigns logical roles (reader, tester, auditor) to LLM chains, generating cohesive pull request feedback.</p><h5>Key Engineering Milestones</h5><ul><li>Chain-of-thought prompting resulted in an 80% accuracy score for complex debug analysis.</li><li>Custom security rules configured to scan for OWASP Top 10 code vulnerabilities.</li><li>CI/CD integrations automated commit updates directly inside GitHub Actions.</li></ul>",
                        DetailText_DE = "<p>AI Dev Helper steuert Gemini-API-Agenten zur Überprüfung von Repositories, schlägt PR-Änderungen vor und validiert Testsuiten.</p><p>Unter Node.js-Agenten-Runtimes weist der Assistent LLM-Ketten logische Rollen (Reader, Tester, Auditor) zu und erzeugt zusammenhängendes Pull-Request-Feedback.</p><h5>Wesentliche Meilensteine</h5><ul><li>Chain-of-Thought-Prompting führte zu einer Genauigkeit von 80 % bei komplexen Debug-Analysen.</li><li>Spezielle Sicherheitsregeln zur Überprüfung auf OWASP Top 10 Schwachstellen konfiguriert.</li><li>CI/CD-Integrationen automatisierten Commit-Updates direkt in GitHub Actions.</li></ul>"
                    },
                    new ProjectEntity
                    {
                        Id = "glassui-kit",
                        Title_TR = "Frosty GlassUI Kit",
                        Title_EN = "Frosty GlassUI Kit",
                        Title_DE = "Frosty GlassUI Kit",
                        Category = "other",
                        Date = "2025-01-15",
                        Client = "Açık Kaynak Projesi",
                        SubTag_TR = "ESTETİK & ÖN UÇ BİLEŞENLERİ",
                        SubTag_EN = "AESTHETICS & FRONTEND UTILS",
                        SubTag_DE = "ÄSTHETIK & FRONTEND-KOMPONENTEN",
                        Description_TR = "Web kontrol panelleri için tasarlanmış, optimize edilmiş backdrop-filter maskeleri içeren modern glassmorphic arayüz kütüphanesi.",
                        Description_EN = "Modern CSS glassmorphism library with optimized GPU rendering.",
                        Description_DE = "Moderne CSS-Glassmorphismus-Bibliothek mit optimiertem GPU-Rendering.",
                        Tech = "CSS3, Vanilla CSS, Aesthetics",
                        RepoUrl = "https://github.com",
                        DemoUrl = "portfolio/glassui-kit",
                        ImagesJson = "[\"assets/project_placeholder.png\", \"assets/project_slide1.png\", \"assets/project_slide2.png\"]",
                        DetailText_TR = "<p>Frosty GlassUI, modern web siteleri ve analitik gösterge panelleri için tasarlanmış, optimize edilmiş frosted glass arayüz bileşenlerini içeren hafif bir CSS kütüphanesidir.</p><p>Saf CSS3 değişkenleri, optimize edilmiş backdrop-filter filtreleri ve özel iç gölge katmanları ile yazılmıştır. Tarayıcıların ekran kartı ivmelendirmesini tetikleyerek gecikmesiz animasyonlar sunar.</p><h5>Temel Mühendislik Kazanımları</h5><ul><li>Modern tarayıcılarda piksel tabanlı donanımsal hızlandırma tetiklenerek sayfa kaydırma performansı 60 FPS'e sabitlendi.</li><li>Sadece 12KB dosya boyutu ile sıfır bağımlılıklı modüler bileşen yapısı sunuldu.</li><li>Karanlık ve aydınlık mod temalarına tam duyarlı CSS değişken yapılandırması tamamlandı.</li></ul>",
                        DetailText_EN = "<p>Frosty GlassUI is a lightweight CSS library providing frosted glass components styled via CSS3 variables and backdrop filter assets.</p><p>Engineered for high performance dashboard mockups, the library triggers browser hardware acceleration (GPU) to display latency-free UI renders.</p><h5>Key Engineering Milestones</h5><ul><li>Achieved a stable 60 FPS scrolling performance by triggering GPU rendering pipelines.</li><li>Zero dependencies and package layout footprint limited to 12KB.</li><li>Comprehensive CSS Custom Property bindings for seamless light and dark mode toggles.</li></ul>",
                        DetailText_DE = "<p>Frosty GlassUI ist eine schlanke CSS-Bibliothek mit Mattglaskomponenten, die über CSS3-Variablen und Backdrop-Filter gestaltet sind.</p><p>Die für hochperformante Dashboard-Mockups entwickelte Bibliothek nutzt die Browser-Hardwarebeschleunigung (GPU) für latenzfreie UI-Renderings.</p><h5>Wesentliche Meilensteine</h5><ul><li>Stabile 60 FPS Scroll-Leistung durch Aktivierung der GPU-Rendering-Pipelines erreicht.</li><li>Keine Abhängigkeiten und Paket-Layout-Footprint auf 12 KB begrenzt.</li><li>Umfassende CSS Custom Property Bindings für nahtlose Hell- und Dunkelmodus-Umschaltung.</li></ul>"
                    }
                });
            }
        }

        private void SeedArticles()
        {
            if (!_db.Articles.Any())
            {
                _db.Articles.AddRange(new List<ArticleEntity>
                {
                    new ArticleEntity
                    {
                        Id = "saas-multitenancy",
                        Title_TR = "SaaS Mimarilerinde Çok Kiracılı Veri Güvenliği ve EF Core",
                        Title_EN = "Multi-Tenant Data Security in SaaS Architectures with EF Core",
                        Title_DE = "Mandantenfähige Datensicherheit in SaaS-Architekturen mit EF Core",
                        Category = "architecture",
                        Date = "2026-04-15",
                        ReadTime = "6",
                        SubTag_TR = "YAZILIM MİMARİSİ & POSTGRESQL",
                        SubTag_EN = "SOFTWARE ARCHITECTURE & POSTGRESQL",
                        SubTag_DE = "SOFTWAREARCHITEKTUR & POSTGRESQL",
                        Excerpt_TR = "Entity Framework Core ve PostgreSQL şemalarıyla her tenant için izole edilmiş veri tabanları kurmayı ve SQL seviyesinde yalıtımı anlatan rehber makale.",
                        Excerpt_EN = "Guide explaining database tenant isolation strategies using EF Core, PostgreSQL schemas, and SQL-level security layers.",
                        Excerpt_DE = "Leitfaden zur Erläuterung von Strategien zur Mandantenisolierung in Datenbanken mit EF Core, PostgreSQL-Schemata und Sicherheitsstufen.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>Modern SaaS uygulamalarının mimari tasarımlarında en kritik konulardan biri çok kiracılı veri yalıtımıdır. Farklı müşterilerin verilerinin birbirine karışmaması ve yasal uyumluluklerin sağlanması veritabanı yalıtım stratejisinin doğru seçilmesine bağlıdır.</p><p>Veri yalıtımında üç ana yaklaşım bulunur: Veritabanı bazlı yalıtım, Şema bazlı yalıtım ve Paylaşımlı veritabanı. Bu makalede paylaşımlı veritabanı modelinde Entity Framework Core kullanarak verileri nasıl güvenli bir şekilde yalıtabileceğimizi inceleyeceğiz.</p><h5>EF Core Global Query Filters Entegrasyonu</h5><p>EF Core, veritabanı sorgularına otomatik olarak eklenen global filtreleri destekler. Bu sayede, geliştiricinin her sorguda <code>where TenantId == X</code> yazmasına gerek kalmadan sorgular SQL seviyesinde filtrelenir.</p><pre><code>protected override void OnModelCreating(ModelBuilder modelBuilder)\n{\n    base.OnModelCreating(modelBuilder);\n\n    // Global Query Filter tanımı\n    modelBuilder.Entity&lt;Order&gt;()\n        .HasQueryFilter(o =&gt; o.TenantId == _tenantService.TenantId);\n}</code></pre><h5>Temel Avantajlar ve Dikkat Edilmesi Gerekenler</h5><ul><li><strong>Maliyet Etkinliği:</strong> Tek bir veritabanı sunucusu ve şeması kullanıldığı için kaynak tüketimi ve veritabanı lisanslama maliyetleri düşüktür.</li><li><strong>Performans İndeksleri:</strong> Paylaşımlı tablolarda sorguların hızlı çalışması için <code>TenantId</code> sütununun mutlaka kompozit indekslere dahil edilmesi gerekir.</li><li><strong>Veri Sızıntısı Riski:</strong> Global filtrelerin yanlışlıkla <code>IgnoreQueryFilters()</code> metodu ile devre dışı bırakılmadığından emin olunmalıdır.</li></ul>",
                        DetailText_EN = "<p>Data isolation in modern SaaS applications is one of the most critical aspects of architecture design, ensuring that user data does not mix.</p><p>This guide reviews Database-per-tenant, Schema-per-tenant, and Shared-database strategies. We explore implementing global query filters in Entity Framework Core to secure data isolation at the database level.</p><h5>EF Core Global Query Filters Integration</h5><p>EF Core supports global query filters added to entity queries automatically. Developers don't have to append <code>where TenantId == X</code> manually.</p><pre><code>protected override void OnModelCreating(ModelBuilder modelBuilder)\n{\n    base.OnModelCreating(modelBuilder);\n    modelBuilder.Entity&lt;Order&gt;().HasQueryFilter(o =&gt; o.TenantId == _tenant);\n}</code></pre>",
                        DetailText_DE = "<p>Die Datenisolierung in modernen SaaS-Anwendungen ist einer der kritischsten Aspekte des Architekturentwurfs, um sicherzustellen, dass Benutzerdaten nicht vermischt werden.</p><p>Dieser Leitfaden untersucht Strategien für separate Datenbanken, separate Schemata und gemeinsam genutzte Datenbanken. Wir betrachten die Implementierung globaler Abfragefilter in EF Core, um die Datenisolation auf Datenbankebene abzusichern.</p><h5>EF Core Globale Abfragefilter-Integration</h5><p>EF Core unterstützt globale Abfragefilter, die automatisch zu Abfragen hinzugefügt werden, ohne dass Entwickler <code>where TenantId == X</code> manuell anfügen müssen.</p><pre><code>protected override void OnModelCreating(ModelBuilder modelBuilder)\n{\n    base.OnModelCreating(modelBuilder);\n    modelBuilder.Entity&lt;Order&gt;().HasQueryFilter(o =&gt; o.TenantId == _tenant);\n}</code></pre>"
                    },
                    new ArticleEntity
                    {
                        Id = "signalr-deepdive",
                        Title_TR = ".NET Core SignalR ve Gerçek Zamanlı Telemetri Akışları",
                        Title_EN = "Deep Dive into .NET Core SignalR and Real-time Telemetry Streams",
                        Title_DE = "Deep Dive in .NET Core SignalR und Echtzeit-Telemetrieströme",
                        Category = "architecture",
                        Date = "2026-06-15",
                        ReadTime = "8",
                        SubTag_TR = "SOKET PROGRAMLAMA & WEBSOCKETS",
                        SubTag_EN = "SOCKET PROGRAMMING & WEBSOCKETS",
                        SubTag_DE = "SOCKETPROGRAMMIERUNG & WEBSOCKETS",
                        Excerpt_TR = "Soket bağlantı el sıkışmalarını, bağlantı kopmalarını, sunucu kümesi senkronizasyonunu ve telemetri veri akışlarını yöneten hub tasarımları.",
                        Excerpt_EN = "Handshakes, reconconnections, scale-out clustering, and telemetry streams under SignalR hubs.",
                        Excerpt_DE = "Handshakes, Wiederverbindungen, Scale-out-Clustering und Telemetrieströme unter SignalR Hubs.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>Gerçek zamanlı telemetri sistemleri yüksek hızlı WebSocket kanalları gerektirir. Hub yapıları üzerinde Redis backplane kullanarak ölçekleme çözümlerini inceleyeceğiz.</p><pre><code>services.AddSignalR().AddStackExchangeRedis(\"redis-conn-string\");</code></pre>",
                        DetailText_EN = "<p>Real-time telemetry systems require reliable, persistent connection channels to transmit sensor packets. SignalR abstracts WebSockets, Server-Sent Events, and Long Polling.</p><pre><code>services.AddSignalR().AddStackExchangeRedis(\"redis-conn-string\");</code></pre>",
                        DetailText_DE = "<p>Echtzeit-Telemetriesysteme erfordern zuverlässige, persistente Verbindungskanäle zur Sensorübertragung. SignalR abstrahiert WebSockets, Server-Sent Events und Long Polling.</p><pre><code>services.AddSignalR().AddStackExchangeRedis(\"redis-conn-string\");</code></pre>"
                    },
                    new ArticleEntity
                    {
                        Id = "ai-code-agents",
                        Title_TR = "Yapay Zeka Destekli Otonom Kod İnceleme Ajanları",
                        Title_EN = "AI-Powered Autonomous Code Review Agents",
                        Title_DE = "KI-gestützte autonome Code-Review-Agenten",
                        Category = "ai",
                        Date = "2026-03-15",
                        ReadTime = "7",
                        SubTag_TR = "YAPAY ZEKA & OTONOM AJANLAR",
                        SubTag_EN = "ARTIFICIAL INTELLIGENCE & AGENTS",
                        SubTag_DE = "KÜNSTLICHE INTELLIGENZ & AGENTEN",
                        Excerpt_TR = "Gemini 1.5 Pro API'si ve Node.js agentic runtime ile PR analizlerini otomatikleştiren çok etmenli sistemlerin tasarımı.",
                        Excerpt_EN = "Designing multi-agent systems automating Pull Request reviews via Gemini API.",
                        Excerpt_DE = "Entwicklung von Multi-Agenten-Systemen zur Automatisierung von PR-Reviews via Gemini-API.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>Kod incelemelerinde yapay zeka etmenlerinin otonom orkestrasyonu.</p><pre><code>import google.generativeai as genai\nmodel = genai.GenerativeModel('gemini-1.5-pro')</code></pre>",
                        DetailText_EN = "<p>AI tooling for developers is moving from simple completions to agentic workflows.</p><pre><code>import google.generativeai as genai\nmodel = genai.GenerativeModel('gemini-1.5-pro')</code></pre>",
                        DetailText_DE = "<p>KI-Tools für Entwickler entwickeln sich von einfachen Vervollständigungen hin zu Agenten-Workflows.</p><pre><code>import google.generativeai as genai\nmodel = genai.GenerativeModel('gemini-1.5-pro')</code></pre>"
                    },
                    new ArticleEntity
                    {
                        Id = "vector-search-hnsw",
                        Title_TR = "Vektör Veritabanları ve RAG Sistemlerinde Arama Hızı",
                        Title_EN = "Vector Databases and Search Speed in RAG Systems",
                        Title_DE = "Vektordatenbanken und Suchgeschwindigkeit in RAG-Systemen",
                        Category = "ai",
                        Date = "2026-02-15",
                        ReadTime = "5",
                        SubTag_TR = "VEKTÖR VERİTABANLARI & LLM ARAMA",
                        SubTag_EN = "VECTOR DATABASES & LLM SEARCH",
                        SubTag_DE = "VEKTORDATENBANKEN & LLM SUCHE",
                        Excerpt_TR = "pgvector PostgreSQL eklentisinde HNSW ve IVFFlat indeksleme tekniklerinin vektör arama hızı ve doğruluk oranı üzerindeki etkileri.",
                        Excerpt_EN = "Analyzing pgvector indices (HNSW and IVFFlat) performance and recall properties.",
                        Excerpt_DE = "Anhaltspunkte der pgvector-Indizes (HNSW und IVFFlat) performance und recall properties.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>pgvector HNSW indeksi oluşturma ve optimizasyon rehberi.</p><pre><code>CREATE INDEX ON documents USING hnsw (embedding vector_cosine_ops);</code></pre>",
                        DetailText_EN = "<p>pgvector HNSW indexing optimization guide.</p><pre><code>CREATE INDEX ON documents USING hnsw (embedding vector_cosine_ops);</code></pre>",
                        DetailText_DE = "<p>pgvector HNSW-Indexierungsoptimierungs-Leitfaden.</p><pre><code>CREATE INDEX ON documents USING hnsw (embedding vector_cosine_ops);</code></pre>"
                    },
                    new ArticleEntity
                    {
                        Id = "css-gpu-acceleration",
                        Title_TR = "Modern Glassmorphism Efektleri ve GPU Optimizasyonu",
                        Title_EN = "Modern Glassmorphism Effects and GPU Optimization",
                        Title_DE = "Moderne Glassmorphismus-Effekte und GPU-Optimierung",
                        Category = "frontend",
                        Date = "2026-01-15",
                        ReadTime = "4",
                        SubTag_TR = "ESTETİK & DONANIMSAL HIZLANDIRMA",
                        SubTag_EN = "AESTHETICS & HARDWARE ACCELERATION",
                        SubTag_DE = "ÄSTHETIK & HARDWARE-BESCHLEUNIGUNG",
                        Excerpt_TR = "Backdrop-filter, box-shadow maskeleri ve tarayıcılarda donanımsal hızlandırmayı tetikleyerek 60 FPS sayfa kaydırma performansı sunan CSS optimizasyon teknikleri.",
                        Excerpt_EN = "CSS optimizations using GPU triggers and backdrop filters yielding 60 FPS page scrolling.",
                        Excerpt_DE = "CSS-Optimierungen mittels GPU-Triggern und Backdrop-Filtern für 60 FPS Scrolling.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>Buzlu cam tasarımlarında donanımsal ivmelendirmeyi tetikleyen CSS optimizasyonları.</p><pre><code>.glass-panel { will-change: transform, backdrop-filter; }</code></pre>",
                        DetailText_EN = "<p>Will-change optimizations triggering hardware acceleration in glassmorphism designs.</p><pre><code>.glass-panel { will-change: transform, backdrop-filter; }</code></pre>",
                        DetailText_DE = "<p>Will-change-Optimierungen zur Aktivierung der Hardwarebeschleunigung bei Glassmorphismus.</p><pre><code>.glass-panel { will-change: transform, backdrop-filter; }</code></pre>"
                    },
                    new ArticleEntity
                    {
                        Id = "rust-zero-copy",
                        Title_TR = "Rust ile Sıfır Kopyalama Derleyici Ön Ucu Geliştirmek",
                        Title_EN = "Developing Zero-Copy Compiler Frontend in Rust",
                        Title_DE = "Entwicklung eines Zero-Copy-Compiler-Frontends in Rust",
                        Category = "other",
                        Date = "2025-08-15",
                        ReadTime = "10",
                        SubTag_TR = "DERLEYİCİ MÜHENDİSLİĞİ & BELLEK YÖNETİMİ",
                        SubTag_EN = "COMPILER ENGINEERING & MEMORY MANAGEMENT",
                        SubTag_DE = "COMPILER-ENGINEERING & SPEICHERVERWALTUNG",
                        Excerpt_TR = "Rust dilinin ownership kurallarını kullanarak sıfır bellek kopyalamalı Lexer/Parser mimarisi ve AST üretim süreçlerinin analizi.",
                        Excerpt_EN = "Zero-copy Lexer/Parser architecture and AST generation utilizing Rust ownership rules.",
                        Excerpt_DE = "Zero-Copy Lexer/Parser-Architektur und AST-Generierung unter Ausnutzung von Rust Ownership-Regeln.",
                        ImageUrl = "assets/blog_placeholder.png",
                        DetailText_TR = "<p>Rust lifetime kuralları ile sıfır kopyalamalı token okuma yapıları.</p><pre><code>pub enum Token&lt;'a&gt; { Identifier(&'a str) }</code></pre>",
                        DetailText_EN = "<p>Zero-copy token scans leveraging Rust lifetimes.</p><pre><code>pub enum Token&lt;'a&gt; { Identifier(&'a str) }</code></pre>",
                        DetailText_DE = "<p>Zero-Copy-Token-Scans unter Verwendung von Rust Lifetimes.</p><pre><code>pub enum Token&lt;'a&gt; { Identifier(&'a str) }</code></pre>"
                    }
                });
            }
        }
    }
}
