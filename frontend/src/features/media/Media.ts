export type Media = {
  id: string
  fullName: string,
  createdAt: string,
  viewCount: number,
  tags: string[],
  thumbnails: string[],
  isFavorite: boolean,
  width: number,
  height: number,
  fileSize: number,
  type: string,
  format: string,
  durationSeconds: number | null,
  bitRate: number | null, 
}
