export interface Review {
  id: number;
  productId: number;
  userId: string;
  userName?: string;
  rating: number;
  comment?: string;
  imageUrl?: string;
  createdAt: string;
}
