import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService, Profile } from '../../../shared/services/data.service';
import { CvStructuredData } from '../../../shared/models/profile.model';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
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
    if (typeof window !== 'undefined') {
      return localStorage.getItem('app_language') || 'tr';
    }
    return 'tr';
  }

  constructor(private dataService: DataService) {}

  ngOnInit() {
  }
}
