import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
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
export class BlogComponent implements OnInit, OnDestroy {
  articles: Article[] = [];
  filteredArticles: Article[] = [];
  activeCategory = 'all';
  searchQuery = '';

  private subscription = new Subscription();

  constructor(
    private dataService: DataService,
    private localizationService: LocalizationService
  ) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  ngOnInit() {
    this.subscription.add(
      this.dataService.dataUpdated$.subscribe(() => {
        this.articles = this.dataService.getArticles();
        this.filterArticles();
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
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
      // 1. Category check — virgülle ayrılmış çoklu kategori desteği
      const artCats = (art.category || '').split(',').map(c => c.trim()).filter(c => !!c);
      const matchCat = this.activeCategory === 'all' || artCats.includes(this.activeCategory);

      // 2. Query check
      const q = this.searchQuery.toLowerCase().trim();
      const matchQuery = !q ||
        art.title.toLowerCase().includes(q) ||
        art.excerpt.toLowerCase().includes(q) ||
        art.subTag.toLowerCase().includes(q);

      return matchCat && matchQuery;
    });
  }

  /** Yayınlanan makalelerden benzersiz kategorileri döndürür (çoklu kategori desteği) */
  getAvailableCategories(): string[] {
    const cats = new Set<string>();
    for (const art of this.articles) {
      if (!art.category) continue;
      art.category.split(',').map(c => c.trim()).filter(c => !!c).forEach(c => cats.add(c));
    }
    return Array.from(cats).sort();
  }

  getCategoryLabel(category: string): string {
    if (!category || !category.trim()) return 'Genel';
    return category;
  }
}
