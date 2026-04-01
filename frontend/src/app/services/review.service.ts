import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review } from '../models/review';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5243/api';

  getReviews(productId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.apiUrl}/products/${productId}/reviews`);
  }

  createReview(productId: number, rating: number, comment: string, image?: File): Observable<Review> {
    const formData = new FormData();
    formData.append('productId', productId.toString());
    formData.append('rating', rating.toString());
    if (comment) {
      formData.append('comment', comment);
    }
    if (image) {
      formData.append('image', image);
    }

    return this.http.post<Review>(`${this.apiUrl}/products/${productId}/reviews`, formData);
  }
}
