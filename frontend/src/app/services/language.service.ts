import { Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type SupportedLang = 'tr' | 'en';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly STORAGE_KEY = 'lang';

  currentLang = signal<SupportedLang>(this.getInitialLang());

  constructor(private translate: TranslateService) {
    this.translate.addLangs(['tr', 'en']);
    this.translate.setDefaultLang('tr');
    this.translate.use(this.currentLang());
  }

  setLanguage(lang: SupportedLang): void {
    this.currentLang.set(lang);
    this.translate.use(lang);
    localStorage.setItem(this.STORAGE_KEY, lang);
  }

  private getInitialLang(): SupportedLang {
    const stored = localStorage.getItem(this.STORAGE_KEY) as SupportedLang | null;
    if (stored === 'tr' || stored === 'en') return stored;
    const browser = navigator.language.startsWith('tr') ? 'tr' : 'en';
    return browser;
  }
}
