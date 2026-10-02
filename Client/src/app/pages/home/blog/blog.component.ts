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
  activeMainCategory = 'all';
  activeSubCategory = 'all';
  searchQuery = '';

  private subscription = new Subscription();

  constructor(
    private dataService: DataService,
    private localizationService: LocalizationService
  ) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  categories: any[] = [];

  ngOnInit() {
    this.dataService.getCategories().then(cats => {
      this.categories = cats;
    });
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

  selectMainCategory(catId: string) {
    this.activeMainCategory = catId;
    this.activeSubCategory = 'all'; // Ana kategori değişince alt kategoriyi sıfırla
    this.filterArticles();
  }

  selectSubCategory(catId: string) {
    this.activeSubCategory = catId;
    this.filterArticles();
  }

  onSearchChange() {
    this.filterArticles();
  }

  filterArticles() {
    this.filteredArticles = this.articles.filter(art => {
      let matchCat = false;

      if (this.activeMainCategory === 'all') {
        matchCat = true;
      } else {
        if (this.activeSubCategory === 'all') {
          // Check if article's subcategory belongs to activeMainCategory
          const main = this.categories.find(c => c.id === this.activeMainCategory);
          if (main && main.subCategories) {
            matchCat = main.subCategories.some((s: any) => s.id === art.categoryId);
          }
        } else {
          // Check if article belongs to the specific subcategory
          matchCat = art.categoryId === this.activeSubCategory;
        }
      }

      const q = this.searchQuery.toLowerCase().trim();
      const matchQuery = !q ||
        art.title.toLowerCase().includes(q) ||
        art.excerpt.toLowerCase().includes(q);

      return matchCat && matchQuery;
    });
  }

  getAvailableMainCategories(): {id: string, name: string}[] {
    const usedSubCats = new Set<string>();
    for (const art of this.articles) {
      if (art.categoryId) usedSubCats.add(art.categoryId);
    }
    
    const result: {id: string, name: string}[] = [];
    for (const main of this.categories) {
       const hasUsedSub = main.subCategories?.some((s: any) => usedSubCats.has(s.id));
       if (hasUsedSub) {
           const langSuffix = this.localizationService.getLanguage().toUpperCase();
           const name = main['name_' + langSuffix] || main.name_TR;
           result.push({id: main.id, name: name});
       }
    }
    return result;
  }

  getAvailableSubCategories(): {id: string, name: string}[] {
    if (this.activeMainCategory === 'all') return [];
    
    const usedSubCats = new Set<string>();
    for (const art of this.articles) {
      if (art.categoryId) usedSubCats.add(art.categoryId);
    }

    const main = this.categories.find(c => c.id === this.activeMainCategory);
    if (!main || !main.subCategories) return [];

    const result: {id: string, name: string}[] = [];
    for (const sub of main.subCategories) {
        if (usedSubCats.has(sub.id)) {
            const langSuffix = this.localizationService.getLanguage().toUpperCase();
            const name = sub['name_' + langSuffix] || sub.name_TR;
            result.push({id: sub.id, name: name});
        }
    }
    return result;
  }

  getCategoryLabel(categoryId?: string): string {
    if (!categoryId) return 'Genel';
    const langSuffix = this.localizationService.getLanguage().toUpperCase();
    for (const main of this.categories) {
      const sub = main.subCategories?.find((s: any) => s.id === categoryId);
      if (sub) return sub['name_' + langSuffix] || sub.name_TR;
    }
    return categoryId;
  }
}
