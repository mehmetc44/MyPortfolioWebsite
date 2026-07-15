import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, RawProfile, TechTag } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-profile.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminProfileComponent implements OnInit {
  profile: RawProfile | null = null;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';
  isUploading = false;

  // Tech tags management
  techTags: TechTag[] = [];
  selectedTag: TechTag | null = null;
  isTagNew = false;

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadProfile();
    this.loadTechTags();
  }

  async loadProfile() {
    this.profile = await this.dataService.getRawProfile();
  }

  async loadTechTags() {
    this.techTags = await this.dataService.getRawTechTags();
  }

  async saveProfile() {
    if (!this.profile) return;
    const ok = await this.dataService.saveRawProfile(this.profile);
    if (ok) {
      alert('Profil bilgileri başarıyla veritabanına kaydedildi!');
      await this.loadProfile();
    } else {
      alert('Profil kaydedilirken bir hata oluştu.');
    }
  }

  async onFileSelected(event: any) {
    if (!this.profile) return;
    const file = event.target.files[0];
    if (file) {
      this.isUploading = true;
      const oldUrl = this.profile.avatarUrl;
      const uploadedPath = await this.dataService.uploadAvatar(file, oldUrl);
      this.isUploading = false;
      
      if (uploadedPath) {
        this.profile.avatarUrl = `${this.dataService.apiBaseUrl}/${uploadedPath}`;
      } else {
        alert('Görsel yüklenirken sunucu hatası oluştu.');
      }
    }
  }

  // Tech tag methods
  selectTag(tag: TechTag) {
    this.selectedTag = { ...tag };
    this.isTagNew = false;
  }

  createNewTag() {
    this.selectedTag = {
      id: 0,
      name: ''
    };
    this.isTagNew = true;
  }

  cancelTagEdit() {
    this.selectedTag = null;
  }

  async saveTag() {
    if (!this.selectedTag) return;
    if (!this.selectedTag.name.trim()) {
      alert('Teknoloji paketi adı boş olamaz.');
      return;
    }

    const success = await this.dataService.saveRawTechTag(this.selectedTag, this.isTagNew);
    if (success) {
      this.selectedTag = null;
      await this.loadTechTags();
    } else {
      alert('Teknoloji paketi kaydedilirken bir hata oluştu.');
    }
  }

  async deleteTag(id: number) {
    if (confirm('Bu teknoloji paketini silmek istediğinize emin misiniz?')) {
      const success = await this.dataService.deleteRawTechTag(id);
      if (success) {
        if (this.selectedTag?.id === id) {
          this.selectedTag = null;
        }
        await this.loadTechTags();
      } else {
        alert('Teknoloji paketi silinirken bir hata oluştu.');
      }
    }
  }
}
