export interface Article {
  id: string;
  title: string;
  categoryId: string;
  date: string;
  readTime: string;
  excerpt: string;
  imageUrl: string;
  detailText: string;
  isDraft?: boolean;
}

export interface RawArticle {
  id: string;
  title_TR: string;
  title_EN: string;
  title_DE: string;
  categoryId: string;
  date: string;
  readTime: string;
  excerpt_TR: string;
  excerpt_EN: string;
  excerpt_DE: string;
  imageUrl: string;
  detailText_TR: string;
  detailText_EN: string;
  detailText_DE: string;
  orderIndex?: number;
  isDraft?: boolean;
}
