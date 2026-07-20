import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { DataService, Project } from '../../../shared/services/data.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

import { LocalizationService } from '../../../shared/services/localization.service';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslatePipe],
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit, OnDestroy {
  projects: Project[] = [];
  filteredProjects: Project[] = [];
  activeCategory = 'all';
  searchQuery = '';

  private subscription = new Subscription();

  constructor(private dataService: DataService, private localizationService: LocalizationService) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  getCategoryLabel(category: string): string {
    if (!category) return '';
    switch (category.trim()) {
      case 'AI & Machine Learning':
      case 'ai-rag':
      case 'ml-dl':
        return this.localizationService.translate('CAT_AI_ML');
      case 'Web Development':
      case 'web':
        return this.localizationService.translate('CAT_WEB_DEV');
      case 'Software Architecture':
      case 'architecture':
        return this.localizationService.translate('CAT_SOFTWARE_ARCH');
      case 'DevOps & Infrastructure':
      case 'devops':
        return this.localizationService.translate('CAT_DEVOPS_INFRA');
      case 'Diğer':
      case 'other':
        return this.localizationService.translate('CAT_OTHER');
      default:
        return category;
    }
  }

  ngOnInit() {
    this.subscription.add(
      this.dataService.dataUpdated$.subscribe(() => {
        this.projects = this.dataService.getProjects();
        this.filterList();
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  selectCategory(cat: string) {
    this.activeCategory = cat;
    this.filterList();
  }

  onSearchChange() {
    this.filterList();
  }

  filterList() {
    this.filteredProjects = this.projects.filter(proj => {
      // 1. Multi-category check
      const matchCat = this.activeCategory === 'all' || 
        (proj.category && proj.category.split(',').map(c => c.trim()).includes(this.activeCategory));

      // 2. Query check
      const q = this.searchQuery.toLowerCase().trim();
      const matchQuery = !q ||
        proj.title.toLowerCase().includes(q) ||
        proj.description.toLowerCase().includes(q) ||
        proj.tech.toLowerCase().includes(q) ||
        proj.client.toLowerCase().includes(q);

      return matchCat && matchQuery;
    });
  }

  getProjectCategories(categoryStr: string): string[] {
    if (!categoryStr) return [];
    return categoryStr.split(',').map(c => c.trim()).filter(c => c.length > 0);
  }

  getTechList(techString: string): string[] {
    if (!techString) return [];
    return techString.split(',').map(t => t.trim());
  }
}
