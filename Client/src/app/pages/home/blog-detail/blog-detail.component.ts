import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
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
  styleUrls: ['./blog-detail.component.css'],
  encapsulation: ViewEncapsulation.None
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
          let parsedHtml = marked.parse(detailText, { async: false }) as string;

          parsedHtml = parsedHtml.replace(/(?:<p>)?\s*(<img\s+[^>]*?>)\s*(?:<\/p>)?/gi, (fullMatch, imgTag) => {
            if (fullMatch.includes('blog-image-wrapper') || imgTag.includes('blog-image-wrapper')) {
              return fullMatch;
            }
            const srcMatch = imgTag.match(/src=["']([^"']+)["']/i);
            const altMatch = imgTag.match(/alt=["']([^"']+)["']/i);
            const src = srcMatch ? srcMatch[1] : '';
            const alt = altMatch ? altMatch[1] : '';
            
            if (!src) return fullMatch;

            const altAttr = alt ? alt.replace(/"/g, '&quot;') : 'Görsel';
            const captionText = alt && alt.trim() && alt !== 'Görsel' ? alt.trim() : 'Görsel Açıklaması';

            return `<figure class="blog-inline-figure">
  <div class="article-banner-wrapper blog-image-wrapper">
    <img src="${src}" alt="${altAttr}" class="article-banner-image" />
  </div>
  <figcaption>${captionText}</figcaption>
</figure>`;
          });

          parsedHtml = parsedHtml.replace(/<p>\s*(<figure[\s\S]*?<\/figure>)\s*<\/p>/gi, '$1');
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
      case 'architecture': return this.localizationService.translateUpper('CAT_ARCHITECTURE');
      case 'ai': return this.localizationService.translateUpper('CAT_AI');
      case 'backend': return this.localizationService.translateUpper('CAT_BACKEND');
      case 'devops-cloud': return this.localizationService.translateUpper('CAT_DEVOPS_CLOUD');
      case 'performance': return this.localizationService.translateUpper('CAT_PERFORMANCE');
      case 'web-dev': return this.localizationService.translateUpper('CAT_WEB_DEV_BLOG');
      case 'software-eng': return this.localizationService.translateUpper('CAT_SOFTWARE_ENG');
      default: return this.localizationService.translateUpper('CAT_OTHER');
    }
  }
}
