import { Component, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StarRating } from '../star-rating/star-rating';
import { ReviewService } from '../../services/review.service';
import { Review } from '../../models/review';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-review-form',
  standalone: true,
  imports: [CommonModule, FormsModule, StarRating, TranslateModule],
  templateUrl: './review-form.html',
  styleUrl: './review-form.scss'
})
export class ReviewForm {
  @Input() productId!: number;
  @Output() reviewAdded = new EventEmitter<Review>();

  reviewService = inject(ReviewService);
  translateService = inject(TranslateService);

  rating = 0;
  comment = '';
  selectedFile: File | null = null;
  selectedFileUrl = signal<string | null>(null);

  isSubmitting = signal(false);
  error = signal('');

  onFileSelected(event: any) {
    const file = event.target.files[0] as File;
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e) => this.selectedFileUrl.set(e.target?.result as string);
      reader.readAsDataURL(file);
    }
  }

  removeImage() {
    this.selectedFile = null;
    this.selectedFileUrl.set(null);
  }

  submitReview() {
    if (this.rating < 1 || this.rating > 5) {
      this.error.set(this.translateService.instant('REVIEW_FORM.ERROR_RATING_REQUIRED'));
      return;
    }

    this.isSubmitting.set(true);
    this.error.set('');

    this.reviewService.createReview(this.productId, this.rating, this.comment, this.selectedFile || undefined)
      .subscribe({
        next: (review) => {
          this.isSubmitting.set(false);
          this.reviewAdded.emit(review);
          // reset form
          this.rating = 0;
          this.comment = '';
          this.removeImage();
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.error.set(this.translateService.instant('REVIEW_FORM.ERROR_SUBMIT'));
        }
      });
  }
}
