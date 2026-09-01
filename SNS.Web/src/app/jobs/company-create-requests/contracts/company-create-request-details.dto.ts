import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { CompanyCreateRequestStatus } from '../../enums/company-create-request-status.enum';

export interface CompanyCreateRequestDetailsDto {
    id: string;
    profileId: string;
    profile: ProfileSnapshotDto;
    name: string;
    industry: string;
    websiteUrl: string | null;
    logoUrl: string | null;
    status: CompanyCreateRequestStatus;
    createdCompanyId: string | null;
    reviewedByProfileId: string | null;
    reviewNote: string | null;
    createdAt: string;
    reviewedAt: string | null;
}
