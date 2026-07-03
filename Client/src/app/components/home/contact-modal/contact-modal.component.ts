import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

import { LocalizationService } from '../../../shared/services/localization.service';
import { DataService } from '../../../shared/services/data.service';

@Component({
  selector: 'app-contact-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './contact-modal.component.html',
  styleUrls: ['./contact-modal.component.css']
})
export class ContactModalComponent {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();

  name = '';
  email = '';
  message = '';

  constructor(private localizationService: LocalizationService, private dataService: DataService) {}

  get cvPath(): string {
    const lang = this.localizationService.getLanguage();
    return `assets/cv_${lang}.pdf`;
  }

  isSubmitting = false;
  isSuccess = false;

  closeModal() {
    this.close.emit();
    // Reset form states
    setTimeout(() => {
      this.name = '';
      this.email = '';
      this.message = '';
      this.isSubmitting = false;
      this.isSuccess = false;
    }, 300);
  }

  async onSubmit() {
    if (!this.name.trim() || !this.email.trim() || !this.message.trim()) return;
    this.isSubmitting = true;
    
    const ok = await this.dataService.postContactMessage({
      name: this.name,
      email: this.email,
      message: this.message
    });
    
    this.isSubmitting = false;
    if (ok) {
      this.isSuccess = true;
      // Auto close modal after 2.5 seconds
      setTimeout(() => {
        this.closeModal();
      }, 2500);
    } else {
      alert('Mesajınız gönderilirken hata oluştu. Lütfen tekrar deneyin.');
    }
  }
}
