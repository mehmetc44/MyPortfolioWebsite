import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-overview.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminOverviewComponent implements OnInit {
  projectCount = 0;
  articleCount = 0;
  lastUpdated = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.projectCount = this.dataService.getProjects().length;
    this.articleCount = this.dataService.getArticles().length;
    this.lastUpdated = this.dataService.getLastUpdated();
  }
}
