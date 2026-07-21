import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';
import { Subscription } from 'rxjs';
import { DataService, Article, sanitizeImageUrl } from '../../../shared/services/data.service';
import { LocalizationService } from '../../../shared/services/localization.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-blog-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './blog-detail.component.html',
  styleUrls: ['./blog-detail.component.css']
})
export class BlogDetailComponent implements OnInit, OnDestroy {
  article?: Article;
  sanitizedDetailText?: SafeHtml;

  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private sanitizer: DomSanitizer,
    private localizationService: LocalizationService
  ) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  ngOnInit() {
    this.subscription.add(
      this.route.paramMap.subscribe(params => {
        this.loadArticle(params.get('id'));
      })
    );

    this.subscription.add(
      this.dataService.dataUpdated$.subscribe(() => {
        const id = this.route.snapshot.paramMap.get('id');
        this.loadArticle(id);
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  loadArticle(id: string | null) {
    if (id) {
      const found = this.dataService.getArticle(id);
      if (found) {
        this.article = found;
        try {
          const detailText = this.resolveDetailImages(found.detailText || '');
          const parsedHtml = marked.parse(detailText, { async: false }) as string;
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(parsedHtml);
        } catch (_) {
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText || '');
        }
      } else {
        this.router.navigate(['/blog']);
      }
    }
  }

  private resolveDetailImages(text: string): string {
    if (!text) return '';
    return text.replace(/(src=["']|!\[.*?\]\()([^"'\)]+)(["']|\))/gi, (match, prefix, url, suffix) => {
      const sanitized = sanitizeImageUrl(url, this.dataService.apiBaseUrl);
      return `${prefix}${sanitized}${suffix}`;
    });
  }

  getCategoryLabel(category: string): string {
    switch (category) {
      case 'architecture': return this.localizationService.translate('CAT_ARCHITECTURE').toUpperCase();
      case 'ai': return this.localizationService.translate('CAT_AI').toUpperCase();
      case 'frontend': return this.localizationService.translate('CAT_FRONTEND').toUpperCase();
      default: return this.localizationService.translate('CAT_OTHER').toUpperCase();
    }
  }
}
