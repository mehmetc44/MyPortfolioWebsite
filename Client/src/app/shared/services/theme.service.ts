import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly themeKey = 'app_theme_mode';
  private isDarkSubject = new BehaviorSubject<boolean>(false);
  public isDark$: Observable<boolean> = this.isDarkSubject.asObservable();

  constructor() {
    this.initTheme();
  }

  private initTheme(): void {
    if (typeof window === 'undefined') return;
    
    let savedTheme: ThemeMode = 'light';
    const cached = localStorage.getItem(this.themeKey) as ThemeMode;
    
    if (cached === 'light' || cached === 'dark') {
      savedTheme = cached;
    } else {
      // Check system preferences
      const darkPref = window.matchMedia('(prefers-color-scheme: dark)').matches;
      savedTheme = darkPref ? 'dark' : 'light';
    }

    this.isDarkSubject.next(savedTheme === 'dark');
    this.applyThemeClass(savedTheme === 'dark');
  }

  public toggleTheme(): void {
    const nextDark = !this.isDarkSubject.value;
    this.isDarkSubject.next(nextDark);
    
    if (typeof window !== 'undefined') {
      localStorage.setItem(this.themeKey, nextDark ? 'dark' : 'light');
    }
    this.applyThemeClass(nextDark);
  }

  public get isDarkMode(): boolean {
    return this.isDarkSubject.value;
  }

  private applyThemeClass(isDark: boolean): void {
    if (typeof document === 'undefined') return;
    
    const body = document.body;
    if (isDark) {
      body.classList.add('dark-theme');
    } else {
      body.classList.remove('dark-theme');
    }
  }
}
