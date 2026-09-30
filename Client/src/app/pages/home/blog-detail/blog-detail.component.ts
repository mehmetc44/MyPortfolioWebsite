import { Component, OnInit, OnDestroy, ViewEncapsulation, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { DataService, Article, sanitizeImageUrl } from '../../../shared/services/data.service';
import { LocalizationService } from '../../../shared/services/localization.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
import { parseMarkdownToHtml } from '../../../shared/helpers/markdown-parser.helper';
import mermaid from 'mermaid';
import * as Prism from 'prismjs';

// Import Prism languages that might be used
import 'prismjs/components/prism-csharp';
import 'prismjs/components/prism-typescript';
import 'prismjs/components/prism-javascript';
import 'prismjs/components/prism-json';
import 'prismjs/components/prism-bash';
import 'prismjs/components/prism-css';
import 'prismjs/components/prism-sql';
import 'prismjs/components/prism-python';
import 'prismjs/components/prism-markup';

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
  zoomedImageUrl: string | null = null;

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
          let parsedHtml = parseMarkdownToHtml(detailText);

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
  <div class="blog-image-wrapper">
    <img src="${src}" alt="${altAttr}" style="cursor: zoom-in;" />
  </div>
  <figcaption>${captionText}</figcaption>
</figure>`;
          });

          parsedHtml = parsedHtml.replace(/<p>\s*(<figure[\s\S]*?<\/figure>)\s*<\/p>/gi, '$1');
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(parsedHtml);
          
          this.initializePlugins();
        } catch (_) {
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText || '');
        }
      } else {
        this.router.navigate(['/blog']);
      }
    }
  }

  initializePlugins() {
    setTimeout(() => {
      // 1. Prism Syntax Highlighting
      Prism.highlightAll();

      // 2. Mermaid initialization & rendering
      try {
        mermaid.initialize({
          startOnLoad: false,
          theme: 'neutral',
          securityLevel: 'loose'
        });
        mermaid.run({
          nodes: document.querySelectorAll('.mermaid')
        });
      } catch (err) {
        console.warn("Mermaid initialization failed:", err);
      }
    }, 100);
  }

  closeZoom() {
    this.zoomedImageUrl = null;
  }

  @HostListener('click', ['$event'])
  onContentClick(event: Event) {
    const target = event.target as HTMLElement;

    // 1. Image Zoom click handler
    if (target.tagName === 'IMG' && target.closest('.detail-text-body')) {
      const img = target as HTMLImageElement;
      this.zoomedImageUrl = img.src;
      event.preventDefault();
      return;
    }

    // 2. Custom Tabs header switch click handler
    if (target.classList.contains('tab-button')) {
      const tabGroup = target.closest('.custom-tabs-container');
      if (tabGroup) {
        const buttons = tabGroup.querySelectorAll('.tab-button');
        const panels = tabGroup.querySelectorAll('.tab-panel');
        const index = Array.from(buttons).indexOf(target);
        
        buttons.forEach((btn, idx) => {
          if (idx === index) {
            btn.classList.add('active');
          } else {
            btn.classList.remove('active');
          }
        });
        
        panels.forEach((panel, idx) => {
          if (idx === index) {
            panel.classList.add('active');
          } else {
            panel.classList.remove('active');
          }
        });
      }
    }

    // 3. Copy Code Button click handler
    const copyBtn = target.closest('.blog-code-copy-btn');
    if (copyBtn) {
      const codeWrapper = copyBtn.closest('.blog-code-wrapper');
      const codeEl = codeWrapper?.querySelector('pre code');
      if (codeEl) {
        const textToCopy = codeEl.textContent || '';
        navigator.clipboard.writeText(textToCopy).then(() => {
          const textSpan = copyBtn.querySelector('span');
          if (textSpan) {
            textSpan.textContent = 'Copied!';
            copyBtn.classList.add('copied');
            setTimeout(() => {
              textSpan.textContent = 'Copy';
              copyBtn.classList.remove('copied');
            }, 2000);
          }
        }).catch(err => {
          console.error('Failed to copy code block content: ', err);
        });
      }
      event.preventDefault();
      return;
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
    if (!category || !category.trim()) return 'Genel';
    return category;
  }
}
