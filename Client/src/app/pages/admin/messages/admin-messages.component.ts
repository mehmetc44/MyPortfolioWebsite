import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService, ContactMessage } from '../../../shared/services/data.service';

@Component({
  selector: 'app-admin-messages',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-messages.component.html',
  styleUrls: ['./admin-messages.component.css', '../admin.component.css']
})
export class AdminMessagesComponent implements OnInit {
  messages: ContactMessage[] = [];
  filteredMessages: ContactMessage[] = [];
  activeFilter: 'all' | 'unread' | 'read' = 'all';
  selectedMessage: ContactMessage | null = null;

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadMessages();
  }

  async loadMessages() {
    this.messages = await this.dataService.getAdminMessages();
    this.applyFilter();
  }

  applyFilter() {
    if (this.activeFilter === 'unread') {
      this.filteredMessages = this.messages.filter(m => !m.isRead);
    } else if (this.activeFilter === 'read') {
      this.filteredMessages = this.messages.filter(m => m.isRead);
    } else {
      this.filteredMessages = [...this.messages];
    }

    // Keep selected message reference updated if it exists in current load
    if (this.selectedMessage) {
      const updated = this.messages.find(m => m.id === this.selectedMessage!.id);
      this.selectedMessage = updated || null;
    }

    this.dataService.setUnreadCount(this.getUnreadCount());
  }

  selectFilter(filter: 'all' | 'unread' | 'read') {
    this.activeFilter = filter;
    this.applyFilter();
  }

  async readMessage(msg: ContactMessage) {
    this.selectedMessage = msg;
    if (!msg.isRead) {
      const ok = await this.dataService.markMessageAsRead(msg.id, true);
      if (ok) {
        // Update local state without full reload
        msg.isRead = true;
        this.applyFilter();
      }
    }
  }

  async toggleRead(msg: ContactMessage, event: MouseEvent) {
    event.stopPropagation();
    const newStatus = !msg.isRead;
    const ok = await this.dataService.markMessageAsRead(msg.id, newStatus);
    if (ok) {
      msg.isRead = newStatus;
      this.applyFilter();
    }
  }

  async deleteMessage(msg: ContactMessage, event: MouseEvent) {
    event.stopPropagation();
    if (confirm('Bu mesajı kalıcı olarak silmek istediğinize emin misiniz?')) {
      const ok = await this.dataService.deleteMessage(msg.id);
      if (ok) {
        if (this.selectedMessage && this.selectedMessage.id === msg.id) {
          this.selectedMessage = null;
        }
        await this.loadMessages();
      } else {
        alert('Mesaj silinirken hata oluştu.');
      }
    }
  }

  getUnreadCount(): number {
    return this.messages.filter(m => !m.isRead).length;
  }
}
