import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';
import { DataService, RawArticle } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-blog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-blog.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminBlogComponent implements OnInit {
  articles: RawArticle[] = [];
  isEditing = false;
  articleModalTitle = 'Yeni Makale Ekle';
  editingArticleIdx = -1;
  activeFormTab: 'tr' | 'en' | 'de' = 'tr';

  // Editor mode for each language tab: 'write' | 'preview'
  activeEditorTab_TR: 'write' | 'preview' = 'write';
  activeEditorTab_EN: 'write' | 'preview' = 'write';
  activeEditorTab_DE: 'write' | 'preview' = 'write';

  isPublishingToMedium = false;
  isUploadingImage = false;

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
  artImageUrl = '';
  artExcerpt_TR = '';
  artExcerpt_EN = '';
  artExcerpt_DE = '';
  artDetail_TR = '';
  artDetail_EN = '';
  artDetail_DE = '';

  constructor(private dataService: DataService, private sanitizer: DomSanitizer) {}

  ngOnInit() {
    this.loadArticles();
  }

  async loadArticles() {
    this.articles = await this.dataService.getRawArticles();
  }

  formatDate(dateStr?: string): string {
    return this.dataService.formatDate(dateStr);
  }

  // Language tab switching with automatic copy from TR if empty
  switchTab(tab: 'tr' | 'en' | 'de') {
    this.activeFormTab = tab;

    if (tab === 'en') {
      if (!this.artDetail_EN || !this.artDetail_EN.trim()) {
        this.copyTRToEN();
      }
    } else if (tab === 'de') {
      if (!this.artDetail_DE || !this.artDetail_DE.trim()) {
        this.copyTRToDE();
      }
    }
  }

  copyTRToEN() {
    this.artDetail_EN = this.artDetail_TR;
    if (!this.artTitle_EN) this.artTitle_EN = this.artTitle_TR;
    if (!this.artSubTag_EN) this.artSubTag_EN = this.artSubTag_TR;
    if (!this.artExcerpt_EN) this.artExcerpt_EN = this.artExcerpt_TR;
  }

  copyTRToDE() {
    this.artDetail_DE = this.artDetail_TR;
    if (!this.artTitle_DE) this.artTitle_DE = this.artTitle_TR;
    if (!this.artSubTag_DE) this.artSubTag_DE = this.artSubTag_TR;
    if (!this.artExcerpt_DE) this.artExcerpt_DE = this.artExcerpt_TR;
  }

  openNewArticleModal() {
    this.editingArticleIdx = -1;
    this.articleModalTitle = 'Yeni Makale Ekle';
    this.activeFormTab = 'tr';
    this.activeEditorTab_TR = 'write';
    this.activeEditorTab_EN = 'write';
    this.activeEditorTab_DE = 'write';

    this.artId = '';
    this.artImageUrl = '';
    this.artTitle_TR = '';
    this.artTitle_EN = '';
    this.artTitle_DE = '';
    this.artCategory = 'architecture';
    this.artDate = new Date().toISOString().split('T')[0];
    this.artReadTime = '5 dk';
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
  }

  openEditArticleModal(idx: number) {
    this.editingArticleIdx = idx;
    this.articleModalTitle = 'Makale Düzenle';
    this.activeFormTab = 'tr';
    this.activeEditorTab_TR = 'write';
    this.activeEditorTab_EN = 'write';
    this.activeEditorTab_DE = 'write';

    const art = this.articles[idx];
    this.artId = art.id;
    this.artImageUrl = art.imageUrl || '';
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
  }

  closeArticleModal() {
    this.isEditing = false;
  }

  triggerCoverImageUpload() {
    const fileInput = document.getElementById('blogCoverImageUploadInput') as HTMLInputElement;
    if (fileInput) fileInput.click();
  }

  async onCoverImageSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    this.isUploadingImage = true;
    const currentSlug = this.artId || this.artTitle_TR || 'general';
    const publicUrl = await this.dataService.uploadBlogImage(file, currentSlug + '-cover');
    this.isUploadingImage = false;

    if (publicUrl) {
      this.artImageUrl = publicUrl;
    } else {
      alert('Kapak görseli yüklenirken hata oluştu.');
    }
    event.target.value = '';
  }

  // Markdown Helper Actions (Matching Admin Projects Component)
  insertMarkdown(lang: 'tr' | 'en' | 'de', type: string) {
    const textareaId = lang === 'tr' ? 'crudBlogDetailTR' : (lang === 'en' ? 'crudBlogDetailEN' : 'crudBlogDetailDE');
    const textarea = document.getElementById(textareaId) as HTMLTextAreaElement;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    let text = '';

    if (lang === 'tr') text = this.artDetail_TR;
    else if (lang === 'en') text = this.artDetail_EN;
    else if (lang === 'de') text = this.artDetail_DE;

    const selectedText = text.substring(start, end);
    let replacement = '';
    let cursorOffset = 0;

    switch (type) {
      case 'bold':
        replacement = `**${selectedText || 'kalın yazı'}**`;
        cursorOffset = selectedText ? 0 : 2;
        break;
      case 'italic':
        replacement = `*${selectedText || 'eğik yazı'}*`;
        cursorOffset = selectedText ? 0 : 1;
        break;
      case 'code':
        replacement = `\`${selectedText || 'kod'}\``;
        cursorOffset = selectedText ? 0 : 1;
        break;
      case 'code-block':
        replacement = `\n\`\`\`javascript\n${selectedText || '// kodunuzu buraya yazın'}\n\`\`\`\n`;
        cursorOffset = selectedText ? 0 : 5;
        break;
      case 'link':
        replacement = `[${selectedText || 'bağlantı metni'}](https://)`;
        cursorOffset = selectedText ? 12 : 1;
        break;
      case 'list':
        replacement = `\n- ${selectedText || 'liste elemanı'}`;
        cursorOffset = selectedText ? 0 : 2;
        break;
      case 'h1':
        replacement = `\n# ${selectedText || 'Başlık 1'}\n`;
        cursorOffset = selectedText ? 0 : 2;
        break;
      case 'h2':
        replacement = `\n## ${selectedText || 'Başlık 2'}\n`;
        cursorOffset = selectedText ? 0 : 3;
        break;
      case 'h3':
        replacement = `\n### ${selectedText || 'Başlık 3'}\n`;
        cursorOffset = selectedText ? 0 : 4;
        break;
      case 'quote':
        replacement = `\n> ${selectedText || 'Alıntı metni'}\n`;
        cursorOffset = selectedText ? 0 : 2;
        break;
    }

    const newValue = text.substring(0, start) + replacement + text.substring(end);
    if (lang === 'tr') this.artDetail_TR = newValue;
    else if (lang === 'en') this.artDetail_EN = newValue;
    else if (lang === 'de') this.artDetail_DE = newValue;

    setTimeout(() => {
      textarea.focus();
      const newCursorPos = start + replacement.length - cursorOffset;
      textarea.setSelectionRange(newCursorPos, newCursorPos);
    }, 50);
  }

  triggerImageUpload() {
    const fileInput = document.getElementById('blogImageUploadInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  async onBlogImageSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    this.isUploadingImage = true;
    const currentSlug = this.artId || this.artTitle_TR || 'general';
    const publicUrl = await this.dataService.uploadBlogImage(file, currentSlug);
    this.isUploadingImage = false;

    if (!publicUrl) {
      alert('Görsel yüklenirken hata oluştu.');
      return;
    }

    this.insertImageMarkdown(publicUrl);
    event.target.value = '';
  }

  insertImageMarkdown(imageUrl: string) {
    const lang = this.activeFormTab;
    const textareaId = lang === 'tr' ? 'crudBlogDetailTR' : (lang === 'en' ? 'crudBlogDetailEN' : 'crudBlogDetailDE');
    const textarea = document.getElementById(textareaId) as HTMLTextAreaElement;

    const imageTag = `\n![Görsel Açıklaması](${imageUrl})\n`;

    if (textarea) {
      const start = textarea.selectionStart || 0;
      const end = textarea.selectionEnd || 0;
      let text = '';
      if (lang === 'tr') text = this.artDetail_TR;
      else if (lang === 'en') text = this.artDetail_EN;
      else if (lang === 'de') text = this.artDetail_DE;

      const newValue = text.substring(0, start) + imageTag + text.substring(end);
      if (lang === 'tr') this.artDetail_TR = newValue;
      else if (lang === 'en') this.artDetail_EN = newValue;
      else if (lang === 'de') this.artDetail_DE = newValue;

      setTimeout(() => {
        textarea.focus();
        const newCursorPos = start + imageTag.length;
        textarea.setSelectionRange(newCursorPos, newCursorPos);
      }, 50);
    } else {
      if (lang === 'tr') this.artDetail_TR += imageTag;
      else if (lang === 'en') this.artDetail_EN += imageTag;
      else if (lang === 'de') this.artDetail_DE += imageTag;
    }
  }

  getParsedMarkdown(markdownText: string): SafeHtml {
    if (!markdownText) return '';
    try {
      let parsedHtml = marked.parse(markdownText, { async: false }) as string;

      parsedHtml = parsedHtml.replace(/(?:<p>)?\s*(<img\s+[^>]*?>)\s*(?:<\/p>)?/gi, (fullMatch, imgTag) => {
        if (fullMatch.includes('blog-image-wrapper') || imgTag.includes('blog-image-wrapper')) {
          return fullMatch;
        }
        const srcMatch = imgTag.match(/src=["']([^"']+)["']/i);
        const altMatch = imgTag.match(/alt=["']([^"']+)["']/i);
        const src = srcMatch ? srcMatch[1] : '';
        const alt = altMatch ? altMatch[1] : '';
        
        if (!src) return fullMatch;

        const altAttr = alt ? alt.replace(/"/g, '&quot;') : 'Görsel';
        const captionText = alt && alt.trim() && alt !== 'Görsel' ? alt.trim() : 'Görsel Açıklaması';

        return `<figure class="blog-inline-figure">
  <div class="blog-image-wrapper">
    <img src="${src}" alt="${altAttr}" />
  </div>
  <figcaption>${captionText}</figcaption>
</figure>`;
      });

      parsedHtml = parsedHtml.replace(/<p>\s*(<figure[\s\S]*?<\/figure>)\s*<\/p>/gi, '$1');
      return this.sanitizer.bypassSecurityTrustHtml(parsedHtml);
    } catch (_) {
      return markdownText;
    }
  }

  slugify(text: string): string {
    if (!text) return 'article-' + Date.now();
    const unaccented = text.replace(/[çÇ]/g, 'c').replace(/[ğĞ]/g, 'g').replace(/[ıİ]/g, 'i').replace(/[öÖ]/g, 'o').replace(/[şŞ]/g, 's').replace(/[üÜ]/g, 'u');
    return unaccented.toString().toLowerCase().trim()
      .replace(/[^a-z0-9\-_]/g, '-')
      .replace(/-+/g, '-')
      .replace(/^-+|-+$/g, '');
  }

  async saveArticle() {
    if (!this.artTitle_TR || !this.artDate) {
      alert('Lütfen başlık (TR) ve tarih alanlarını doldurun.');
      return;
    }

    let slug = this.artId ? this.artId.trim() : '';
    if (!slug) {
      slug = this.slugify(this.artTitle_TR);
    }

    const payload: RawArticle = {
      id: slug,
      title_TR: this.artTitle_TR,
      title_EN: this.artTitle_EN || this.artTitle_TR,
      title_DE: this.artTitle_DE || this.artTitle_TR,
      category: this.artCategory,
      date: this.artDate,
      readTime: this.artReadTime || '5 dk',
      subTag_TR: this.artSubTag_TR,
      subTag_EN: this.artSubTag_EN || this.artSubTag_TR,
      subTag_DE: this.artSubTag_DE || this.artSubTag_TR,
      excerpt_TR: this.artExcerpt_TR,
      excerpt_EN: this.artExcerpt_EN || this.artExcerpt_TR,
      excerpt_DE: this.artExcerpt_DE || this.artExcerpt_TR,
      imageUrl: this.artImageUrl,
      detailText_TR: this.artDetail_TR,
      detailText_EN: this.artDetail_EN || this.artDetail_TR,
      detailText_DE: this.artDetail_DE || this.artDetail_TR
    };

    const isNew = this.editingArticleIdx === -1;
    const ok = await this.dataService.saveRawArticle(payload, isNew);
    
    if (ok) {
      await this.loadArticles();
      this.closeArticleModal();
    } else {
      alert('Makale kaydedilirken sunucu hatası oluştu.');
    }
  }

  async deleteArticle(idx: number) {
    const art = this.articles[idx];
    if (!art) return;
    if (confirm(`"${art.title_TR}" başlıklı makaleyi silmek istediğinize emin misiniz?`)) {
      const ok = await this.dataService.deleteRawArticle(art.id);
      if (ok) {
        await this.loadArticles();
      } else {
        alert('Makale silinirken hata oluştu.');
      }
    }
  }

  // Drag & drop sorting
  onDragStart(idx: number) {
    this.dragIndex = idx;
  }

  onDragOver(event: DragEvent, idx: number) {
    event.preventDefault();
    this.dragOverIndex = idx;
  }

  onDragLeave() {
    this.dragOverIndex = null;
  }

  async onDrop(event: DragEvent, dropIdx: number) {
    event.preventDefault();
    if (this.dragIndex === null || this.dragIndex === dropIdx) {
      this.dragIndex = null;
      this.dragOverIndex = null;
      return;
    }

    const itemToMove = this.articles.splice(this.dragIndex, 1)[0];
    this.articles.splice(dropIdx, 0, itemToMove);

    const reorderPayload = this.articles.map((art, index) => ({
      id: art.id,
      orderIndex: index + 1
    }));

    this.dragIndex = null;
    this.dragOverIndex = null;

    this.isSavingOrder = true;
    const ok = await this.dataService.reorderArticles(reorderPayload);
    this.isSavingOrder = false;

    if (!ok) {
      alert('Sıralama güncellenirken hata oluştu.');
      await this.loadArticles();
    }
  }

  onDragEnd() {
    this.dragIndex = null;
    this.dragOverIndex = null;
  }

  async publishToMedium() {
    const fullHtml = `<h1>${this.artTitle_TR}</h1>${this.artExcerpt_TR ? `<p><em>${this.artExcerpt_TR}</em></p><hr/>` : ''}${marked.parse(this.artDetail_TR)}`;
    const plainText = `${this.artTitle_TR}\n\n${this.artExcerpt_TR ? this.artExcerpt_TR + '\n\n---\n\n' : ''}${this.artDetail_TR}`;

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
      alert("✨ Makaleniz biçimlendirmeleriyle panoya kopyalandı!\n\nAçılan Medium sekmesinde 'Ctrl + V' yaptığınızda başlıklar, görseller ve kod blokları yapışacaktır.");
    } catch (e) {
      await navigator.clipboard.writeText(plainText);
      alert("Makale metni kopyalandı. Medium sekmesinde 'Ctrl + V' yapabilirsiniz.");
    }

    window.open('https://medium.com/new-story', '_blank');
  }
}
