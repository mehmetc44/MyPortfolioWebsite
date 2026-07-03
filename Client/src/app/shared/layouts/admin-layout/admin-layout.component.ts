import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { DataService } from '../../services/data.service';
import { ContactModalComponent } from '../../../components/home/contact-modal/contact-modal.component';
import { TranslatePipe } from '../../pipes/translate.pipe';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ContactModalComponent, TranslatePipe],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.css']
})
export class AdminLayoutComponent implements OnInit {
  logoName = 'Mehmet';
  showContact = false;

  constructor(private dataService: DataService) {}

  ngOnInit() {
    const profile = this.dataService.getProfile();
    this.logoName = profile.name;
  }

  openContact(event: Event) {
    event.preventDefault();
    this.showContact = true;
  }

  onContactClose() {
    this.showContact = false;
  }
}
