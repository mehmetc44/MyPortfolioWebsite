import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { DataService } from '../../services/data.service';
import { ContactModalComponent } from '../../../components/home/contact-modal/contact-modal.component';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { LocalizationService, LanguageCode } from '../../services/localization.service';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-home-layout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet, RouterLink, RouterLinkActive, ContactModalComponent, TranslatePipe],
  templateUrl: './home-layout.component.html',
  styleUrls: ['./home-layout.component.css']
})
export class HomeLayoutComponent implements OnInit {
  get logoName(): string {
    return this.dataService.getProfile().name;
  }
  showContact = false;
  activeLang: LanguageCode = 'tr';
  isMobileMenuOpen = false;

  constructor(
    private dataService: DataService,
    private localizationService: LocalizationService,
    private themeService: ThemeService
  ) {}

  ngOnInit() {
    this.activeLang = this.localizationService.getLanguage();
  }

  openContact(event: Event) {
    event.preventDefault();
    this.showContact = true;
    this.isMobileMenuOpen = false;
  }

  onContactClose() {
    this.showContact = false;
  }

  changeLanguage(lang: LanguageCode) {
    this.localizationService.setLanguage(lang);
    this.activeLang = lang;
    this.dataService.loadDataFromServer();
  }

  get isDarkMode(): boolean {
    return this.themeService.isDarkMode;
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
