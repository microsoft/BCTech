export class Language {
  languageTag: string;
  translationId: string;
  displayName: string;
  nativeName: string;
  fullName: string;
  localizedName: string;
}

export class ViewModel {
  reportId: string;
  embedUrl: string;
  token: string;
  langauges: Language[];
}
