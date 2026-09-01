import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { CompanyRole } from '../../enums/company-role.enum';

export interface CompanyAdministratorDto {
    id: string;
    companyId: string;
    profileId: string;
    profile: ProfileSnapshotDto;
    adminRole: CompanyRole;
}
