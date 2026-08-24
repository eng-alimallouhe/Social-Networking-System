import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LucideShield, LucideUser, LucideUserCheck, LucideEyeOff, LucideLifeBuoy, LucideGhost } from '@lucide/angular';
import { LoginService } from '../../../identity/account-settings/security-sessions/login/services/login.service';
import { GlobalLoaderService } from '../../../shared/Loading/services/global-loader.service';
import { LoginWithPasswordRequest } from '../../../identity/account-settings/security-sessions/login/contracts/login-with-password-request.dto';
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';
import { RequestInformationService } from '../../../identity/shared/services/request-information.service';
import { TokenService } from '../../../identity/shared/services/token.service';

interface DemoRole {
    id: string;
    titleKey: string;
    email: string;
    icon: any;
}

@Component({
    selector: 'app-role-switcher',
    standalone: true,
    imports: [CommonModule, TranslatePipe],
    templateUrl: './role-switcher.html',
    styleUrl: './role-switcher.css',
})
export class RoleSwitcher {
    private loginService = inject(LoginService);
    private loadingService = inject(GlobalLoaderService);
    private router = inject(Router);
    private tokenService = inject(TokenService);
    private requestInfoService = inject(RequestInformationService);
    private authenticationService = inject(AuthenticationService);

    public isAuthenticated = this.authenticationService.isAuthenticated;
    public currentRole = this.authenticationService.currentRole;
    public activeRole = computed(() => {
        const roleId = this.currentRole()?.toLowerCase();
        return this.roles.find(r => r.id === roleId);
    });
    public guestIcon = LucideUser;

    private readonly SHARED_PASSWORD = 'alimallohi0947041713A';

    roles: DemoRole[] = [
        { id: 'admin', titleKey: 'Demo.RoleSwitcher.Roles.Admin', email: 'admin_omar@example.com', icon: LucideShield },
        { id: 'user', titleKey: 'Demo.RoleSwitcher.Roles.User', email: 'engalimallouhe@gmail.com', icon: LucideUser },
        { id: 'moderator', titleKey: 'Demo.RoleSwitcher.Roles.Moderator', email: 'moderator_sara@example.com', icon: LucideUserCheck },
        { id: 'support', titleKey: 'Demo.RoleSwitcher.Roles.Support', email: 'support_ahmad@example.com', icon: LucideLifeBuoy },
        { id: 'ghost', titleKey: 'Demo.RoleSwitcher.Roles.Ghost', email: 'deleted_user@gmail.com', icon: LucideGhost }
    ];

    navigateToDemoDashboard() {
        this.router.navigate(['/demo/dashboard']);
    }

    selectRole(role: DemoRole) {
        if (this.loadingService.isLoading()) return;

        const request: LoginWithPasswordRequest = {
            identifier: role.email,
            password: this.SHARED_PASSWORD
        };

        this.loadingService.show();

        this.loginService.loginWithPassword(request)
            .pipe(finalize(() => this.loadingService.hide()))
            .subscribe({
                next: (response) => {
                    if (response.value?.accessToken) {
                        this.tokenService.setToken(response.value.accessToken);
                        this.requestInfoService.setDeviceId(response.value.deviceId!);
                        this.router.navigate(['/demo/dashboard']);
                    }
                }
            });
    }
}