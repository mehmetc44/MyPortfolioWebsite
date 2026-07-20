import { Component, OnInit, ViewChild, ElementRef, HostListener, OnDestroy } from '@angular/core';
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
export class AdminBlogComponent implements OnInit, OnDestroy {
  @ViewChild('editorTR') editorTR!: ElementRef<HTMLDivElement>;
  @ViewChild('editorEN') editorEN!: ElementRef<HTMLDivElement>;
  @ViewChild('editorDE') editorDE!: ElementRef<HTMLDivElement>;
  @ViewChild('blogImageInput') blogImageInput!: ElementRef<HTMLInputElement>;

  articles: RawArticle[] = [];
  isEditing = false;
  articleModalTitle = 'Yeni Makale Ekle';
  editingArticleIdx = -1;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';
  isPublishingToMedium = false;
  isUploadingImage = false;
  isFullscreenEditor = false;

  toggleFullscreenEditor() {
    this.isFullscreenEditor = !this.isFullscreenEditor;
    if (this.isFullscreenEditor) {
      document.body.classList.add('blog-fullscreen-active');
    } else {
      document.body.classList.remove('blog-fullscreen-active');
    }
  }

  @HostListener('window:keydown.escape', ['$event'])
  handleEscapeKey(event: KeyboardEvent) {
    if (this.isFullscreenEditor) {
      this.isFullscreenEditor = false;
      document.body.classList.remove('blog-fullscreen-active');
    }
  }

  ngOnDestroy() {
    document.body.classList.remove('blog-fullscreen-active');
  }

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
    this.isFullscreenEditor = false;
    document.body.classList.remove('blog-fullscreen-active');
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

    // Fallback: Rich Text (HTML + PlainText) Clipboard Copy
    const title = art ? (art.title_TR || art.title_EN) : (this.artTitle_TR || this.artTitle_EN);
    const excerpt = art ? (art.excerpt_TR || art.excerpt_EN) : (this.artExcerpt_TR || this.artExcerpt_EN);
    const detail = art ? (art.detailText_TR || art.detailText_EN) : (this.artDetail_TR || this.artDetail_EN);

    const fullHtml = `<h1>${title}</h1>${excerpt ? `<p><em>${excerpt}</em></p><hr/>` : ''}${detail}`;
    const plainText = `${title}\n\n${excerpt ? excerpt + '\n\n---\n\n' : ''}${detail ? detail.replace(/<br\s*[\/]?>/gi, '\n').replace(/<\/p>/gi, '\n\n').replace(/<[^>]*>/g, '') : ''}`;

    try {
      if (navigator.clipboard && typeof ClipboardItem !== 'undefined') {
        const htmlBlob = new Blob([fullHtml], { type: 'text/html' });
        const textBlob = new Blob([plainText], { type: 'text/plain' });
        const item = new ClipboardItem({
          'text/html': htmlBlob,
          'text/plain': textBlob
        });
        await navigator.clipboard.write([item]);
      } else {
        await navigator.clipboard.writeText(plainText);
      }
      alert("✨ Makaleniz başlık, özet ve HTML biçimlendirmeleriyle (Rich Text) panoya kopyalandı!\n\nAçılan Medium sekmesinde 'Ctrl + V' (Yapıştır) yaptığınızda başlıklar ve kod blokları biçimlendirilmiş olarak yapışacaktır.");
    } catch (e) {
      console.warn("Clipboard rich text failed", e);
      try {
        await navigator.clipboard.writeText(plainText);
        alert("Makale metni kopyalandı. Açılan Medium sekmesinde 'Ctrl + V' yaparak yapıştırabilirsiniz.");
      } catch (err) {}
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

  triggerImageUpload() {
    if (this.blogImageInput) {
      this.blogImageInput.nativeElement.click();
    }
  }

  async onBlogImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.isUploadingImage = true;

    try {
      const currentSlug = this.artId || this.artTitle_TR || 'general';
      const publicUrl = await this.dataService.uploadBlogImage(file, currentSlug);
      this.isUploadingImage = false;

      if (!publicUrl) {
        alert('Görsel Supabase (blog) klasörüne yüklenirken hata oluştu.');
        return;
      }

      this.insertImageToActiveEditor(publicUrl);
    } catch (e) {
      this.isUploadingImage = false;
      console.error(e);
      alert('Görsel yükleme hatası oluştu.');
    } finally {
      input.value = '';
    }
  }

