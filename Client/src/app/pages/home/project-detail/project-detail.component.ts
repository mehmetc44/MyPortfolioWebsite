import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DataService, Project } from '../../../shared/services/data.service';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit {
  project?: Project;
  sanitizedDetailText?: SafeHtml;
  currentSlide = 0;

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
        const found = this.dataService.getProject(id);
        if (found) {
          this.project = found;
          this.sanitizedDetailText = this.sanitizer.bypassSecurityTrustHtml(found.detailText);
          this.currentSlide = 0;
        } else {
          // If project not found, redirect to portfolio listing
          this.router.navigate(['/portfolio']);
        }
      }
    });
  }

  nextSlide() {
    if (this.project && this.project.images.length > 0) {
      this.currentSlide = (this.currentSlide + 1) % this.project.images.length;
    }
  }

  prevSlide() {
    if (this.project && this.project.images.length > 0) {
      this.currentSlide = (this.currentSlide - 1 + this.project.images.length) % this.project.images.length;
    }
  }

  setSlide(index: number) {
    this.currentSlide = index;
  }
}
