import { Component, computed, signal } from '@angular/core';

const FACE_PIPS: Record<number, Set<number>> = {
  1: new Set([4]),
  2: new Set([2, 6]),
  3: new Set([2, 4, 6]),
  4: new Set([0, 2, 6, 8]),
  5: new Set([0, 2, 4, 6, 8]),
  6: new Set([0, 2, 3, 5, 6, 8]),
};

const SLOTS = [0, 1, 2, 3, 4, 5, 6, 7, 8];

@Component({
  selector: 'app-dice',
  standalone: true,
  templateUrl: './dice.component.html',
  styleUrl: './dice.component.scss',
})
export class DiceComponent {
  protected readonly slots = SLOTS;
  protected readonly rolling = signal(false);

  private readonly currentFace = signal(1);
  protected readonly activePips = computed(() => FACE_PIPS[this.currentFace()]);

  roll(result: number): void {
    if (this.rolling()) return;
    this.rolling.set(true);

    const steps = 14;
    let step = 0;

    const schedule = (): void => {
      if (step >= steps - 1) {
        this.currentFace.set(result);
        this.rolling.set(false);
        return;
      }
      this.currentFace.set(Math.floor(Math.random() * 6) + 1);
      step++;
      // ease out: 40ms at start → ~320ms at end
      setTimeout(schedule, 40 + (step / steps) * 280);
    };

    schedule();
  }
}
