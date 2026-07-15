import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, RawProject } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-projects',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-projects.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminProjectsComponent implements OnInit {
  projects: RawProject[] = [];
  showProjectModal = false;
  projectModalTitle = 'Yeni Proje Ekle';
  editingProjectIdx = -1;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';

  projId = '';
  projTitle_TR = '';
  projTitle_EN = '';
  projTitle_DE = '';
  projCategory = 'web';
  projDate = '';
  projClient = '';
  projSubTag_TR = '';
  projSubTag_EN = '';
  projSubTag_DE = '';
  projDesc_TR = '';
  projDesc_EN = '';
  projDesc_DE = '';
  projDetail_TR = '';
  projDetail_EN = '';
  projDetail_DE = '';
  projTech = '';
  projRepo = '';
  projDemo = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadProjects();
  }

  async loadProjects() {
    this.projects = await this.dataService.getRawProjects();
  }

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  openNewProjectModal() {
    this.editingProjectIdx = -1;
    this.projectModalTitle = 'Yeni Proje Ekle';
    this.activeFormTab = 'tr';
    
    this.projId = '';
    this.projTitle_TR = '';
    this.projTitle_EN = '';
    this.projTitle_DE = '';
    this.projCategory = 'web';
    this.projDate = '';
    this.projClient = '';
    this.projSubTag_TR = '';
    this.projSubTag_EN = '';
    this.projSubTag_DE = '';
    this.projDesc_TR = '';
    this.projDesc_EN = '';
    this.projDesc_DE = '';
    this.projDetail_TR = '';
    this.projDetail_EN = '';
    this.projDetail_DE = '';
    this.projTech = '';
    this.projRepo = '';
    this.projDemo = '';
    this.showProjectModal = true;
  }

  openEditProjectModal(idx: number) {
    this.editingProjectIdx = idx;
    this.projectModalTitle = 'Proje Düzenle';
    this.activeFormTab = 'tr';
    const proj = this.projects[idx];
    
    this.projId = proj.id;
    this.projTitle_TR = proj.title_TR;
    this.projTitle_EN = proj.title_EN;
    this.projTitle_DE = proj.title_DE;
    this.projCategory = proj.category;
    this.projDate = proj.date;
    this.projClient = proj.client;
    this.projSubTag_TR = proj.subTag_TR;
    this.projSubTag_EN = proj.subTag_EN;
    this.projSubTag_DE = proj.subTag_DE;
    this.projDesc_TR = proj.description_TR;
    this.projDesc_EN = proj.description_EN;
    this.projDesc_DE = proj.description_DE;
    this.projDetail_TR = proj.detailText_TR;
    this.projDetail_EN = proj.detailText_EN;
    this.projDetail_DE = proj.detailText_DE;
    this.projTech = proj.tech;
    this.projRepo = proj.repoUrl;
    this.projDemo = proj.demoUrl || '';
    this.showProjectModal = true;
  }

  closeProjectModal() {
    this.showProjectModal = false;
  }

  async saveProject() {
    const slugId = this.projId.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '') || 
                   this.projTitle_TR.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
                   
    const projData: RawProject = {
      id: this.editingProjectIdx >= 0 ? this.projects[this.editingProjectIdx].id : slugId,
      title_TR: this.projTitle_TR,
      title_EN: this.projTitle_EN,
      title_DE: this.projTitle_DE,
      category: this.projCategory,
      date: this.projDate,
      client: this.projClient,
      subTag_TR: this.projSubTag_TR,
      subTag_EN: this.projSubTag_EN,
      subTag_DE: this.projSubTag_DE,
      description_TR: this.projDesc_TR,
      description_EN: this.projDesc_EN,
      description_DE: this.projDesc_DE,
      detailText_TR: this.projDetail_TR,
      detailText_EN: this.projDetail_EN,
      detailText_DE: this.projDetail_DE,
      tech: this.projTech,
      repoUrl: this.projRepo,
      demoUrl: this.projDemo || `portfolio/${this.editingProjectIdx >= 0 ? this.projects[this.editingProjectIdx].id : slugId}`,
      imagesJson: this.editingProjectIdx >= 0 ? (this.projects[this.editingProjectIdx].imagesJson || "[]") : "[\"assets/project_placeholder.png\", \"assets/project_slide1.png\", \"assets/project_slide2.png\"]"
    };

    const isNew = this.editingProjectIdx < 0;
    const ok = await this.dataService.saveRawProject(projData, isNew);
    if (ok) {
      await this.loadProjects();
      this.closeProjectModal();
    } else {
      alert("Proje kaydedilirken sunucu hatası oluştu.");
    }
  }

  async deleteProject(idx: number) {
    if (confirm('Bu projeyi silmek istediğinizden emin misiniz?')) {
      const proj = this.projects[idx];
      const ok = await this.dataService.deleteRawProject(proj.id);
      if (ok) {
        await this.loadProjects();
      } else {
        alert("Proje silinirken sunucu hatası oluştu.");
      }
    }
  }
}
