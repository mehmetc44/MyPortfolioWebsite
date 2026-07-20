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
  isEditing = false;
  articleModalTitle = 'Yeni Makale Ekle';
  editingArticleIdx = -1;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';
  isPublishingToMedium = false;

  // Drag & Drop state
  dragIndex: number | null = null;
  dragOverIndex: number | null = null;
  isSavingOrder = false;

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
    this.isEditing = true;
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
    this.isEditing = true;
    this.initializeEditors();
  }

  closeArticleModal() {
    this.isEditing = false;
  }

  async publishToMedium(articleId?: string) {
    const idx = articleId 
      ? this.articles.findIndex(a => a.id === articleId) 
      : this.editingArticleIdx;
      
    const art = idx >= 0 ? this.articles[idx] : null;
    const targetId = articleId || (art ? art.id : this.artId);

    if (!targetId && !this.artTitle_TR) {
      alert('Lütfen önce makale bilgilerini girin.');
      return;
    }

    let token = localStorage.getItem('medium_integration_token') || '';

    // Explanation & Option Choice
    const useApi = confirm(
      "Medium yeni API Integration Token alımını durdurmuştur.\n\n" +
      "• Mevcut bir Integration Token'ınız varsa 'TAMAM' butonuna basarak API ile otomatik yayınlayabilirsiniz.\n" +
      "• Token'ınız yoksa 'İPTAL' butonuna basarak içeriği panoya kopyalayıp doğrudan Medium Editöründe açabilirsiniz."
    );

    if (useApi) {
      if (!token) {
        const input = prompt('Lütfen Medium Integration Token girin:\n(Eğer .env dosyasında MEDIUM_INTEGRATION_TOKEN ekliyse boş bırakabilirsiniz)');
        if (input !== null) {
          token = input.trim();
          if (token) {
            localStorage.setItem('medium_integration_token', token);
          }
        }
      }

      this.isPublishingToMedium = true;
      const res = await this.dataService.publishArticleToMedium(targetId, token);
      this.isPublishingToMedium = false;

      if (res.success && res.url) {
        if (confirm(`Makale Medium'da başarıyla yayınlandı!\n\nBağlantı: ${res.url}\n\nYeni sekmede açmak ister misiniz?`)) {
          window.open(res.url, '_blank');
        }
        return;
      } else {
        alert(`API ile paylaşım gerçekleşmedi:\n${res.message || 'Token geçersiz veya bulunamadı.'}\n\nİçerik kopyalanarak Medium editörüne aktarılıyor...`);
      }
    }

    // Fallback: Copy content to clipboard and open Medium New Story page
    const title = art ? (art.title_TR || art.title_EN) : (this.artTitle_TR || this.artTitle_EN);
    const excerpt = art ? (art.excerpt_TR || art.excerpt_EN) : (this.artExcerpt_TR || this.artExcerpt_EN);
    const detail = art ? (art.detailText_TR || art.detailText_EN) : (this.artDetail_TR || this.artDetail_EN);

    // Strip HTML tags for clean text paste fallback
    const textDetail = detail ? detail.replace(/<br\s*[\/]?>/gi, '\n').replace(/<\/p>/gi, '\n\n').replace(/<[^>]*>/g, '') : '';
    const fullContent = `${title}\n\n${excerpt ? excerpt + '\n\n---\n\n' : ''}${textDetail}`;

    try {
      await navigator.clipboard.writeText(fullContent);
      alert("Makale başlığı ve içeriği pano hafızasına kopyalandı! 📋\n\nAçılan Medium sayfasında 'Ctrl + V' yaparak yazınızı yayınlayabilirsiniz.");
    } catch (e) {
      console.warn("Clipboard access denied", e);
    }

    window.open('https://medium.com/new-story', '_blank');
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

  // ── Drag & Drop Handlers ──────────────────────────────────────

  onDragStart(index: number) {
    this.dragIndex = index;
  }

  onDragOver(event: DragEvent, index: number) {
    event.preventDefault();
    this.dragOverIndex = index;
  }

  onDragLeave() {
    this.dragOverIndex = null;
  }

  async onDrop(event: DragEvent, dropIndex: number) {
    event.preventDefault();
    if (this.dragIndex === null || this.dragIndex === dropIndex) {
      this.dragIndex = null;
      this.dragOverIndex = null;
      return;
    }
    const moved = this.articles.splice(this.dragIndex, 1)[0];
    this.articles.splice(dropIndex, 0, moved);
    this.dragIndex = null;
    this.dragOverIndex = null;
    await this.saveOrder();
  }

  onDragEnd() {
    this.dragIndex = null;
    this.dragOverIndex = null;
  }

  async saveOrder() {
    this.isSavingOrder = true;
    const items = this.articles.map((a, i) => ({ id: a.id, orderIndex: i }));
    await this.dataService.reorderArticles(items);
    this.isSavingOrder = false;
  }
}
