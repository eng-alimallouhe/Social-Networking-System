import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideNotebookPen, LucideRocket, LucideSparkle } from '@lucide/angular';

@Component({
  selector: 'app-creation-hub',
  imports: [TranslatePipe, LucideNotebookPen, LucideSparkle, LucideRocket ],
  templateUrl: './creation-hub.component.html',
  styleUrl: './creation-hub.component.css',
})
export class CreationHubComponent {}
