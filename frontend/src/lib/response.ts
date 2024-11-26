export type ApiResponse<T> = {
  value: T,
  formatters: string[] ,
  contentTypes: string[] ,
  declaredTypes: string | null,
  statusCode: number 
}
