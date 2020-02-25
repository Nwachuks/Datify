export interface Message {
  id: number;
  senderId: number;
  senderFullName: string;
  senderPhotoUrl: string;
  recipientId: number;
  recipientFullName: string;
  recipientPhotoUrl: string;
  content: string;
  isRead: boolean;
  dateRead: Date;
  messageSent: Date;
}
