import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService, Profile } from '../../../shared/services/data.service';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  get profile(): Profile {
    return this.dataService.getProfile();
  }

  constructor(private dataService: DataService) {}

  ngOnInit() {
  }
}
