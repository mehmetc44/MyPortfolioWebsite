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

  constructor() {
    this.loadDataFromServer();
  }

  public async loadDataFromServer(): Promise<void> {
    if (typeof window === 'undefined') return;
    const lang = localStorage.getItem('app_language') || 'tr';
    try {
      const profRes = await fetch(`http://localhost:5169/api/profile?lang=${lang}`);
      if (profRes.ok) {
        this.cachedProfile = await profRes.json();
      }

      const projRes = await fetch(`http://localhost:5169/api/projects?lang=${lang}`);
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

      const artRes = await fetch(`http://localhost:5169/api/articles?lang=${lang}`);
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
      const res = await fetch('http://localhost:5169/api/projects/raw');
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
        ? 'http://localhost:5169/api/projects' 
        : `http://localhost:5169/api/projects/${project.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(project)
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async deleteRawProject(id: string): Promise<boolean> {
    try {
      const res = await fetch(`http://localhost:5169/api/projects/${id}`, {
        method: 'DELETE'
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async getRawProfile(): Promise<RawProfile | null> {
    try {
      const res = await fetch('http://localhost:5169/api/profile/raw');
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
      const res = await fetch('http://localhost:5169/api/profile', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(profile)
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async getRawArticles(): Promise<RawArticle[]> {
    try {
      const res = await fetch('http://localhost:5169/api/articles/raw');
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
        ? 'http://localhost:5169/api/articles' 
        : `http://localhost:5169/api/articles/${article.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(article)
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async deleteRawArticle(id: string): Promise<boolean> {
    try {
      const res = await fetch(`http://localhost:5169/api/articles/${id}`, {
        method: 'DELETE'
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
      
      let url = 'http://localhost:5169/api/upload/avatar';
      if (oldUrl) {
        url += `?oldUrl=${encodeURIComponent(oldUrl)}`;
      }
      
      const res = await fetch(url, {
        method: 'POST',
        body: formData
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
      
      let url = 'http://localhost:5169/api/upload/cv';
      if (oldUrl) {
        url += `?oldUrl=${encodeURIComponent(oldUrl)}`;
      }
      
      const res = await fetch(url, {
        method: 'POST',
        body: formData
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
      const res = await fetch('http://localhost:5169/api/contact', {
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
      const res = await fetch('http://localhost:5169/api/messages');
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
      const res = await fetch(`http://localhost:5169/api/messages/${id}/read?isRead=${isRead}`, {
        method: 'PUT'
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to update message status", e);
      return false;
    }
  }

  async deleteMessage(id: number): Promise<boolean> {
    try {
      const res = await fetch(`http://localhost:5169/api/messages/${id}`, {
        method: 'DELETE'
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to delete message", e);
      return false;
    }
  }

  private unreadCount = 0;

  getUnreadCount(): number {
    return this.unreadCount;
  }

  setUnreadCount(count: number): void {
    this.unreadCount = count;
  }
}
