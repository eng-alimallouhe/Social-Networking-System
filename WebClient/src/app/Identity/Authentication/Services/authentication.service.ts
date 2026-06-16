import { HttpClient } from "@angular/common/http";
import { Injectable, inject, signal } from "@angular/core";
import { Observable, tap } from "rxjs";
import { environment } from "../../../../environments/environment";
import { AuthTokenDto } from "../../Shared/DTOs/auth-token";
import { LoginRequestDto } from "../DTOs/login-request";
import { RefreshTokenRequestDto } from "../DTOs/refresh-token-request";
import { TokenService } from "../../Shared/Services/token.service";
import { Result } from "../../../shared/dtos/result.dto";

@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    private readonly apiUrl = environment.apiUrl + "identity/authentication/";
    private readonly tokenService = inject(TokenService);
    isAuthenticated = signal<boolean>(false);

    constructor(private readonly http: HttpClient) { }

    public Login(command: LoginRequestDto): Observable<Result<AuthTokenDto>> {
        return this.http.post<Result<AuthTokenDto>>(`${this.apiUrl}login`, command).pipe(
            tap((response: Result<AuthTokenDto>) => {
                this.tokenService.setToken(response.value?.accessToken ?? '', response.value?.refreshToken ?? '');
                this.isAuthenticated.set(true);
            })
        );
    }

    public Logout(): Observable<AuthTokenDto> {
        return this.http.post<AuthTokenDto>(`${this.apiUrl}logout`, {});
    }

    public RefreshToken(command: RefreshTokenRequestDto): Observable<AuthTokenDto> {
        return this.http.post<AuthTokenDto>(`${this.apiUrl}refresh`, command);
    }

    public checkIfUserIsAuthenticated(): boolean {
        const token = this.tokenService.getAccessToken();
        if (token) {
            this.isAuthenticated.set(true);
        }
        return this.isAuthenticated();
    }
}