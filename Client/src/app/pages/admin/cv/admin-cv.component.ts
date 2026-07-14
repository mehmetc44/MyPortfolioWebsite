import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, RawProfile } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-cv',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-cv.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminCvComponent implements OnInit {
  @ViewChild('editorTR') editorTR!: ElementRef<HTMLDivElement>;
  @ViewChild('editorEN') editorEN!: ElementRef<HTMLDivElement>;
  @ViewChild('editorDE') editorDE!: ElementRef<HTMLDivElement>;

  profile: RawProfile | null = null;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';
  isUploading = { tr: false, en: false, de: false };
  isSaving = false;

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    await this.loadProfile();
  }

  async loadProfile() {
    const raw = await this.dataService.getRawProfile();
    if (raw) {
      this.profile = raw;
      this.initializeEditors();
    }
  }

  initializeEditors() {
    setTimeout(() => {
      if (this.profile) {
        if (this.editorTR) this.editorTR.nativeElement.innerHTML = this.profile.cvText_TR || '';
        if (this.editorEN) this.editorEN.nativeElement.innerHTML = this.profile.cvText_EN || '';
        if (this.editorDE) this.editorDE.nativeElement.innerHTML = this.profile.cvText_DE || '';
      }
    }, 150);
  }

  onEditorInput(lang: 'tr' | 'en' | 'de', html: string) {
    if (this.profile) {
      if (lang === 'tr') this.profile.cvText_TR = html;
      else if (lang === 'en') this.profile.cvText_EN = html;
      else if (lang === 'de') this.profile.cvText_DE = html;
    }
  }

  execCmd(command: string, value: string = '') {
    document.execCommand(command, false, value);
    this.updateValuesFromDOM();
  }

  updateValuesFromDOM() {
    if (this.profile) {
      if (this.editorTR) this.profile.cvText_TR = this.editorTR.nativeElement.innerHTML;
      if (this.editorEN) this.profile.cvText_EN = this.editorEN.nativeElement.innerHTML;
      if (this.editorDE) this.profile.cvText_DE = this.editorDE.nativeElement.innerHTML;
    }
  }

  async onFileSelected(event: any, lang: 'tr' | 'en' | 'de') {
    const file = event.target.files[0];
    if (!file || !this.profile) return;

    this.isUploading[lang] = true;
    let oldUrl = '';
    if (lang === 'tr') oldUrl = this.profile.cvPdfUrl_TR || '';
    else if (lang === 'en') oldUrl = this.profile.cvPdfUrl_EN || '';
    else if (lang === 'de') oldUrl = this.profile.cvPdfUrl_DE || '';

    const uploadedUrl = await this.dataService.uploadCv(file, oldUrl);
    this.isUploading[lang] = false;

    if (uploadedUrl) {
      if (lang === 'tr') this.profile.cvPdfUrl_TR = uploadedUrl;
      else if (lang === 'en') this.profile.cvPdfUrl_EN = uploadedUrl;
      else if (lang === 'de') this.profile.cvPdfUrl_DE = uploadedUrl;
      alert(`PDF başarıyla yüklendi: ${uploadedUrl}`);
    } else {
      alert('PDF dosyası yüklenirken bir hata oluştu.');
    }
  }

  async saveCv() {
    if (!this.profile) return;

    this.isSaving = true;
    this.updateValuesFromDOM();
    
    const ok = await this.dataService.saveRawProfile(this.profile);
    this.isSaving = false;

    if (ok) {
      alert('Özgeçmiş bilgileri başarıyla kaydedildi.');
      // Refresh current public profile values
      await this.dataService.loadProfile('tr');
    } else {
      alert('Kaydedilirken bir sunucu hatası oluştu.');
    }
  }
}
