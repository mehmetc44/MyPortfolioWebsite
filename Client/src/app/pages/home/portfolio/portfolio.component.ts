import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DataService, Project } from '../../../shared/services/data.service';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslatePipe],
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  projects: Project[] = [];
  filteredProjects: Project[] = [];
  activeCategory = 'all';
  searchQuery = '';

  constructor(private dataService: DataService) {}

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  ngOnInit() {
    this.projects = this.dataService.getProjects();
    this.filterList();
  }

  selectCategory(cat: string) {
    this.activeCategory = cat;
    this.filterList();
  }

  onSearchChange() {
    this.filterList();
  }

  filterList() {
    this.filteredProjects = this.projects.filter(proj => {
      // 1. Category check
      const matchCat = this.activeCategory === 'all' || proj.category === this.activeCategory;

      // 2. Query check
      const q = this.searchQuery.toLowerCase().trim();
      const matchQuery = !q ||
        proj.title.toLowerCase().includes(q) ||
        proj.description.toLowerCase().includes(q) ||
        proj.tech.toLowerCase().includes(q) ||
        proj.client.toLowerCase().includes(q);

      return matchCat && matchQuery;
    });
  }

  getTechList(techString: string): string[] {
    if (!techString) return [];
    return techString.split(',').map(t => t.trim());
  }
}
