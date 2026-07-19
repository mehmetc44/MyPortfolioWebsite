export interface Project {
  id: string;
  title: string;
  category: string;
  date: string;
  client: string;
  subTag: string;
  description: string;
  tech: string;
  repoUrl: string;
  demoUrl: string;
  images: string[];
  detailText: string;
}

export interface RawProject {
  id: string;
  title_TR: string;
  title_EN: string;
  title_DE: string;
  category: string;
  date: string;
  client: string;
  subTag_TR: string;
  subTag_EN: string;
  subTag_DE: string;
  description_TR: string;
  description_EN: string;
  description_DE: string;
  tech: string;
  repoUrl: string;
  demoUrl: string;
  imagesJson: string;
  detailText_TR: string;
  detailText_EN: string;
  detailText_DE: string;
  orderIndex?: number;
}
