import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DataService, Article } from '../../../shared/services/data.service';
import { LocalizationService } from '../../../shared/services/localization.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-blog-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './blog-detail.component.html',
  styleUrls: ['./blog-detail.component.css']
})
export class BlogDetailComponent implements OnInit {
  article?: Article;
  sanitizedDetailText?: SafeHtml;

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
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        const found = this.dataService.getArticle(id);
        if (found) {
          this.article = found;
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText);
        } else {
          this.router.navigate(['/blog']);
        }
      }
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
