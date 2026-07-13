import { Component, OnInit, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DataService, Profile } from '../../../shared/services/data.service';
import { ContactModalComponent } from '../../../components/home/contact-modal/contact-modal.component';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';

interface DayCell {
  date: Date;
  level: number;
  count: number;
  tooltip: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, ContactModalComponent, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, AfterViewInit {
  get profile(): Profile {
    return this.dataService.getProfile();
  }
  projectCount = 0;
  articleCount = 0;

  dateList: DayCell[] = [];
  
  // Tooltip bindings
  tooltipText = '';
  tooltipVisible = false;
  tooltipLeft = '0px';
  tooltipTop = '0px';

  // Hire me trigger modal
  showContact = false;

  constructor(private dataService: DataService, private elRef: ElementRef) {}

  async ngOnInit() {
    this.projectCount = this.dataService.getProjects().length;
    this.articleCount = this.dataService.getArticles().length;
    await this.generateContributionData();
  }

  ngAfterViewInit() {
    this.setupSkillsAnimation();
  }

  openContactModal() {
    this.showContact = true;
  }

  async generateContributionData() {
    let contributionMap: Record<string, number> = {};
    if (this.profile && this.profile.github) {
      const githubUrl = this.profile.github.trim();
      const cleanUrl = githubUrl.replace(/\/$/, '');
      const parts = cleanUrl.split('/');
      const username = parts[parts.length - 1];
      if (username && username !== 'github.com' && username !== '#') {
        try {
          const res = await fetch(`https://api.github.com/users/${username}/events`);
          if (res.ok) {
            const events = await res.json();
            for (const ev of events) {
              if (!ev.created_at) continue;
              const dateStr = ev.created_at.substring(0, 10); // "YYYY-MM-DD"
              let count = 1;
              if (ev.type === 'PushEvent' && ev.payload && ev.payload.commits) {
                count = ev.payload.commits.length || 1;
              }
              contributionMap[dateStr] = (contributionMap[dateStr] || 0) + count;
            }
          }
        } catch (e) {
          console.warn("Failed to fetch GitHub events, using fallback data", e);
        }
      }
    }

    const totalDays = 371; 
    const today = new Date();
    const dayOfWeek = today.getDay();
    const startDate = new Date(today);
    startDate.setDate(today.getDate() - totalDays + (6 - dayOfWeek));

    const ninetyDaysAgo = new Date();
    ninetyDaysAgo.setDate(today.getDate() - 90);

    const tempDateList: DayCell[] = [];

    for (let i = 0; i < totalDays; i++) {
      const currentDate = new Date(startDate);
      currentDate.setDate(startDate.getDate() + i);
      
      const dayNum = currentDate.getDay();
      const monthNum = currentDate.getMonth();
      const dateNum = currentDate.getDate();
      
      let level = 0;
      let count = 0;

      const year = currentDate.getFullYear();
      const monthStr = String(currentDate.getMonth() + 1).padStart(2, '0');
      const dateStr = String(currentDate.getDate()).padStart(2, '0');
      const dateKey = `${year}-${monthStr}-${dateStr}`;

      if (currentDate >= ninetyDaysAgo && currentDate <= today) {
        count = contributionMap[dateKey] || 0;
        if (count > 0) {
          if (count <= 2) {
            level = 1;
          } else if (count <= 4) {
            level = 2;
          } else if (count <= 8) {
            level = 3;
          } else {
            level = 4;
          }
        }
      } else {
        let baseChance = (dayNum === 0 || dayNum === 6) ? 0.25 : 0.7;
        if ((monthNum === 2 || monthNum === 5 || monthNum === 9 || monthNum === 11) && dateNum % 3 !== 0) {
          baseChance += 0.2;
        }
        
        if (Math.random() < baseChance) {
          const rand = Math.random();
          if (rand < 0.5) {
            level = 1;
            count = Math.floor(Math.random() * 3) + 1;
          } else if (rand < 0.8) {
            level = 2;
            count = Math.floor(Math.random() * 4) + 4;
          } else if (rand < 0.95) {
            level = 3;
            count = Math.floor(Math.random() * 5) + 8;
          } else {
            level = 4;
            count = Math.floor(Math.random() * 8) + 13;
          }
        }
      }

      const options: Intl.DateTimeFormatOptions = { weekday: 'long', year: 'numeric', month: 'short', day: 'numeric' };
      const formattedDate = currentDate.toLocaleDateString('en-US', options);
      const countText = count === 0 ? 'No active operations' : `${count} active operation${count > 1 ? 's' : ''}`;

      tempDateList.push({
        date: currentDate,
        level: level,
        count: count,
        tooltip: `${countText} on ${formattedDate}`
      });
    }

    this.dateList = tempDateList;
  }

  showTooltip(event: MouseEvent, item: DayCell) {
    this.tooltipText = item.tooltip;
    this.tooltipVisible = true;
    
    // Position tooltip above the cell
    const cell = event.currentTarget as HTMLElement;
    const rect = cell.getBoundingClientRect();
    
    // Using viewport coordinates relative to scroll position
    const scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    
    this.tooltipLeft = `${rect.left + scrollLeft + (rect.width / 2)}px`;
    this.tooltipTop = `${rect.top + scrollTop - 10}px`;
  }

  hideTooltip() {
    this.tooltipVisible = false;
  }

  setupSkillsAnimation() {
    const cardEl = this.elRef.nativeElement.querySelector('#skills-card');
    const skillBars = this.elRef.nativeElement.querySelectorAll('.skill-bar-progress');

    if (cardEl && skillBars.length > 0) {
      const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            skillBars.forEach((bar: any) => {
              const targetWidth = bar.getAttribute('data-target-width');
              if (targetWidth) {
                bar.style.width = targetWidth;
              }
            });
            observer.unobserve(entry.target);
          }
        });
      }, { threshold: 0.15 });

      observer.observe(cardEl);
    }
  }
}
