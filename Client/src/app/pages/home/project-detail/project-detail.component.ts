import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { marked } from 'marked';
import { DataService, Project } from '../../../shared/services/data.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  project?: Project;
  sanitizedDetailText?: SafeHtml;
  currentSlide = 0;

  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private sanitizer: DomSanitizer
  ) {}

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
          const parsed = marked.parse(found.detailText || '') as string;
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(parsed);
        } catch (e) {
          console.error("Markdown parsing failed", e);
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText || '');
        }
        this.currentSlide = 0;
      } else {
        // If project not found, redirect to portfolio listing
        this.router.navigate(['/portfolio']);
      }
    }
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
  }

  onTouchEnd(event: TouchEvent) {
    const endX = event.changedTouches[0].screenX;
    const endY = event.changedTouches[0].screenY;
    this.handleSwipe(this.swipeStartX, this.swipeStartY, endX, endY);
  }

  onMouseDown(event: MouseEvent) {
    this.swipeStartX = event.screenX;
    this.swipeStartY = event.screenY;
  }

  onMouseUp(event: MouseEvent) {
    const endX = event.screenX;
    const endY = event.screenY;
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
