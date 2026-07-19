import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, RawProfile } from '../../../shared/services/data.service';
import { 
  CvStructuredData, 
  CvItemExperience, 
  CvItemEducation, 
  CvItemCertificate, 
  CvItemVolunteering, 
  CvItemLanguage 
} from '../../../shared/models/profile.model';

@Component({
  selector: 'app-admin-cv',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-cv.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminCvComponent implements OnInit {
  profile: RawProfile | null = null;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';
  isUploading = { tr: false, en: false, de: false };
  isSaving = false;

  cvTR: CvStructuredData = { experiences: [], educations: [], certificates: [], volunteering: [], languages: [] };
  cvEN: CvStructuredData = { experiences: [], educations: [], certificates: [], volunteering: [], languages: [] };
  cvDE: CvStructuredData = { experiences: [], educations: [], certificates: [], volunteering: [], languages: [] };

  // Drag & Drop state — her section için ayrı
  drag: Record<string, { dragIndex: number | null, dragOverIndex: number | null }> = {
    experiences:   { dragIndex: null, dragOverIndex: null },
    educations:    { dragIndex: null, dragOverIndex: null },
    certificates:  { dragIndex: null, dragOverIndex: null },
    volunteering:  { dragIndex: null, dragOverIndex: null },
    languages:     { dragIndex: null, dragOverIndex: null },
  };

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    await this.loadProfile();
  }

  async loadProfile() {
    const raw = await this.dataService.getRawProfile();
    if (raw) {
      this.profile = raw;
      this.cvTR = this.parseCvData(raw.cvText_TR);
      this.cvEN = this.parseCvData(raw.cvText_EN);
      this.cvDE = this.parseCvData(raw.cvText_DE);
    }
  }

  parseCvData(jsonStr?: string): CvStructuredData {
    const defaultData: CvStructuredData = { experiences: [], educations: [], certificates: [], volunteering: [], languages: [] };
    if (!jsonStr) return defaultData;
    try {
      const parsed = JSON.parse(jsonStr);
      return {
        experiences: parsed.experiences || [],
        educations: parsed.educations || [],
        certificates: parsed.certificates || [],
        volunteering: parsed.volunteering || [],
        languages: parsed.languages || []
      };
    } catch (e) {
      console.error("Failed to parse CV JSON", e);
      return defaultData;
    }
  }

  get currentCv(): CvStructuredData {
    if (this.activeFormTab === 'tr') return this.cvTR;
    if (this.activeFormTab === 'en') return this.cvEN;
    return this.cvDE;
  }

  // --- List Item Addition & Removal Methods ---

  addExperience() {
    this.currentCv.experiences.push({ title: '', org: '', date: '', bullets: [''] });
  }

  removeExperience(index: number) {
    this.currentCv.experiences.splice(index, 1);
  }

  addBullet(exp: CvItemExperience) {
    exp.bullets.push('');
  }

  removeBullet(exp: CvItemExperience, index: number) {
    exp.bullets.splice(index, 1);
  }

  addEducation() {
    this.currentCv.educations.push({ title: '', org: '', date: '', desc: '' });
  }

  removeEducation(index: number) {
    this.currentCv.educations.splice(index, 1);
  }

  addCertificate() {
    this.currentCv.certificates.push({ title: '', date: '' });
  }

  removeCertificate(index: number) {
    this.currentCv.certificates.splice(index, 1);
  }

  addVolunteering() {
    this.currentCv.volunteering.push({ title: '', org: '', date: '', desc: '' });
  }

  removeVolunteering(index: number) {
    this.currentCv.volunteering.splice(index, 1);
  }

  addLanguage() {
    this.currentCv.languages.push({ name: '', level: '', percentage: 80 });
  }

  removeLanguage(index: number) {
    this.currentCv.languages.splice(index, 1);
  }

  trackByFn(index: any, item: any) {
    return index;
  }

  // ── Drag & Drop Handlers (Generic per-section) ────────────────

  onSectionDragStart(section: string, index: number) {
    this.drag[section].dragIndex = index;
  }

  onSectionDragOver(event: DragEvent, section: string, index: number) {
    event.preventDefault();
    this.drag[section].dragOverIndex = index;
  }

  onSectionDragLeave(section: string) {
    this.drag[section].dragOverIndex = null;
  }

  onSectionDrop(event: DragEvent, section: string, dropIndex: number) {
    event.preventDefault();
    const from = this.drag[section].dragIndex;
    if (from === null || from === dropIndex) {
      this.drag[section].dragIndex = null;
      this.drag[section].dragOverIndex = null;
      return;
    }

    const arr = (this.currentCv as any)[section] as any[];
    const moved = arr.splice(from, 1)[0];
    arr.splice(dropIndex, 0, moved);

    this.drag[section].dragIndex = null;
    this.drag[section].dragOverIndex = null;
  }

  onSectionDragEnd(section: string) {
    this.drag[section].dragIndex = null;
    this.drag[section].dragOverIndex = null;
  }

  isDragging(section: string, index: number): boolean {
    return this.drag[section].dragIndex === index;
  }

  isDragOver(section: string, index: number): boolean {
    return this.drag[section].dragOverIndex === index && this.drag[section].dragIndex !== null;
  }

  // --- PDF CV File Uploader ---
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

  // --- Save Changes ---
  async saveCv() {
    if (!this.profile) return;

    this.isSaving = true;
    
    // Stringify structured arrays back to JSON columns
    this.profile.cvText_TR = JSON.stringify(this.cvTR);
    this.profile.cvText_EN = JSON.stringify(this.cvEN);
    this.profile.cvText_DE = JSON.stringify(this.cvDE);

    const ok = await this.dataService.saveRawProfile(this.profile);
    this.isSaving = false;

    if (ok) {
      alert('Özgeçmiş bilgileri başarıyla kaydedildi.');
      await this.dataService.loadDataFromServer();
    } else {
      alert('Kaydedilirken bir sunucu hatası oluştu.');
    }
  }
}
