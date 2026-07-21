import { Pipe, PipeTransform } from '@angular/core';
import { LocalizationService } from '../services/localization.service';

@Pipe({
  name: 'localeUppercase',
  standalone: true,
  pure: false
})
export class LocaleUppercasePipe implements PipeTransform {
  constructor(private localizationService: LocalizationService) {}

  transform(value: string | null | undefined): string {
    return this.localizationService.toUpperCase(value);
  }
}
