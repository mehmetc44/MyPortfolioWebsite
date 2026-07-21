import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DataService, Profile } from '../../shared/services/data.service';
import { AdminOverviewComponent } from './overview/admin-overview.component';
import { AdminProfileComponent } from './profile/admin-profile.component';
import { AdminProjectsComponent } from './projects/admin-projects.component';
import { AdminBlogComponent } from './blog/admin-blog.component';
import { AdminMessagesComponent } from './messages/admin-messages.component';
import { AdminCvComponent } from './cv/admin-cv.component';
import { AdminSkillsComponent } from './skills/admin-skills.component';
import { AdminAccountComponent } from './account/admin-account.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdminOverviewComponent,
    AdminProfileComponent,
    AdminProjectsComponent,
    AdminBlogComponent,
    AdminMessagesComponent,
    AdminCvComponent,
    AdminSkillsComponent,
    AdminAccountComponent
  ],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeTab = 'panelOverview';
  loginData = { username: '', password: '' };
  isLoggingIn = false;
  errorMessage = '';

  isForgotPasswordMode = false;
  forgotPasswordEmail = '';
  forgotPasswordSuccessMessage = '';
  forgotPasswordErrorMessage = '';
  isSendingResetLink = false;

  toggleForgotPasswordMode() {
    this.isForgotPasswordMode = !this.isForgotPasswordMode;
    this.forgotPasswordSuccessMessage = '';
    this.forgotPasswordErrorMessage = '';
    this.errorMessage = '';
  }

  async onForgotPasswordClick() {
    this.isForgotPasswordMode = true;
    this.forgotPasswordSuccessMessage = '';
    this.forgotPasswordErrorMessage = '';
    this.isSendingResetLink = true;

    // Direct request without requiring email input from user
    const success = await this.dataService.forgotPassword('admin@example.com');
    this.isSendingResetLink = false;

    if (success) {
      this.forgotPasswordSuccessMessage = 'Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.';
    } else {
      this.forgotPasswordErrorMessage = 'Şifre sıfırlama bağlantısı gönderilemedi. Lütfen tekrar deneyin.';
    }
  }

  get profile(): Profile {
    return this.dataService.getProfile();
  }

  get isAuthenticated(): boolean {
    return this.dataService.isAuthenticated;
  }

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    await this.dataService.checkAuthStatus();
    if (this.isAuthenticated) {
      await this.loadAdminData();
    }
  }

  async loadAdminData() {
    try {
      const msgs = await this.dataService.getAdminMessages();
      const unread = msgs.filter(m => !m.isRead).length;
      this.dataService.setUnreadCount(unread);
    } catch(e) {
      console.error(e);
    }
  }

  async onLogin() {
    if (!this.loginData.username || !this.loginData.password) {
      this.errorMessage = 'Lütfen kullanıcı adı ve şifreyi doldurun.';
      return;
    }

    this.isLoggingIn = true;
    this.errorMessage = '';
    const success = await this.dataService.login(this.loginData.username, this.loginData.password);
    this.isLoggingIn = false;

    if (success) {
      this.loginData = { username: '', password: '' };
      await this.loadAdminData();
    } else {
      this.errorMessage = 'Geçersiz kullanıcı adı veya şifre.';
    }
  }

  async onLogout() {
    await this.dataService.logout();
    this.activeTab = 'panelOverview';
  }

  get unreadMessagesCount(): number {
    return this.dataService.getUnreadCount();
  }

  switchTab(tab: string) {
    this.activeTab = tab;
  }
}
