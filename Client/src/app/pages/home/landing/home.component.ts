import { Component, OnInit, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { DataService, Profile, Skill, TechTag } from '../../../shared/services/data.service';
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
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  get profile(): Profile {
    return this.dataService.getProfile();
  }
  projectCount = 0;
  articleCount = 0;
  skills: Skill[] = [];
  techTags: TechTag[] = [];

  dateList: DayCell[] = [];
  
  // Tooltip bindings
  tooltipText = '';
  tooltipVisible = false;
  tooltipLeft = '0px';
  tooltipTop = '0px';

  // Hire me trigger modal
  showContact = false;

  private subscription = new Subscription();

  constructor(private dataService: DataService, private elRef: ElementRef) {}

  async ngOnInit() {
    this.loadDashboardData();

    this.subscription.add(
      this.dataService.dataUpdated$.subscribe(() => {
        this.loadDashboardData();
      })
    );
  }

  async loadDashboardData() {
    this.projectCount = this.dataService.getProjects().length;
    this.articleCount = this.dataService.getArticles().length;
    this.skills = this.dataService.getSkills();
    this.techTags = this.dataService.getTechTagsList();

    await this.generateContributionData();
    
    setTimeout(() => {
      this.setupSkillsAnimation();
    }, 300);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  ngAfterViewInit() {
    this.setupSkillsAnimation();
  }

  openContactModal() {
    this.showContact = true;
  }

  async generateContributionData() {
    let contributionMap: Record<string, number> = {};
    const rawGithub = (this.profile && this.profile.github) ? this.profile.github : 'https://github.com/mehmetc44';
    const githubUrl = rawGithub.trim();
    const cleanUrl = githubUrl.replace(/\/$/, '');
    const parts = cleanUrl.split('/');
    let username = parts[parts.length - 1];
    if (!username || username === 'github.com' || username === '#') {
      username = 'mehmetc44';
    }

    try {
      // 1. Fetch real 365-day GitHub contribution calendar
      const res = await fetch(`https://github-contributions-api.deno.dev/${username}.json`);
      if (res.ok) {
        const data = await res.json();
        if (data && data.contributions && Array.isArray(data.contributions)) {
          for (const week of data.contributions) {
            if (Array.isArray(week)) {
              for (const day of week) {
                if (day && day.date) {
                  contributionMap[day.date] = day.contributionCount || 0;
                }
              }
            }
          }
        }
      }
    } catch (e) {
      ConsoleError("Failed to fetch 365-day contributions API, trying fallback", e);
    }

    // 2. Fallback to REST events API if contribution map is empty
    if (Object.keys(contributionMap).length === 0) {
      try {
        const res = await fetch(`https://api.github.com/users/${username}/events?per_page=100`);
        if (res.ok) {
          const events = await res.json();
          for (const ev of events) {
            if (!ev.created_at) continue;
            const dateStr = ev.created_at.substring(0, 10);
            let count = 1;
            if (ev.type === 'PushEvent' && ev.payload && ev.payload.commits) {
              count = ev.payload.commits.length || 1;
            }
            contributionMap[dateStr] = (contributionMap[dateStr] || 0) + count;
          }
        }
      } catch (err) {
        ConsoleError("Failed to fetch fallback GitHub events", err);
      }
    }

    const totalDays = 371; 
    const today = new Date();
    const dayOfWeek = today.getDay();
    const startDate = new Date(today);
    startDate.setDate(today.getDate() - totalDays + (6 - dayOfWeek));

    const tempDateList: DayCell[] = [];

    for (let i = 0; i < totalDays; i++) {
      const currentDate = new Date(startDate);
      currentDate.setDate(startDate.getDate() + i);

      const year = currentDate.getFullYear();
      const monthStr = String(currentDate.getMonth() + 1).padStart(2, '0');
      const dateStr = String(currentDate.getDate()).padStart(2, '0');
      const dateKey = `${year}-${monthStr}-${dateStr}`;

      const count = contributionMap[dateKey] || 0;
      let level = 0;
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

      const activeLang = typeof window !== 'undefined' ? (localStorage.getItem('app_language') || 'tr') : 'tr';
      const options: Intl.DateTimeFormatOptions = { weekday: 'long', year: 'numeric', month: 'short', day: 'numeric' };
      const formattedDate = currentDate.toLocaleDateString(activeLang === 'en' ? 'en-US' : (activeLang === 'de' ? 'de-DE' : 'tr-TR'), options);
      
      let countText = '';
      if (activeLang === 'en') {
        countText = count === 0 ? 'No active operations' : `${count} active operation${count > 1 ? 's' : ''}`;
      } else if (activeLang === 'de') {
        countText = count === 0 ? 'Keine aktiven Vorgänge' : `${count} aktive${count > 1 ? 's' : ''} Vorgänge`;
      } else {
        countText = count === 0 ? 'Aktif işlem yok' : `${count} aktif işlem`;
      }

      tempDateList.push({
        date: currentDate,
        level: level,
        count: count,
        tooltip: activeLang === 'en' ? `${countText} on ${formattedDate}` : (activeLang === 'de' ? `${countText} am ${formattedDate}` : `${formattedDate} tarihinde ${countText}`)
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

function ConsoleError(msg: string, err: any) {
  console.warn(msg, err);
}
