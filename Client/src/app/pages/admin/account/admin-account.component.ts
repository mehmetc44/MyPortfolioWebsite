import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-account',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-account.component.html',
  styleUrls: ['./admin-account.component.css']
})
export class AdminAccountComponent implements OnInit {
  username = '';
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';

  isSaving = false;
  successMessage = '';
  errorMessage = '';

  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    this.username = await this.dataService.getAccountUsername();
  }

  async onSave() {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.username.trim()) {
      this.errorMessage = 'Kullanıcı adı boş olamaz.';
      return;
    }

    if (this.newPassword) {
      if (!this.currentPassword) {
        this.errorMessage = 'Şifrenizi değiştirmek için mevcut şifrenizi girmeniz gerekmektedir.';
        return;
      }
      if (this.newPassword.length < 6) {
        this.errorMessage = 'Yeni şifre en az 6 karakter olmalıdır.';
        return;
      }
      if (this.newPassword !== this.confirmPassword) {
        this.errorMessage = 'Yeni şifreler uyuşmuyor.';
        return;
      }
    }

    this.isSaving = true;
    const res = await this.dataService.updateAccount(
      this.username.trim(),
      this.currentPassword || undefined,
      this.newPassword || undefined
    );
    this.isSaving = false;

    if (res.success) {
      this.successMessage = 'Hesap ayarları başarıyla güncellendi.';
      this.currentPassword = '';
      this.newPassword = '';
      this.confirmPassword = '';
    } else {
      this.errorMessage = res.message || 'Güncelleme sırasında bir hata oluştu.';
    }
  }
}
