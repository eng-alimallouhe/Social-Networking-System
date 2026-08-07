export interface CreateProfileRequest {
  fullName: string;
  bio: string | null;
  profilePicture: File | null;
  specialization: string | null;
}