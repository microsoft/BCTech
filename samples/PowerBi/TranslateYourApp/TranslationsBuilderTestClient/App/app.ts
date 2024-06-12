
import * as $ from 'jquery';

import * as powerbi from "powerbi-client";
import * as pbimodels from "powerbi-models";

require('powerbi-models');
require('powerbi-client');

import { ViewModel, Language } from './models/models';
import { WebApi } from './services/webapi'

export default class App {

  private static languagesList: JQuery;
  private static embedContainer: JQuery;

  private static viewModel: ViewModel;
  private static powerbi: powerbi.service.Service = window.powerbi;

  private static currentPage: string = "";
  private static currentLanguage: string = "";

  private static startPage: number;
  private static startLanguage: string;

  public static onDocumentReady = () => {

    // get query string parameter for start page
    var urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('page')) {
      let page = urlParams.get('page');
      let pageNumber = Number(page);
      if (!isNaN(pageNumber)) {
        App.startPage = pageNumber;
      }
    }

    // get query string parameter for start language
    if (urlParams.has('language')) {
      let language = urlParams.get('language');
      App.startLanguage = language;
    }

    // remove query string parameters from URL
    if (window.location.href.indexOf('?') > -1) {
      history.pushState('', document.title, window.location.pathname);
    }

    App.languagesList = $('#language-list');
    App.embedContainer = $('#embed-container');
    App.initializeApplication();
    App.setEmbedContainerHeight();
    $(window).resize(function () {
      App.setEmbedContainerHeight();
    });
  }

  public static initializeApplication = async () => {
    this.viewModel = await WebApi.GetViewModel();
    let languages: Language[] = App.viewModel.langauges;
    App.viewModel.langauges.forEach((language: Language) => {
      var li = $("<li>")
        .text(language.localizedName)
        .click(() => {
          App.embedReport(language);
        });
      App.languagesList.append(li);
    });

    let startupLangauge: Language;

    // get startup language from query string
    if (App.startLanguage != undefined) {
      startupLangauge = App.viewModel.langauges.find((language) => language.languageTag == App.startLanguage);
      App.startLanguage = undefined;
    }

    // set default language if not passed in query string
    if (startupLangauge == undefined) {
      startupLangauge = languages[0];
    }


    App.embedReport(startupLangauge);
  };

  public static embedReport = async (language: Language) => {

    App.currentLanguage = language.localizedName;

    App.updateSelectedLanguageInLeftNav();

    var models = pbimodels;
    var config: powerbi.IReportEmbedConfiguration = {
      type: 'report',
      id: this.viewModel.reportId,
      embedUrl: this.viewModel.embedUrl,
      accessToken: this.viewModel.token,
      tokenType: models.TokenType.Embed,
      permissions: models.Permissions.Read,
      viewMode: models.ViewMode.View,
      settings: {
        background: pbimodels.BackgroundType.Transparent,
        localeSettings: { language: language.languageTag },
        panes: {
          filters: { visible: false },
          pageNavigation: { visible: false }
        }
      }
    };

    if (App.currentPage != "") {
      config.pageName = App.currentPage;
    }

    App.powerbi.reset(App.embedContainer[0]);
    let report = <powerbi.Report>App.powerbi.embed(App.embedContainer[0], config);
    report.off("loaded")
    report.on("loaded", async (event: any) => {

      if (App.startPage != undefined) {
        let pages = await report.getPages();
        if (App.startPage <= pages.length) {
          report.setPage(pages[App.startPage - 1].name);
          App.startPage = undefined;
        }
      }

      const filters = [{
        $schema: "http://powerbi.com/product/schema#basic",
        target: {
          table: "Languages",
          column: "LanguageTag"
        },
        operator: "In",
        values: [language.languageTag],
        filterType: models.FilterType.Basic,
        requireSingleSelection: true
      }];
      await report.updateFilters(models.FiltersOperations.Replace, filters);

    });

    report.off("pageChanged")
    report.on("pageChanged", async (event: any) => {
      App.currentPage = event.detail.newPage.name;
    });

  }

  public static updateSelectedLanguageInLeftNav() {

    console.log("updateSelectedLanguageInLeftNav");

    $("#language-list li").each((index: number, element: HTMLElement): false | void => {
      console.log();
      if (element.textContent == App.currentLanguage) {
        element.style.backgroundColor = "#F0E199"
      }
      else {
        element.style.backgroundColor = "#FFFFFF";
      }
    });
  }

  private static setEmbedContainerHeight = async () => {
    App.embedContainer.height(($(window).height() - 6));
  }
}

$(App.onDocumentReady);