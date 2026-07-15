import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DataService, Article } from '../../../shared/services/data.service';
import { LocalizationService } from '../../../shared/services/localization.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslatePipe],
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css']
})
export class BlogComponent implements OnInit {
  articles: Article[] = [];
  filteredArticles: Article[] = [];
  activeCategory = 'all';
  searchQuery = '';

  constructor(
    private dataService: DataService,
    private localizationService: LocalizationService
  ) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  ngOnInit() {
    this.articles = this.dataService.getArticles();
    this.filterArticles();
  }

  selectCategory(cat: string) {
    this.activeCategory = cat;
    this.filterArticles();
  }

  onSearchChange() {
    this.filterArticles();
  }

  filterArticles() {
    this.filteredArticles = this.articles.filter(art => {
      // 1. Category check
      const matchCat = this.activeCategory === 'all' || art.category === this.activeCategory;

      // 2. Query check
      const q = this.searchQuery.toLowerCase().trim();
      const matchQuery = !q ||
        art.title.toLowerCase().includes(q) ||
        art.excerpt.toLowerCase().includes(q) ||
        art.subTag.toLowerCase().includes(q);

      return matchCat && matchQuery;
    });
  }

  getCategoryLabel(category: string): string {
    switch (category) {
      case 'architecture': return this.localizationService.translate('CAT_ARCHITECTURE');
      case 'ai': return this.localizationService.translate('CAT_AI');
      case 'frontend': return this.localizationService.translate('CAT_FRONTEND');
      default: return this.localizationService.translate('CAT_OTHER');
    }
  }
}
