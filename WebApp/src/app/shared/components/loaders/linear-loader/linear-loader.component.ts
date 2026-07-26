import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-linear-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './linear-loader.component.html',
  styleUrls: ['./linear-loader.component.css']
})
export class LinearLoaderComponent {
  @Input() width: string = '100%';
}
