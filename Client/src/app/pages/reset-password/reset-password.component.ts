import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../shared/services/data.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  newPassword = '';
  confirmPassword = '';
  
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParams['token'] || '';
    if (!this.token) {
      this.errorMessage = 'Şifre sıfırlama anahtarı (token) bulunamadı. Lütfen e-postanızdaki bağlantıyı kontrol edin.';
    }
  }

  async onSubmit() {
    if (!this.token) {
      this.errorMessage = 'Geçersiz veya eksik sıfırlama anahtarı.';
      return;
    }

    if (!this.newPassword || !this.confirmPassword) {
      this.errorMessage = 'Lütfen tüm alanları doldurun.';
      return;
    }

    if (this.newPassword.length < 6) {
      this.errorMessage = 'Şifre en az 6 karakter uzunluğunda olmalıdır.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Şifreler uyuşmuyor. Lütfen kontrol edin.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const success = await this.dataService.resetPassword(this.token, this.newPassword);
    this.isSubmitting = false;

    if (success) {
      this.successMessage = 'Şifreniz başarıyla güncellendi! Giriş sayfasına yönlendiriliyorsunuz...';
      setTimeout(() => {
        this.router.navigate(['/admin']);
      }, 3000);
    } else {
      this.errorMessage = 'Şifre sıfırlama başarısız oldu. Anahtarın süresi dolmuş veya geçersiz olabilir.';
    }
  }
}
