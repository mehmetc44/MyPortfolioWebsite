import { Injectable } from '@angular/core';
import { Profile, RawProfile, ContactMessage } from '../models/profile.model';
import { Project, RawProject } from '../models/project.model';
import { Article, RawArticle } from '../models/article.model';

export type { Profile, RawProfile, ContactMessage } from '../models/profile.model';
export type { Project, RawProject } from '../models/project.model';
export type { Article, RawArticle } from '../models/article.model';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private readonly updateTimeKey = 'admin_last_updated';

  private cachedProfile: Profile | null = null;
  private cachedProjects: Project[] | null = null;
  private cachedArticles: Article[] | null = null;

  private _isAuthenticated = false;

  public get isAuthenticated(): boolean {
    return this._isAuthenticated;
  }

  public get apiBaseUrl(): string {
    if (typeof window !== 'undefined') {
      const hostname = window.location.hostname;
      if (hostname === 'localhost' || hostname === '127.0.0.1') {
        return 'http://localhost:5169';
      }
    }
    // Deployed production API URL on Render
    return 'https://myportfoliowebsite-xx7w.onrender.com';
  }

  constructor() {
    this.loadDataFromServer();
  }

  public async loadDataFromServer(): Promise<void> {
    if (typeof window === 'undefined') return;
    const lang = localStorage.getItem('app_language') || 'tr';
    try {
      const profRes = await fetch(`${this.apiBaseUrl}/api/profile?lang=${lang}`);
      if (profRes.ok) {
        this.cachedProfile = await profRes.json();
      }

      const projRes = await fetch(`${this.apiBaseUrl}/api/projects?lang=${lang}`);
      if (projRes.ok) {
        const rawProjects = await projRes.json();
        this.cachedProjects = rawProjects.map((p: any) => {
          let parsedImages: string[] = [];
          try {
            parsedImages = p.imagesJson ? JSON.parse(p.imagesJson) : [];
          } catch(e) {
            console.error("Failed to parse imagesJson", e);
          }
          return {
            ...p,
            images: parsedImages
          };
        });
      }

      const artRes = await fetch(`${this.apiBaseUrl}/api/articles?lang=${lang}`);
      if (artRes.ok) {
        this.cachedArticles = await artRes.json();
      }
    } catch (e) {
      console.warn("API Server not available.", e);
    }
  }

  getLastUpdated(): string {
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem(this.updateTimeKey) || new Date().toLocaleString('tr-TR');
    }
    return new Date().toLocaleString('tr-TR');
  }

  formatDate(dateStr?: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return dateStr; // fallback if parsing fails
    const activeLang = typeof window !== 'undefined' ? (localStorage.getItem('app_language') || 'tr') : 'tr';
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'long' };
    return date.toLocaleDateString(activeLang === 'en' ? 'en-US' : (activeLang === 'de' ? 'de-DE' : 'tr-TR'), options);
  }

  getProfile(): Profile {
    if (this.cachedProfile) {
      return this.cachedProfile;
    }
    return {
      name: "Mehmet",
      tag: "Software Architect",
      title: "Senior Full Stack Engineer & Tech Lead",
      bio: "Loading...",
      avatarUrl: "assets/avatar_senior.png",
      repos: 0,
      pubs: 0,
      github: "https://github.com/mehmetc44",
      linkedin: "https://linkedin.com",
      instagram: "https://instagram.com",
      medium: "https://medium.com"
    };
  }

  getProjects(): Project[] {
    return this.cachedProjects || [];
  }

  getProject(id: string): Project | undefined {
    const list = this.getProjects();
    return list.find(p => p.id === id);
  }

  getArticles(): Article[] {
    return this.cachedArticles || [];
  }

  getArticle(id: string): Article | undefined {
    const list = this.getArticles();
    return list.find(a => a.id === id);
  }

  logUpdate() {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.setItem(this.updateTimeKey, new Date().toLocaleString('tr-TR'));
    }
  }

  // Raw Database CRUD APIs (for Admin Panel)
  async getRawProjects(): Promise<RawProject[]> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/projects/raw`, {
        credentials: 'include'
      });
      if (res.ok) {
        return await res.json();
      }
    } catch(e) {
      console.error(e);
    }
    return [];
  }

  async saveRawProject(project: RawProject, isNew: boolean): Promise<boolean> {
    try {
      const url = isNew 
        ? `${this.apiBaseUrl}/api/projects` 
        : `${this.apiBaseUrl}/api/projects/${project.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(project),
        credentials: 'include'
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async deleteRawProject(id: string): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/projects/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async getRawProfile(): Promise<RawProfile | null> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/profile/raw`, {
        credentials: 'include'
      });
      if (res.ok) {
        return await res.json();
      }
    } catch(e) {
      console.error(e);
    }
    return null;
  }

  async saveRawProfile(profile: RawProfile): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/profile`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(profile),
        credentials: 'include'
      });
      if (res.ok) {
        await this.loadDataFromServer();
      }
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async getRawArticles(): Promise<RawArticle[]> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/articles/raw`, {
        credentials: 'include'
      });
      if (res.ok) {
        return await res.json();
      }
    } catch(e) {
      console.error(e);
    }
    return [];
  }

  async saveRawArticle(article: RawArticle, isNew: boolean): Promise<boolean> {
    try {
      const url = isNew 
        ? `${this.apiBaseUrl}/api/articles` 
        : `${this.apiBaseUrl}/api/articles/${article.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(article),
        credentials: 'include'
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async deleteRawArticle(id: string): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/articles/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  // File Upload Helper APIs (for Admin Panel)
  async uploadAvatar(file: File, oldUrl?: string): Promise<string | null> {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      let url = `${this.apiBaseUrl}/api/upload/avatar`;
      if (oldUrl) {
        url += `?oldUrl=${encodeURIComponent(oldUrl)}`;
      }
      
      const res = await fetch(url, {
        method: 'POST',
        body: formData,
        credentials: 'include'
      });
      
      if (res.ok) {
        const data = await res.json();
        return data.url; // e.g. "uploads/avatars/guid_filename.png"
      }
    } catch(e) {
      console.error(e);
    }
    return null;
  }

  async uploadCv(file: File, oldUrl?: string): Promise<string | null> {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      let url = `${this.apiBaseUrl}/api/upload/cv`;
      if (oldUrl) {
        url += `?oldUrl=${encodeURIComponent(oldUrl)}`;
      }
      
      const res = await fetch(url, {
        method: 'POST',
        body: formData,
        credentials: 'include'
      });
      
      if (res.ok) {
        const data = await res.json();
        return data.url; // e.g. "uploads/cvs/guid_filename.pdf"
      }
    } catch(e) {
      console.error(e);
    }
    return null;
  }

  async postContactMessage(msg: { name: string; email: string; message: string }): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/contact`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(msg)
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to post contact message", e);
      return false;
    }
  }

  async getAdminMessages(): Promise<ContactMessage[]> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/messages`, {
        credentials: 'include'
      });
      if (res.ok) {
        return await res.json();
      }
    } catch (e) {
      console.error("Failed to fetch messages", e);
    }
    return [];
  }

  async markMessageAsRead(id: number, isRead: boolean): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/messages/${id}/read?isRead=${isRead}`, {
        method: 'PUT',
        credentials: 'include'
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to update message status", e);
      return false;
    }
  }

  async deleteMessage(id: number): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/messages/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to delete message", e);
      return false;
    }
  }

  // Authentication APIs
  async checkAuthStatus(): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/auth/status`, {
        credentials: 'include'
      });
      if (res.ok) {
        const data = await res.json();
        this._isAuthenticated = data.isAuthenticated;
        return this._isAuthenticated;
      }
    } catch(e) {
      console.error("Failed to check auth status", e);
    }
    this._isAuthenticated = false;
    return false;
  }

  async login(username: string, password: string): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
        credentials: 'include'
      });
      if (res.ok) {
        const data = await res.json();
        this._isAuthenticated = data.success;
        return this._isAuthenticated;
      }
    } catch(e) {
      console.error("Login failed", e);
    }
    this._isAuthenticated = false;
    return false;
  }

  async logout(): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/auth/logout`, {
        method: 'POST',
        credentials: 'include'
      });
      if (res.ok) {
        this._isAuthenticated = false;
        return true;
      }
    } catch(e) {
      console.error("Logout failed", e);
    }
    return false;
  }

  private unreadCount = 0;

  getUnreadCount(): number {
    return this.unreadCount;
  }

  setUnreadCount(count: number): void {
    this.unreadCount = count;
  }
}
