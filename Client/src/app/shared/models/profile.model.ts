export interface Profile {
  name: string;
  tag: string;
  title: string;
  bio: string;
  avatarUrl: string;
  repos: number;
  pubs: number;
  github: string;
  linkedin: string;
  instagram: string;
  medium: string;
  cvText?: string;
  cvPdfUrl?: string;
}

export interface RawProfile {
  id: number;
  name: string;
  tag_TR: string;
  tag_EN: string;
  tag_DE: string;
  title_TR: string;
  title_EN: string;
  title_DE: string;
  bio_TR: string;
  bio_EN: string;
  bio_DE: string;
  avatarUrl: string;
  repos: number;
  pubs: number;
  github: string;
  linkedin: string;
  instagram: string;
  medium: string;
  cvText_TR?: string;
  cvText_EN?: string;
  cvText_DE?: string;
  cvPdfUrl_TR?: string;
  cvPdfUrl_EN?: string;
  cvPdfUrl_DE?: string;
}

export interface ContactMessage {
  id: number;
  name: string;
  email: string;
  message: string;
  date: string;
  isRead: boolean;
}
