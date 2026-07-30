import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { LucideShield, LucideUser, LucideUserCheck, LucideEyeOff, LucideLifeBuoy, LucideGhost } from '@lucide/angular';

import { LoginService } from '../../../identity/security-sesstions/login/services/login.service';
import { DemoLoadingService } from '../../services/demo-loading.service';
import { ToastService } from '../../../identity/notifications/services/toast.service';
import { TokenService } from '../../../identity/shared/services/token.service';
import { RequestInformationService } from '../../../identity/shared/services/request-information.service';
import { LoginWithPasswordRequest } from '../../../identity/security-sesstions/login/contracts/login-with-password-request.dto';

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
    private loadingService = inject(DemoLoadingService);
    private toastService = inject(ToastService);
    private router = inject(Router);
    private tokenService = inject(TokenService);
    private requestInfoService = inject(RequestInformationService);
    private translateService = inject(TranslateService);

    private readonly SHARED_PASSWORD = 'alimallohi0947041713A';

    roles: DemoRole[] = [
        { id: 'admin', titleKey: 'Demo.RoleSwitcher.Roles.Admin', email: 'admin_omar@example.com', icon: LucideShield },
        { id: 'user', titleKey: 'Demo.RoleSwitcher.Roles.User', email: 'engalimallouhe@gmail.com', icon: LucideUser },
        { id: 'moderator', titleKey: 'Demo.RoleSwitcher.Roles.Moderator', email: 'moderator_sara@example.com', icon: LucideUserCheck },
        { id: 'support', titleKey: 'Demo.RoleSwitcher.Roles.Support', email: 'support_ahmad@example.com', icon: LucideLifeBuoy },
        { id: 'ghost', titleKey: 'Demo.RoleSwitcher.Roles.Ghost', email: 'deleted_user@gmail.com', icon: LucideGhost }
    ];

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
                        this.tokenService.setToken(response.value.accessToken, response.value.refreshToken!);
                        this.requestInfoService.setDeviceId(response.value.deviceId!);
                        this.router.navigate(['/demo/dashboard']);
                    }
                },
                error: (err) => {
                    const errorResult = err.error;
                    if (errorResult && errorResult.statusCode) {
                        const { category, code } = errorResult.statusCode;
                        forkJoin({
                            message: this.translateService.get(`Status_Codes.${category}.${code}`),
                            title: this.translateService.get(`Status_Codes.Shared.Error_Title`)
                        }).subscribe(translations => {
                            this.toastService.error(translations.title, translations.message, 5000);
                        });
                    }
                }
            });
    }
}
