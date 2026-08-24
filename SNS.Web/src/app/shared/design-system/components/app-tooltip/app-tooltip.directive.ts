import { Directive, Input, ElementRef, ComponentRef, OnDestroy, OnChanges, SimpleChanges, inject } from '@angular/core';
import { Overlay, OverlayRef, OverlayPositionBuilder } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { AppTooltip } from './app-tooltip';

@Directive({
  selector: '[appTooltip]',
  standalone: true
})
export class AppTooltipDirective implements OnDestroy, OnChanges {
  @Input('appTooltip') messageKey: string = '';
  @Input() tooltipVisible: boolean = false;

  private overlayRef: OverlayRef | null = null;
  private tooltipRef: ComponentRef<AppTooltip> | null = null;

  private overlay = inject(Overlay);
  private overlayPositionBuilder = inject(OverlayPositionBuilder);
  private elementRef = inject(ElementRef);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tooltipVisible']) {
      if (this.tooltipVisible) {
        this.show();
      } else {
        this.hide();
      }
    }
  }

  show(): void {
    if (this.overlayRef) {
      return;
    }

    const positionStrategy = this.overlayPositionBuilder
      .flexibleConnectedTo(this.elementRef)
      .withPositions([{
        originX: 'center',
        originY: 'top',
        overlayX: 'center',
        overlayY: 'bottom',
        offsetY: -8,
      }]);

    this.overlayRef = this.overlay.create({ positionStrategy });
    const tooltipPortal = new ComponentPortal(AppTooltip);
    this.tooltipRef = this.overlayRef.attach(tooltipPortal);

    this.tooltipRef.instance.messageKey = this.messageKey;
  }

  hide(): void {
    if (this.overlayRef) {
      this.overlayRef.detach();
      this.overlayRef.dispose();
      this.overlayRef = null;
    }
  }

  ngOnDestroy(): void {
    this.hide();
  }
}
