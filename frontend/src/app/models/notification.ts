export interface Notification {
  id: number;
  title: string;
  message: string;
  titleEn: string;
  messageEn: string;
  type: string;
  orderId: number | null;
  isRead: boolean;
  createdAt: string;
}
