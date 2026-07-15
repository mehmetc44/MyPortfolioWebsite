import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, RawArticle } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-blog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-blog.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminBlogComponent implements OnInit {
  @ViewChild('editorTR') editorTR!: ElementRef<HTMLDivElement>;
  @ViewChild('editorEN') editorEN!: ElementRef<HTMLDivElement>;
  @ViewChild('editorDE') editorDE!: ElementRef<HTMLDivElement>;

  articles: RawArticle[] = [];
  showArticleModal = false;
  articleModalTitle = 'Yeni Makale Ekle';
  editingArticleIdx = -1;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';

  artId = '';
  artTitle_TR = '';
  artTitle_EN = '';
  artTitle_DE = '';
  artCategory = 'architecture';
  artDate = '';
  artReadTime = '';
  artSubTag_TR = '';
  artSubTag_EN = '';
  artSubTag_DE = '';
  artExcerpt_TR = '';
  artExcerpt_EN = '';
  artExcerpt_DE = '';
  artDetail_TR = '';
  artDetail_EN = '';
  artDetail_DE = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadArticles();
  }

  async loadArticles() {
    this.articles = await this.dataService.getRawArticles();
  }

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  openNewArticleModal() {
    this.editingArticleIdx = -1;
    this.articleModalTitle = 'Yeni Makale Ekle';
    this.activeFormTab = 'tr';
    
    this.artId = '';
    this.artTitle_TR = '';
    this.artTitle_EN = '';
    this.artTitle_DE = '';
    this.artCategory = 'architecture';
    this.artDate = '';
    this.artReadTime = '';
    this.artSubTag_TR = '';
    this.artSubTag_EN = '';
    this.artSubTag_DE = '';
    this.artExcerpt_TR = '';
    this.artExcerpt_EN = '';
    this.artExcerpt_DE = '';
    this.artDetail_TR = '';
    this.artDetail_EN = '';
    this.artDetail_DE = '';
    this.showArticleModal = true;
    this.initializeEditors();
  }

  openEditArticleModal(idx: number) {
    this.editingArticleIdx = idx;
    this.articleModalTitle = 'Makale Düzenle';
    this.activeFormTab = 'tr';
    const art = this.articles[idx];
    
    this.artId = art.id;
    this.artTitle_TR = art.title_TR;
    this.artTitle_EN = art.title_EN;
    this.artTitle_DE = art.title_DE;
    this.artCategory = art.category;
    this.artDate = art.date;
    this.artReadTime = art.readTime;
    this.artSubTag_TR = art.subTag_TR;
    this.artSubTag_EN = art.subTag_EN;
    this.artSubTag_DE = art.subTag_DE;
    this.artExcerpt_TR = art.excerpt_TR;
    this.artExcerpt_EN = art.excerpt_EN;
    this.artExcerpt_DE = art.excerpt_DE;
    this.artDetail_TR = art.detailText_TR;
    this.artDetail_EN = art.detailText_EN;
    this.artDetail_DE = art.detailText_DE;
    this.showArticleModal = true;
    this.initializeEditors();
  }

  closeArticleModal() {
    this.showArticleModal = false;
  }

  initializeEditors() {
    setTimeout(() => {
      if (this.editorTR) this.editorTR.nativeElement.innerHTML = this.artDetail_TR || '';
      if (this.editorEN) this.editorEN.nativeElement.innerHTML = this.artDetail_EN || '';
      if (this.editorDE) this.editorDE.nativeElement.innerHTML = this.artDetail_DE || '';
    }, 150);
  }

  onEditorInput(lang: 'tr' | 'en' | 'de', html: string) {
    if (lang === 'tr') this.artDetail_TR = html;
    else if (lang === 'en') this.artDetail_EN = html;
    else if (lang === 'de') this.artDetail_DE = html;
  }

  execCmd(command: string, value: string = '') {
    document.execCommand(command, false, value);
    this.updateValuesFromDOM();
  }

  updateValuesFromDOM() {
    if (this.editorTR) this.artDetail_TR = this.editorTR.nativeElement.innerHTML;
    if (this.editorEN) this.artDetail_EN = this.editorEN.nativeElement.innerHTML;
    if (this.editorDE) this.artDetail_DE = this.editorDE.nativeElement.innerHTML;
  }

  async saveArticle() {
    const slugId = this.artId.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '') || 
                   this.artTitle_TR.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
                   
    const artData: RawArticle = {
      id: this.editingArticleIdx >= 0 ? this.articles[this.editingArticleIdx].id : slugId,
      title_TR: this.artTitle_TR,
      title_EN: this.artTitle_EN,
      title_DE: this.artTitle_DE,
      category: this.artCategory,
      date: this.artDate,
      readTime: this.artReadTime,
      subTag_TR: this.artSubTag_TR,
      subTag_EN: this.artSubTag_EN,
      subTag_DE: this.artSubTag_DE,
      excerpt_TR: this.artExcerpt_TR,
      excerpt_EN: this.artExcerpt_EN,
      excerpt_DE: this.artExcerpt_DE,
      detailText_TR: this.artDetail_TR,
      detailText_EN: this.artDetail_EN,
      detailText_DE: this.artDetail_DE,
      imageUrl: this.editingArticleIdx >= 0 ? (this.articles[this.editingArticleIdx].imageUrl || "assets/blog_placeholder.png") : "assets/blog_placeholder.png"
    };

    const isNew = this.editingArticleIdx < 0;
    const ok = await this.dataService.saveRawArticle(artData, isNew);
    if (ok) {
      await this.loadArticles();
      this.closeArticleModal();
    } else {
      alert("Makale kaydedilirken sunucu hatası oluştu.");
    }
  }

  async deleteArticle(idx: number) {
    if (confirm('Bu makaleyi silmek istediğinizden emin misiniz?')) {
      const art = this.articles[idx];
      const ok = await this.dataService.deleteRawArticle(art.id);
      if (ok) {
        await this.loadArticles();
      } else {
        alert("Makale silinirken sunucu hatası oluştu.");
      }
    }
  }
}
