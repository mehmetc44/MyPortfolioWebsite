export interface Article {
  id: string;
  title: string;
  category: string;
  date: string;
  readTime: string;
  subTag: string;
  excerpt: string;
  imageUrl: string;
  detailText: string;
}

export interface RawArticle {
  id: string;
  title_TR: string;
  title_EN: string;
  title_DE: string;
  category: string;
  date: string;
  readTime: string;
  subTag_TR: string;
  subTag_EN: string;
  subTag_DE: string;
  excerpt_TR: string;
  excerpt_EN: string;
  excerpt_DE: string;
  imageUrl: string;
  detailText_TR: string;
  detailText_EN: string;
  detailText_DE: string;
  orderIndex?: number;
}
