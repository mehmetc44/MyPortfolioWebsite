import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Profile, RawProfile, ContactMessage } from '../models/profile.model';
import { Project, RawProject } from '../models/project.model';
import { Article, RawArticle } from '../models/article.model';
import { Skill } from '../models/skill.model';
import { TechTag } from '../models/techtag.model';

export type { Profile, RawProfile, ContactMessage } from '../models/profile.model';
export type { Project, RawProject } from '../models/project.model';
export type { Article, RawArticle } from '../models/article.model';
export type { Skill } from '../models/skill.model';
export type { TechTag } from '../models/techtag.model';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private readonly updateTimeKey = 'admin_last_updated';

  private dataUpdatedSubject = new BehaviorSubject<void>(undefined);
  public dataUpdated$ = this.dataUpdatedSubject.asObservable();

  private cachedProfile: Profile | null = null;
  public isLoaded = false;
  private cachedProjects: Project[] | null = null;
  private cachedArticles: Article[] | null = null;
  private cachedSkills: Skill[] | null = null;
  private cachedTechTags: TechTag[] | null = null;

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
        const prof = await profRes.json();
        // Resolve avatar URL: if it's a relative upload path, prepend the API base URL
        if (prof.avatarUrl && !prof.avatarUrl.startsWith('http') && !prof.avatarUrl.startsWith('assets/')) {
          prof.avatarUrl = encodeURI(`${this.apiBaseUrl}/${prof.avatarUrl}`);
        }
        this.cachedProfile = prof;
      }

      const projRes = await fetch(`${this.apiBaseUrl}/api/projects?lang=${lang}`);
      if (projRes.ok) {
        const rawProjects = await projRes.json();
        this.cachedProjects = rawProjects.map((p: any) => {
          const rawImages = p.imagesJson ?? p.images ?? p.images_json;
          const parsedImages = parseProjectImages(rawImages);
          // Resolve each project image URL with sanitization and URI encoding
          const resolvedImages = parsedImages
            .map((img: string) => sanitizeImageUrl(img, this.apiBaseUrl))
            .filter(Boolean);
          return {
            ...p,
            images: resolvedImages
          };
        });
      }

      const artRes = await fetch(`${this.apiBaseUrl}/api/articles?lang=${lang}`);
      if (artRes.ok) {
        const articles = await artRes.json();
        // Resolve article imageUrl: if relative upload path, prepend the API base URL
        this.cachedArticles = articles.map((a: any) => {
          if (a.imageUrl && !a.imageUrl.startsWith('http') && !a.imageUrl.startsWith('assets/')) {
            a.imageUrl = encodeURI(`${this.apiBaseUrl}/${a.imageUrl}`);
          }
          return a;
        });
      }

      const skillsRes = await fetch(`${this.apiBaseUrl}/api/skills`);
      if (skillsRes.ok) {
        this.cachedSkills = await skillsRes.json();
      }

      const tagsRes = await fetch(`${this.apiBaseUrl}/api/techtags`);
      if (tagsRes.ok) {
        this.cachedTechTags = await tagsRes.json();
      }
    } catch (e) {
      console.warn("API Server not available.", e);
    } finally {
      this.isLoaded = true;
      this.dataUpdatedSubject.next();
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
      bio: "Loading...",
      avatarUrl: "assets/avatar_senior.png",
      repos: 0,
      pubs: 0,
      github: "https://github.com/mehmetc44",
      linkedin: "https://linkedin.com",
      instagram: "https://instagram.com",
      medium: "https://medium.com",
      isOpenToOffers: true
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

  getSkills(): Skill[] {
    return this.cachedSkills || [];
  }

  getTechTagsList(): TechTag[] {
    return this.cachedTechTags || [];
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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders(true),
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
      const res = await fetch(`${this.apiBaseUrl}/api/projects/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(profile)
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

  async getRawSkills(): Promise<Skill[]> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/skills`, {
        headers: this.getAuthHeaders()
      });
      if (res.ok) {
        return await res.json();
      }
    } catch(e) {
      console.error(e);
    }
    return [];
  }

  async saveRawSkill(skill: Skill, isNew: boolean): Promise<boolean> {
    try {
      const url = isNew 
        ? `${this.apiBaseUrl}/api/skills` 
        : `${this.apiBaseUrl}/api/skills/${skill.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(skill)
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

  async deleteRawSkill(id: number): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/skills/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders()
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

  async reorderSkills(items: { id: number, orderIndex: number }[]): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/skills/reorder`, {
        method: 'PUT',
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(items)
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

  async reorderProjects(items: { id: string, orderIndex: number }[]): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/projects/reorder`, {
        method: 'PUT',
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(items)
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

  async reorderArticles(items: { id: string, orderIndex: number }[]): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/articles/reorder`, {
        method: 'PUT',
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(items)
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

  async getRawTechTags(): Promise<TechTag[]> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/techtags`, {
        headers: this.getAuthHeaders()
      });
      if (res.ok) {
        return await res.json();
      }
    } catch(e) {
      console.error(e);
    }
    return [];
  }

  async saveRawTechTag(tag: TechTag, isNew: boolean): Promise<boolean> {
    try {
      const url = isNew 
        ? `${this.apiBaseUrl}/api/techtags` 
        : `${this.apiBaseUrl}/api/techtags/${tag.id}`;
      const method = isNew ? 'POST' : 'PUT';
      const res = await fetch(url, {
        method,
        headers: this.getAuthHeaders(true),
        body: JSON.stringify(tag)
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

  async deleteRawTechTag(id: number): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/techtags/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders(true),
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
      const res = await fetch(`${this.apiBaseUrl}/api/articles/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      });
      return res.ok;
    } catch(e) {
      console.error(e);
      return false;
    }
  }

  async publishArticleToMedium(id: string, token?: string): Promise<{ success: boolean; url?: string; message?: string }> {
    try {
      let url = `${this.apiBaseUrl}/api/articles/${id}/publish-medium`;
      if (token) {
        url += `?token=${encodeURIComponent(token)}`;
      }
      const res = await fetch(url, {
        method: 'POST',
        headers: this.getAuthHeaders()
      });
      return await res.json();
    } catch (e: any) {
      console.error("Medium publish error", e);
      return { success: false, message: e?.message || 'Sunucuya bağlanılamadı.' };
    }
  }

  // File Upload Helper APIs (for Admin Panel)
  async uploadBlogImage(file: File, slug?: string): Promise<string | null> {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      let url = `${this.apiBaseUrl}/api/upload/blog`;
      if (slug) {
        url += `?slug=${encodeURIComponent(slug)}`;
      }

      const res = await fetch(url, {
        method: 'POST',
        body: formData,
        headers: this.getAuthHeaders()
      });
      
      if (res.ok) {
        const data = await res.json();
        return data.url ? sanitizeImageUrl(data.url, this.apiBaseUrl) : null;
      }
      return null;
    } catch (e) {
      console.error("Blog image upload error", e);
      return null;
    }
  }

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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders()
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

  async uploadProjectImage(file: File, projectId: string): Promise<string | null> {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      const url = `${this.apiBaseUrl}/api/upload/project?projectId=${encodeURIComponent(projectId)}`;
      
      const res = await fetch(url, {
        method: 'POST',
        body: formData,
        headers: this.getAuthHeaders()
      });
      
      if (res.ok) {
        const data = await res.json();
        return data.url; // e.g. "storage/projects/slug/guid_filename.png"
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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders()
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
        headers: this.getAuthHeaders()
      });
      return res.ok;
    } catch (e) {
      console.error("Failed to delete message", e);
      return false;
    }
  }

  private getAuthHeaders(includeJsonContentType = false): Record<string, string> {
    const token = typeof window !== 'undefined' ? (localStorage.getItem('admin_token') || '') : '';
    const headers: Record<string, string> = {
      'X-Admin-Token': token
    };
    if (includeJsonContentType) {
      headers['Content-Type'] = 'application/json';
    }
    return headers;
  }

  // Authentication APIs
  async checkAuthStatus(): Promise<boolean> {
    try {
      const token = typeof window !== 'undefined' ? (localStorage.getItem('admin_token') || '') : '';
      const res = await fetch(`${this.apiBaseUrl}/api/auth/status`, {
        headers: this.getAuthHeaders()
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
        body: JSON.stringify({ username, password })
      });
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.token) {
          if (typeof window !== 'undefined') {
            localStorage.setItem('admin_token', data.token);
          }
          this._isAuthenticated = true;
          return true;
        }
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
        headers: this.getAuthHeaders()
      });
      if (res.ok) {
        if (typeof window !== 'undefined') {
          localStorage.removeItem('admin_token');
        }
        this._isAuthenticated = false;
        return true;
      }
    } catch(e) {
      console.error("Logout failed", e);
    }
    return false;
  }

  async forgotPassword(email: string): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/auth/forgot-password`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email })
      });
      return res.ok;
    } catch(e) {
      console.error("Forgot password request failed", e);
      return false;
    }
  }

  async resetPassword(token: string, newPassword: string): Promise<boolean> {
    try {
      const res = await fetch(`${this.apiBaseUrl}/api/auth/reset-password`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token, newPassword })
      });
      return res.ok;
    } catch(e) {
      console.error("Reset password request failed", e);
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

export function parseProjectImages(input: any): string[] {
  if (!input) return [];

  // If input is already an array
  if (Array.isArray(input)) {
    return input
      .map(item => (typeof item === 'string' ? item.trim() : String(item)))
      .map(item => item.replace(/^["']|["']$/g, '').trim())
      .filter(item => item.length > 0);
  }

  if (typeof input !== 'string') return [];

  let str = input.trim();
  if (!str) return [];

  // Try standard JSON.parse first
  try {
    const parsed = JSON.parse(str);
    if (Array.isArray(parsed)) {
      return parsed
        .map(item => (typeof item === 'string' ? item.trim() : String(item)))
        .map(item => item.replace(/^["']|["']$/g, '').trim())
        .filter(item => item.length > 0);
    }
    if (typeof parsed === 'string') {
      try {
        const doubleParsed = JSON.parse(parsed);
        if (Array.isArray(doubleParsed)) {
          return doubleParsed
            .map(item => (typeof item === 'string' ? item.trim() : String(item)))
            .map(item => item.replace(/^["']|["']$/g, '').trim())
            .filter(item => item.length > 0);
        }
      } catch (_) {}
      str = parsed.trim();
    }
  } catch (_) {}

  // Single-quoted array like ['link1', 'link2']
  if (str.includes("'")) {
    try {
      const fixedQuotes = str.replace(/'/g, '"');
      const parsed = JSON.parse(fixedQuotes);
      if (Array.isArray(parsed)) {
        return parsed
          .map(item => (typeof item === 'string' ? item.trim() : String(item)))
          .map(item => item.replace(/^["']|["']$/g, '').trim())
          .filter(item => item.length > 0);
      }
    } catch (_) {}
  }

  // Strip leading [ or { and trailing ] or }
  if ((str.startsWith('[') && str.endsWith(']')) || (str.startsWith('{') && str.endsWith('}'))) {
    str = str.slice(1, -1).trim();
  }

  // Split by comma
  if (str.includes(',')) {
    return str
      .split(',')
      .map(item => item.trim().replace(/^["']|["']$/g, '').trim())
      .filter(item => item.length > 0);
  }

  const clean = str.replace(/^["']|["']$/g, '').trim();
  return clean ? [clean] : [];
}

export function sanitizeImageUrl(rawUrl: string, apiBaseUrl?: string): string {
  if (!rawUrl) return '';
  let url = rawUrl.trim();
  if (url.startsWith('assets/')) return url;

  // 1. Remove concatenated domains if present
  const lastHttps = Math.max(url.lastIndexOf('https://'), url.lastIndexOf('http://'));
  if (lastHttps > 0) {
    url = url.substring(lastHttps);
  }

  // 2. Extract relative path after known folder prefixes
  const knownFolders = ['projects/', 'avatar/', 'articles/', 'cv/', 'uploads/'];
  for (const folder of knownFolders) {
    const folderIdx = url.lastIndexOf(folder);
    if (folderIdx >= 0) {
      const relativePath = url.substring(folderIdx);
      return `https://byewxuhvxtyvlxcxitwd.supabase.co/storage/v1/object/public/portfolio/${relativePath}`;
    }
  }

  // 3. If relative path starting with storage/
  if (url.toLowerCase().startsWith('storage/')) {
    const clean = url.substring(8).replace(/^\//, '');
    return `https://byewxuhvxtyvlxcxitwd.supabase.co/storage/v1/object/public/portfolio/${clean}`;
  }

  // 4. If full http/https URL
  if (url.startsWith('http://') || url.startsWith('https://')) {
    try {
      return encodeURI(decodeURI(url));
    } catch (_) {
      return encodeURI(url);
    }
  }

  // 5. Fallback relative path
  if (apiBaseUrl) {
    const clean = url.replace(/^\//, '');
    return encodeURI(`${apiBaseUrl}/${clean}`);
  }

  return encodeURI(url);
}
