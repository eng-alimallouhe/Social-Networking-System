import { HttpClient } from "@angular/common/http";
import { inject, Inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.development";
import { SkillSummaryDto } from "../dtos/skill-summary.dto";
import { delay, map, Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class SkillService {
    private http = inject(HttpClient);
    private apiUrl = environment.apiUrl + 'preferences/skills';

    searchSkills(query: string): Observable<SkillSummaryDto[]> {
        return this.http.get<SkillSummaryDto[]>(
            this.apiUrl + '/search'
        ).pipe(
            delay(500),
            map(data => data.filter(skill =>
                skill.name.toLowerCase().includes(query.toLowerCase()))
            )
        );
        // return this.http.get<SkillSummaryDto[]>(`${this.apiUrl}/search?query=${query}`);
    }
}