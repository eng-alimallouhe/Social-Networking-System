import { CompanyRole } from '../../enums/company-role.enum';

export interface ChangeCompanyAdministratorRoleRequest {
    newRole: CompanyRole;
}
