export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;
  categoryId?: number;
  category?: string;
  nameTr?: string;
  descriptionTr?: string;
  additionalImages?: string[];
  createdAt: string;
  reviews?: any[];
}
