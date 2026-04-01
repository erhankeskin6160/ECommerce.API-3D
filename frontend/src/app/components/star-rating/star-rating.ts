import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-star-rating',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './star-rating.html',
  styleUrl: './star-rating.scss'
})
export class StarRating {
  @Input() rating = 0;
  @Input() readonly = false;
  @Output() ratingChange = new EventEmitter<number>();
  
  hoverRating = signal(0);
  
  stars = [1, 2, 3, 4, 5];

  setRating(val: number) {
    if (this.readonly) return;
    this.rating = val;
    this.ratingChange.emit(this.rating);
  }

  setHover(val: number) {
    if (this.readonly) return;
    this.hoverRating.set(val);
  }

  clearHover() {
    if (this.readonly) return;
    this.hoverRating.set(0);
  }
}
