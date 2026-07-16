export interface Profile {
  name: string;
  tag: string;
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
  cvPdfUrl_TR?: string;
  cvPdfUrl_EN?: string;
  cvPdfUrl_DE?: string;

  // New fields
  job?: string;
  education?: string;
  motto?: string;
  isOpenToOffers?: boolean;
}

export interface RawProfile {
  id: number;
  name: string;
  tag_TR: string;
  tag_EN: string;
  tag_DE: string;
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

  // New fields
  job_TR?: string;
  job_EN?: string;
  job_DE?: string;
  education_TR?: string;
  education_EN?: string;
  education_DE?: string;
  motto_TR?: string;
  motto_EN?: string;
  motto_DE?: string;
  isOpenToOffers?: boolean;
}

export interface ContactMessage {
  id: number;
  name: string;
  email: string;
  message: string;
  date: string;
  isRead: boolean;
}

export interface CvItemExperience {
  title: string;
  org: string;
  date: string;
  bullets: string[];
}

export interface CvItemEducation {
  title: string;
  org: string;
  date: string;
  desc: string;
}

export interface CvItemCertificate {
  title: string;
  date: string;
}

export interface CvItemVolunteering {
  title: string;
  org: string;
  date: string;
  desc: string;
}

export interface CvItemLanguage {
  name: string;
  level: string;
  percentage: number;
}

export interface CvStructuredData {
  experiences: CvItemExperience[];
  educations: CvItemEducation[];
  certificates: CvItemCertificate[];
  volunteering: CvItemVolunteering[];
  languages: CvItemLanguage[];
}
