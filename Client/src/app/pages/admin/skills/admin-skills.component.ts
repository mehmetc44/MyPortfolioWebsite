import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, Skill } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-skills.component.html',
  styleUrls: ['../admin.component.css']
})
export class AdminSkillsComponent implements OnInit {
  skills: Skill[] = [];
  selectedSkill: Skill | null = null;
  isNew = false;

  // Drag & Drop state
  dragIndex: number | null = null;
  dragOverIndex: number | null = null;
  isSavingOrder = false;

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadSkills();
  }

  async loadSkills() {
    this.skills = await this.dataService.getRawSkills();
  }

  selectSkill(skill: Skill) {
    this.selectedSkill = { ...skill };
    this.isNew = false;
  }

  createNew() {
    this.selectedSkill = {
      id: 0,
      name: '',
      percentage: 80
    };
    this.isNew = true;
  }

  cancelEdit() {
    this.selectedSkill = null;
  }

  async saveSkill() {
    if (!this.selectedSkill) return;
    if (!this.selectedSkill.name.trim()) {
      alert('Yetenek adı boş olamaz.');
      return;
    }

    const success = await this.dataService.saveRawSkill(this.selectedSkill, this.isNew);
    if (success) {
      alert('Yetenek başarıyla kaydedildi.');
      this.selectedSkill = null;
      await this.loadSkills();
    } else {
      alert('Yetenek kaydedilirken bir hata oluştu.');
    }
  }

  async deleteSkill(id: number) {
    if (confirm('Bu yeteneği silmek istediğinize emin misiniz?')) {
      const success = await this.dataService.deleteRawSkill(id);
      if (success) {
        alert('Yetenek silindi.');
        if (this.selectedSkill?.id === id) {
          this.selectedSkill = null;
        }
        await this.loadSkills();
      } else {
        alert('Yetenek silinirken bir hata oluştu.');
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

    // Reorder array
    const moved = this.skills.splice(this.dragIndex, 1)[0];
    this.skills.splice(dropIndex, 0, moved);

    this.dragIndex = null;
    this.dragOverIndex = null;

    // Save to backend
    await this.saveOrder();
  }

  onDragEnd() {
    this.dragIndex = null;
    this.dragOverIndex = null;
  }

  async saveOrder() {
    this.isSavingOrder = true;
    const items = this.skills.map((s, i) => ({ id: s.id, orderIndex: i }));
    await this.dataService.reorderSkills(items);
    this.isSavingOrder = false;
  }
}
