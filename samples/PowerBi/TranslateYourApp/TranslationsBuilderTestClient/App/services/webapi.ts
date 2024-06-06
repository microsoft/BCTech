import * as $ from 'jquery';
import { ViewMode } from 'powerbi-models';
import { Language, ViewModel } from '../models/models';

export class WebApi {

  static GetViewModel = async (): Promise<ViewModel> => {
    var restUrl = "/api/ViewModel";
    return fetch(restUrl, {
      headers: {
        "Accept": "application/json"
      }
    }).then(response => response.json());
  
  }


}