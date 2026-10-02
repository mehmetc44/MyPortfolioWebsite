import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories.component.html',
  styleUrls: ['./admin-categories.component.css']
})
export class AdminCategoriesComponent implements OnInit {
  categories: any[] = [];
  
  expandedCategories: Set<string> = new Set();
  
  isEditing = false;
  editingId: string | null = null;
  
  editCat = {
    id: '',
    name_TR: '',
    name_EN: '',
    name_DE: '',
    isSubCategory: false,
    parentId: ''
  };

  isSaving = false;
  errorMessage = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadCategories();
  }

  async loadCategories() {
    this.categories = await this.dataService.getCategories();
  }

  toggleCategory(id: string, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    if (this.expandedCategories.has(id)) {
      this.expandedCategories.delete(id);
    } else {
      this.expandedCategories.add(id);
    }
  }

  isCategoryExpanded(id: string): boolean {
    return this.expandedCategories.has(id);
  }

  startNewMainCategory() {
    this.isEditing = true;
    this.editingId = null;
    this.editCat = {
      id: '',
      name_TR: '',
      name_EN: '',
      name_DE: '',
      isSubCategory: false,
      parentId: ''
    };
    this.errorMessage = '';
  }

  startNewSubCategory(parentId: string) {
    this.isEditing = true;
    this.editingId = null;
    this.editCat = {
      id: '',
      name_TR: '',
      name_EN: '',
      name_DE: '',
      isSubCategory: true,
      parentId: parentId
    };
    this.errorMessage = '';
  }

  editCategory(cat: any, parentId?: string) {
    this.isEditing = true;
    this.editingId = cat.id;
    this.editCat = {
      id: cat.id,
      name_TR: cat.name_TR,
      name_EN: cat.name_EN,
      name_DE: cat.name_DE,
      isSubCategory: cat.isSubCategory,
      parentId: parentId || ''
    };
    this.errorMessage = '';
  }

  cancelEdit() {
    this.isEditing = false;
    this.editingId = null;
    this.errorMessage = '';
  }

  async saveCategory() {
    if (!this.editCat.id || !this.editCat.name_TR) {
      this.errorMessage = 'Lütfen ID (Slug) ve Türkçe İsim giriniz.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const isNew = this.editingId === null;
    const success = await this.dataService.saveCategory(this.editCat, isNew);
    
    if (success) {
      this.isEditing = false;
      await this.loadCategories();
    } else {
      this.errorMessage = 'Kaydetme başarısız oldu.';
    }
    
    this.isSaving = false;
  }

  async deleteCategory(id: string) {
    if (!confirm('Bu kategoriyi silmek istediğinize emin misiniz? Alt kategoriler de silinebilir.')) {
      return;
    }
    
    const success = await this.dataService.deleteCategory(id);
    if (success) {
      await this.loadCategories();
    }
  }
}
