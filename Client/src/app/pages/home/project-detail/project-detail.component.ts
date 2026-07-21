import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { marked } from 'marked';
import { DataService, Project, sanitizeImageUrl } from '../../../shared/services/data.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
import { LocalizationService } from '../../../shared/services/localization.service';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  project: Project | null = null;
  sanitizedDetailText: SafeHtml = '';
  currentSlide = 0;

  // Lightbox Fullscreen Modal State
  isLightboxOpen = false;
  lightboxImageIndex = 0;
  private isDragging = false;

  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private sanitizer: DomSanitizer,
    private localizationService: LocalizationService
  ) {}

  getProjectCategories(categoryStr?: string): string[] {
    if (!categoryStr) return [];
    return categoryStr.split(',').map(c => c.trim()).filter(c => c.length > 0);
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

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  ngOnInit() {
    this.subscription.add(
      this.route.paramMap.subscribe(params => {
        this.loadProject(params.get('id'));
      })
    );

    this.subscription.add(
      this.dataService.dataUpdated$.subscribe(() => {
        const id = this.route.snapshot.paramMap.get('id');
        this.loadProject(id);
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  loadProject(id: string | null) {
    if (id) {
      const found = this.dataService.getProject(id);
      if (found) {
        this.project = found;
        try {
          const detailText = this.resolveDetailImages(found.detailText || '');
          const parsed = marked.parse(detailText, { async: false }) as string;
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(parsed);
        } catch (e) {
          console.error("Markdown parsing failed", e);
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText || '');
        }
        this.currentSlide = 0;
      } else if (this.dataService.isLoaded) {
        // If data loading is complete AND project is not found, redirect to portfolio listing
        this.router.navigate(['/portfolio']);
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

  // Lightbox Modal Controls
  openLightbox(index?: number) {
    this.lightboxImageIndex = index !== undefined ? index : this.currentSlide;
    this.isLightboxOpen = true;
  }

  closeLightbox() {
    this.isLightboxOpen = false;
  }

  nextLightboxImage(event?: Event) {
    if (event) event.stopPropagation();
    if (this.project && this.project.images.length > 0) {
      this.lightboxImageIndex = (this.lightboxImageIndex + 1) % this.project.images.length;
    }
  }

  prevLightboxImage(event?: Event) {
    if (event) event.stopPropagation();
    if (this.project && this.project.images.length > 0) {
      this.lightboxImageIndex = (this.lightboxImageIndex - 1 + this.project.images.length) % this.project.images.length;
    }
  }



  @HostListener('window:keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent) {
    if (!this.isLightboxOpen) return;
    if (event.key === 'Escape') {
      this.closeLightbox();
    } else if (event.key === 'ArrowRight') {
      this.nextLightboxImage();
    } else if (event.key === 'ArrowLeft') {
      this.prevLightboxImage();
    }
  }

  onImageClick(idx: number, event: MouseEvent) {
    if (this.isDragging) {
      this.isDragging = false;
      return;
    }
    event.stopPropagation();
    this.openLightbox(idx);
  }

  private swipeStartX = 0;
  private swipeStartY = 0;

  nextSlide(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    if (this.project && this.project.images.length > 0) {
      this.currentSlide = (this.currentSlide + 1) % this.project.images.length;
    }
  }

  prevSlide(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    if (this.project && this.project.images.length > 0) {
      this.currentSlide = (this.currentSlide - 1 + this.project.images.length) % this.project.images.length;
    }
  }

  setSlide(index: number) {
    this.currentSlide = index;
  }

  onTouchStart(event: TouchEvent) {
    this.swipeStartX = event.changedTouches[0].screenX;
    this.swipeStartY = event.changedTouches[0].screenY;
    this.isDragging = false;
  }

  onTouchEnd(event: TouchEvent) {
    const endX = event.changedTouches[0].screenX;
    const endY = event.changedTouches[0].screenY;
    if (Math.abs(endX - this.swipeStartX) > 10) {
      this.isDragging = true;
    }
    this.handleSwipe(this.swipeStartX, this.swipeStartY, endX, endY);
  }

  onMouseDown(event: MouseEvent) {
    this.swipeStartX = event.screenX;
    this.swipeStartY = event.screenY;
    this.isDragging = false;
  }

  onMouseUp(event: MouseEvent) {
    const endX = event.screenX;
    const endY = event.screenY;
    if (Math.abs(endX - this.swipeStartX) > 10) {
      this.isDragging = true;
    }
    this.handleSwipe(this.swipeStartX, this.swipeStartY, endX, endY);
  }

  private handleSwipe(startX: number, startY: number, endX: number, endY: number) {
    const diffX = endX - startX;
    const diffY = endY - startY;
    
    // Check if horizontal swipe is dominant and significant
    if (Math.abs(diffX) > Math.abs(diffY) && Math.abs(diffX) > 50) {
      if (diffX > 0) {
        this.prevSlide();
      } else {
        this.nextSlide();
      }
    }
  }
}
