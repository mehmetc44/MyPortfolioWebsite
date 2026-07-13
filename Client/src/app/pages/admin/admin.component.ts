import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DataService, Profile } from '../../shared/services/data.service';
import { AdminOverviewComponent } from './overview/admin-overview.component';
import { AdminProfileComponent } from './profile/admin-profile.component';
import { AdminProjectsComponent } from './projects/admin-projects.component';
import { AdminBlogComponent } from './blog/admin-blog.component';
import { AdminMessagesComponent } from './messages/admin-messages.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    AdminOverviewComponent,
    AdminProfileComponent,
    AdminProjectsComponent,
    AdminBlogComponent,
    AdminMessagesComponent
  ],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeTab = 'panelOverview';
  get profile(): Profile {
    return this.dataService.getProfile();
  }

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    // Fetch and populate initial unread count in DataService
    try {
      const msgs = await this.dataService.getAdminMessages();
      const unread = msgs.filter(m => !m.isRead).length;
      this.dataService.setUnreadCount(unread);
    } catch(e) {
      console.error(e);
    }
  }

  get unreadMessagesCount(): number {
    return this.dataService.getUnreadCount();
  }

  switchTab(tab: string) {
    this.activeTab = tab;
  }
}
