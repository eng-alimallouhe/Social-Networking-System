import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loader',
  imports: [CommonModule],
  templateUrl: './loader.html',
  styleUrl: './loader.css',
})
export class Loader {
  protected readonly LoaderType = LoaderType;
  protected readonly LoaderSize = LoaderSize;

  @Input() type: LoaderType = LoaderType.Circle;
  @Input() size: LoaderSize = LoaderSize.sm;

  getSizeKey(): string {
    return LoaderSize[this.size];
  }

  isCircle(): boolean {
    return this.type === LoaderType.Circle;
  }

  isProfileLoader(): boolean {
    return this.type === LoaderType.ProfileLoader;
  }

  isCommentLoader(): boolean {
    return this.type === LoaderType.CommentLoader;
  }

  isPostLoader(): boolean {
    return this.type === LoaderType.PostLoader;
  }

  isLine(): boolean {
    return this.type === LoaderType.Line;
  }
}

export enum LoaderType {
  Circle,
  ProfileLoader,
  CommentLoader,
  PostLoader,
  Line
}

export enum LoaderSize {
  sm,
  lg,
  vg
}
