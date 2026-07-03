import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type LanguageCode = 'tr' | 'en' | 'de';

@Injectable({
  providedIn: 'root'
})
export class LocalizationService {
  private readonly defaultLang: LanguageCode = 'tr';
  private readonly langKey = 'app_language';

  private currentLangSubject: BehaviorSubject<LanguageCode>;
  public currentLang$: Observable<LanguageCode>;

  private readonly translations: Record<LanguageCode, Record<string, string>> = {
    tr: {
      "NAV_HOME": "ANASAYFA",
      "NAV_ABOUT": "HAKKIMDA",
      "NAV_PORTFOLIO": "PORTFOLYO",
      "NAV_BLOG": "BLOG",
      "NAV_CONTACT": "İLETİŞİM",
      "HERO_SUBTITLE": "YAZILIM MİMARİSİ & MÜHENDİSLİK",
      "HERO_TITLE": "Designing & Constructing Scalable Web Environments",
      "CORE_STACK": "Teknoloji Havuzu",
      "OPERATIONAL_MATRIX": "Çalışma Matrisi",
      "ENGINEERING_ACTIVITIES": "Mühendislik Günlüğü",
      "HIRE_ME": "İletişime Geç",
      "SEARCH_PLACEHOLDER": "Proje veya makale ara...",
      "CV_DOWNLOAD": "Özgeçmiş (CV) İndir",
      "READ_ARTICLE": "Makaleyi Oku",
      "VIEW_DETAILS": "Detayları İncele",
      "BACK_TO_HOME": "Anasayfaya Dön",
      "PROJECT_INFO": "Proje Bilgileri",
      "CLIENT": "Müşteri",
      "DATE": "Tarih",
      "TECH_STACK": "Kullanılan Teknolojiler",
      "SOURCE_CODE": "Kaynak Kodu",
      "DEMO": "Canlı Demo",
      "CONTACT_TITLE": "İletişime Geçin",
      "CONTACT_SUB": "Projeleriniz veya işbirlikleri için mesaj gönderebilirsiniz.",
      "YOUR_NAME": "Adınız",
      "YOUR_EMAIL": "E-posta Adresiniz",
      "MESSAGE": "Mesajınız",
      "SEND_MESSAGE": "Mesajı Gönder",
      "OR_DIRECT": "VEYA DOĞRUDAN CV İNDİRİN",
      "CLOSE": "Kapat",
      "LOGGED_DAYS": "365 Günlük Log"
    },
    en: {
      "NAV_HOME": "HOME",
      "NAV_ABOUT": "ABOUT ME",
      "NAV_PORTFOLIO": "PORTFOLIO",
      "NAV_BLOG": "BLOG",
      "NAV_CONTACT": "CONTACT",
      "HERO_SUBTITLE": "SOFTWARE ARCHITECTURE & ENGINEERING",
      "HERO_TITLE": "Designing & Constructing Scalable Web Environments",
      "CORE_STACK": "Core Engineering Stack",
      "OPERATIONAL_MATRIX": "Operational Matrix",
      "ENGINEERING_ACTIVITIES": "Engineering Activities",
      "HIRE_ME": "Get in Touch",
      "SEARCH_PLACEHOLDER": "Search projects or articles...",
      "CV_DOWNLOAD": "Download Resume (CV)",
      "READ_ARTICLE": "Read Article",
      "VIEW_DETAILS": "View Details",
      "BACK_TO_HOME": "Back to Home",
      "PROJECT_INFO": "Project Info",
      "CLIENT": "Client",
      "DATE": "Date",
      "TECH_STACK": "Tech Stack",
      "SOURCE_CODE": "Source Code",
      "DEMO": "Live Demo",
      "CONTACT_TITLE": "Get in Touch",
      "CONTACT_SUB": "Let's discuss how we can work together or collaborate on your next project.",
      "YOUR_NAME": "Your Name",
      "YOUR_EMAIL": "Your Email",
      "MESSAGE": "Message",
      "SEND_MESSAGE": "Send Message",
      "OR_DIRECT": "OR DOWNLOAD DIRECT CV",
      "CLOSE": "Close",
      "LOGGED_DAYS": "365 Days Log"
    },
    de: {
      "NAV_HOME": "STARTSEITE",
      "NAV_ABOUT": "ÜBER MICH",
      "NAV_PORTFOLIO": "PORTFOLIO",
      "NAV_BLOG": "BLOG",
      "NAV_CONTACT": "KONTAKT",
      "HERO_SUBTITLE": "SOFTWAREARCHITEKTUR & ENGINEERING",
      "HERO_TITLE": "Konzeption & Entwicklung skalierbarer Webumgebungen",
      "CORE_STACK": "Technologie-Stack",
      "OPERATIONAL_MATRIX": "Aktivitätsmatrix",
      "ENGINEERING_ACTIVITIES": "Engineering-Aktivitäten",
      "HIRE_ME": "Kontaktieren",
      "SEARCH_PLACEHOLDER": "Projekte oder Artikel suchen...",
      "CV_DOWNLOAD": "Lebenslauf (CV) herunterladen",
      "READ_ARTICLE": "Artikel lesen",
      "VIEW_DETAILS": "Details anzeigen",
      "BACK_TO_HOME": "Zurück zur Startseite",
      "PROJECT_INFO": "Projektinfo",
      "CLIENT": "Kunde",
      "DATE": "Datum",
      "TECH_STACK": "Verwendete Technologien",
      "SOURCE_CODE": "Quellcode",
      "DEMO": "Live-Demo",
      "CONTACT_TITLE": "Kontakt aufnehmen",
      "CONTACT_SUB": "Lassen Sie uns besprechen, wie wir zusammenarbeiten können.",
      "YOUR_NAME": "Ihr Name",
      "YOUR_EMAIL": "Ihre E-Mail",
      "MESSAGE": "Nachricht",
      "SEND_MESSAGE": "Nachricht senden",
      "OR_DIRECT": "ODER CV DIREKT HERUNTERLADEN",
      "CLOSE": "Schließen",
      "LOGGED_DAYS": "365 Tage Log"
    }
  };

  constructor() {
    let savedLang = this.defaultLang;
    if (typeof window !== 'undefined' && window.localStorage) {
      const saved = localStorage.getItem(this.langKey) as LanguageCode;
      if (saved === 'tr' || saved === 'en' || saved === 'de') {
        savedLang = saved;
      }
    }
    this.currentLangSubject = new BehaviorSubject<LanguageCode>(savedLang);
    this.currentLang$ = this.currentLangSubject.asObservable();
  }

  public getLanguage(): LanguageCode {
    return this.currentLangSubject.value;
  }

  public setLanguage(lang: LanguageCode): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.setItem(this.langKey, lang);
    }
    this.currentLangSubject.next(lang);
  }

  public translate(key: string): string {
    const lang = this.getLanguage();
    return this.translations[lang]?.[key] || key;
  }
}