  insertCodeBlock() {
    const codeHtml = `<pre class="blog-code-block" style="background:#0d1117; color:#e6edf3; padding:18px 22px; border-radius:12px; font-family:'Fira Code', monospace; font-size:0.95rem; line-height:1.65; white-space:pre-wrap; word-break:break-word; margin:24px 0; border:1px solid rgba(255,255,255,0.1); outline:none;"><code>// Kodunuzu buraya yazın...</code></pre><p><br></p>`;
    this.insertHtmlToActiveEditor(codeHtml);
  }

  insertSeparator() {
    const separatorHtml = `<div class="blog-separator" contenteditable="false" style="text-align:center; margin:36px 0; user-select:none;"><span style="color:#94a3b8; font-size:1.5rem; letter-spacing:14px; font-weight:800;">•••</span></div><p><br></p>`;
    this.insertHtmlToActiveEditor(separatorHtml);
  }

  insertHtmlToActiveEditor(html: string) {
    let targetElement: HTMLDivElement | null = null;
    if (this.activeFormTab === 'tr' && this.editorTR) targetElement = this.editorTR.nativeElement;
    else if (this.activeFormTab === 'en' && this.editorEN) targetElement = this.editorEN.nativeElement;
    else if (this.activeFormTab === 'de' && this.editorDE) targetElement = this.editorDE.nativeElement;

    if (targetElement) {
      targetElement.focus();
      document.execCommand('insertHTML', false, html);
      this.onEditorInput(this.activeFormTab, targetElement.innerHTML);
    }
  }

  insertImageToActiveEditor(imageUrl: string) {
    const figureHtml = `
      <figure class="blog-inline-figure" style="margin: 28px auto; text-align: center; max-width: 100%; position: relative; display: block;">
        <div class="image-controls-bar" contenteditable="false" style="display: flex; gap: 6px; justify-content: center; margin-bottom: 8px; user-select: none;">
          <button type="button" onclick="this.closest('figure').style.maxWidth='100%'" title="Tam Genişlik" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">%100</button>
          <button type="button" onclick="this.closest('figure').style.maxWidth='75%'" title="Orta Boyut" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">%75</button>
          <button type="button" onclick="this.closest('figure').style.maxWidth='50%'" title="Küçük Boyut" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">%50</button>
          <span style="border-left: 1px solid #ccc; margin: 0 4px;"></span>
          <button type="button" onclick="this.closest('figure').style.float='left'; this.closest('figure').style.margin='0 20px 20px 0';" title="Sola Hizala" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">⬅️ Sol</button>
          <button type="button" onclick="this.closest('figure').style.float='none'; this.closest('figure').style.margin='28px auto';" title="Ortala" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">↔️ Orta</button>
          <button type="button" onclick="this.closest('figure').style.float='right'; this.closest('figure').style.margin='0 0 20px 20px';" title="Sağa Hizala" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: rgba(11,26,48,0.08); border: 1px solid rgba(0,0,0,0.1); cursor: pointer; color: #1e293b;">➡️ Sağ</button>
          <span style="border-left: 1px solid #ccc; margin: 0 4px;"></span>
          <button type="button" onclick="this.closest('figure').remove()" title="Görseli Sil" style="padding: 3px 8px; font-size: 11px; font-weight: 700; border-radius: 6px; background: #fee2e2; border: 1px solid #fca5a5; cursor: pointer; color: #ef4444;">🗑️ Sil</button>
        </div>
        <img src="${imageUrl}" alt="Blog Görseli" style="width: 100%; height: auto; border-radius: 12px; box-shadow: 0 8px 30px rgba(0,0,0,0.12); display: block; margin: 0 auto;" />
        <figcaption contenteditable="true" style="font-size: 14px; color: #888; font-style: italic; margin-top: 8px; outline: none;">Görsel açıklaması eklemek için tıklayın...</figcaption>
      </figure>
      <p><br></p>
    `;
    this.insertHtmlToActiveEditor(figureHtml);
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
