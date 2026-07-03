import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DataService, Article } from '../../../shared/services/data.service';

@Component({
  selector: 'app-blog-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
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
    private sanitizer: DomSanitizer
  ) {}

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
      case 'architecture': return 'YAZILIM MİMARİSİ';
      case 'ai': return 'YAPAY ZEKA';
      case 'frontend': return 'ÖN UÇ & TASARIM';
      default: return 'TEKNİK MAKALE';
    }
  }
}
