import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, Profile } from '../../../shared/services/data.service';
import { CvStructuredData } from '../../../shared/models/profile.model';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
import { LocalizationService } from '../../../shared/services/localization.service';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, TranslatePipe, FormsModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  selectedCvLang: 'tr' | 'en' | 'de' = 'tr';
  isCvLangDropdownOpen = false;

  toggleCvLangDropdown(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.isCvLangDropdownOpen = !this.isCvLangDropdownOpen;
  }

  selectCvLang(lang: 'tr' | 'en' | 'de') {
    this.selectedCvLang = lang;
    this.isCvLangDropdownOpen = false;
  }

  getSelectedLanguageLabel(): string {
    const labels: Record<string, string> = {
      tr: '🇹🇷 Türkçe',
      en: '🇬🇧 English',
      de: '🇩🇪 Deutsch'
    };
    return labels[this.selectedCvLang] || '🇹🇷 Türkçe';
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick() {
    this.isCvLangDropdownOpen = false;
  }

  get profile(): Profile {
    return this.dataService.getProfile();
  }

  get cvData(): CvStructuredData | null {
    if (this.profile && this.profile.cvText) {
      try {
        return JSON.parse(this.profile.cvText);
      } catch (e) {
        console.error("Failed to parse public CV JSON", e);
      }
    }
    return null;
  }

  getCvDownloadUrl(lang: 'tr' | 'en' | 'de'): string {
    if (!this.profile) return '#';
    let path = '';
    if (lang === 'tr') path = this.profile.cvPdfUrl_TR || '';
    else if (lang === 'en') path = this.profile.cvPdfUrl_EN || '';
    else if (lang === 'de') path = this.profile.cvPdfUrl_DE || '';

    if (!path) return `assets/cv_${lang}.pdf`;

    if (path.startsWith('http') || path.startsWith('assets/')) {
      return path;
    }
    return `${this.dataService.apiBaseUrl}/${path}`;
  }

  get activeLanguage(): string {
    return this.localizationService.getLanguage();
  }

  constructor(
    private dataService: DataService,
    private localizationService: LocalizationService
  ) {}

  ngOnInit() {
    const current = this.activeLanguage;
    if (current === 'tr' || current === 'en' || current === 'de') {
      this.selectedCvLang = current;
    } else {
      this.selectedCvLang = 'tr';
    }
  }
}
