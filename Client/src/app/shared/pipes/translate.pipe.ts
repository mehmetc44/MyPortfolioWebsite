import { Pipe, PipeTransform } from '@angular/core';
import { LocalizationService } from '../services/localization.service';

@Pipe({
  name: 'translate',
  standalone: true,
  pure: false
})
export class TranslatePipe implements PipeTransform {
  constructor(private localizationService: LocalizationService) {}

  transform(key: string, caseType?: 'upper' | 'uppercase' | 'lower' | 'lowercase'): string {
    const translated = this.localizationService.translate(key);
    if (caseType === 'upper' || caseType === 'uppercase') {
      return this.localizationService.toUpperCase(translated);
    }
    if (caseType === 'lower' || caseType === 'lowercase') {
      return this.localizationService.toLowerCase(translated);
    }
    return translated;
  }
}
