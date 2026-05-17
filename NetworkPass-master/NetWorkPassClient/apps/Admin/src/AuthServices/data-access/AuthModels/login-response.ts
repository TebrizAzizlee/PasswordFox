export interface LoginResponse{
  success:boolean;
  requiresTfa:boolean;
  accessToken?:string;
}
