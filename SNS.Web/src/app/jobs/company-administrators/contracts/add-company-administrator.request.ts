import { CompanyRole } from '../../enums/company-role.enum';

export interface AddCompanyAdministratorRequest {
    targetProfileId: string;
    role?: CompanyRole;
}
