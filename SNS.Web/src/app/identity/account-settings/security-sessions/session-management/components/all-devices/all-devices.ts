import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideLaptop, LucideSmartphone, LucideTablet, LucideArrowLeft } from '@lucide/angular';
import { Router } from '@angular/router';
import { SessionManagementService } from '../../services/session-management.service';
import { RegisteredDeviceDto } from '../../contracts/registered-device.dto';
import { getDeviceIcon } from '../shared/device-icon.helper';

@Component({
    selector: 'app-all-devices',
    standalone: true,
    imports: [CommonModule, TranslatePipe, LucideLaptop, LucideSmartphone, LucideTablet, LucideArrowLeft],
    templateUrl: './all-devices.html',
    styleUrls: ['./all-devices.css']
})
export class AllDevicesComponent implements OnInit {
    private sessionService = inject(SessionManagementService);
    private router = inject(Router);

    registeredDevices = signal<RegisteredDeviceDto[]>([]);
    isLoading = signal<boolean>(true);
    error = signal<string | null>(null);

    ngOnInit(): void {
        this.loadDevices();
    }

    loadDevices(): void {
        this.isLoading.set(true);
        this.error.set(null);
        this.sessionService.getUserActiveSessionsAndDevices().subscribe({
            next: (result) => {
                if (result.isSuccess && result.value) {
                    this.registeredDevices.set(result.value.registeredDevices);
                } else {
                    this.error.set('Failed to load devices');
                }
                this.isLoading.set(false);
            },
            error: () => {
                this.error.set('Failed to load devices');
                this.isLoading.set(false);
            }
        });
    }

    getIconName(os: string, deviceName: string): string {
        return getDeviceIcon(os, deviceName);
    }

    goBack(): void {
        this.router.navigate(['/account-settings/security-settings/sessions']);
    }
}
