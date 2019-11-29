import * as vscode from 'vscode';
import * as fs from 'fs';
import * as path from 'path';
import * as util from 'util';
import * as xmldom from 'xmldom';
import * as httpm from 'typed-rest-client/HttpClient';
import * as trc from 'typed-rest-client/Interfaces';
import { Logger } from './logger';

export class XLIFFTranslator {

    private static readonly writeFile = util.promisify(fs.writeFile);
    private static readonly readFile = util.promisify(fs.readFile);
    private translationServiceApiKey: string | undefined;
    private readonly logger = new Logger();

    // Translate the XLIFF files in the workspace
    async translate(): Promise<void> {
        this.translationServiceApiKey = await this.getTranslationServiceApiKey();

        if (util.isUndefined(this.translationServiceApiKey)) {
            console.log("Invalid translation service api key");
            vscode.window.showErrorMessage('Invalid translation service api key');
            return;
        }

        var files = await vscode.workspace.findFiles('**/*.xlf');
        if (files.length === 0) {
            this.logger.log('Error: No .xlf files to translate');
            return;
        }

        files.forEach(fileUri => this.translateFile(fileUri));
    }

    // Retrieve the translation service api key
    private async getTranslationServiceApiKey(): Promise<string | undefined> {
        if (!util.isUndefined(this.translationServiceApiKey)) {
            return this.translationServiceApiKey;
        }

        // Try to get the key from the configuration
        const key = vscode.workspace.getConfiguration().get<string>('XLIFFTranslator.apiKey');
        if (!util.isUndefined(key)) {
            return key;
        }

        // Ask the user for the key
        const result = await vscode.window.showInputBox({prompt: 'Enter the Translation Service API key'}        );

        if (!util.isUndefined(result) && result.length === 0) {
            return undefined;
        }

        return result;
    }

    private async translateFile(fileUri: vscode.Uri): Promise<void> {
        var xliffFilePath = fileUri.fsPath;
        this.logger.log('Translating file ' + path.basename(xliffFilePath));

        var xliffBuffer = await XLIFFTranslator.readFile(xliffFilePath);
        var xliffContent = xliffBuffer.toString();
        var doc = new xmldom.DOMParser().parseFromString(xliffContent);

        var fileElements = doc.getElementsByTagName('file');
        if (fileElements.length === 0) {
            this.logger.log('Error: Invalid XLIFF file format');
            return;
        }

        var sourceLanguage = fileElements[0].getAttribute('source-language');
        if (util.isNull(sourceLanguage)) {
            this.logger.log('Error: Missing source language');
            return;
        }

        var targetLanguage = fileElements[0].getAttribute('target-language');
        if (util.isNull(targetLanguage)) {
            this.logger.log('Error: Missing target language');
            return;
        }

        
        this.logger.log('Translating from source language '+ sourceLanguage +'to target language ' + targetLanguage);
        var translatedDocument = await this.translateSources(doc, sourceLanguage,targetLanguage);
        if (util.isUndefined(translatedDocument)) {
            return;
        }

    var xmlSerializer = new xmldom.XMLSerializer();
    var tranlatedDocumentFileContent = xmlSerializer.serializeToString(translatedDocument);

    XLIFFTranslator.writeFile(xliffFilePath, tranlatedDocumentFileContent);
    var xliffFileUri: vscode.Uri = vscode.Uri.file(xliffFilePath);
    vscode.window.showTextDocument(xliffFileUri);
    }

    // Produce a JSON string with he sources to be translated 
    // in the format expected by  the translation service
    private getSourcesAsJson(doc: Document): string {
        var result: string = '[\n';
        const transUnitElements = doc.getElementsByTagName('trans-unit');
        this.logger.log(transUnitElements.length + ' source string(s) to translate');
       
        var separator = '';
        for (var i = 0; i < transUnitElements.length; i++) {
            const source = transUnitElements[i].getElementsByTagName('source')[0].textContent;
            if (source !== null) {
                result += separator + '{\"Text\":\"' + source + '\"}';
                separator = ',\n';
            }
        }

        result += "\n]";
        return result;
    }

    // Call the translation service to build a dictionary with the translated string
    private async translateSources(doc: Document,sourceLanguage: string, targetLanguage: string): Promise<Document | undefined> {

        // Get the sources to be translated
        var sources = this.getSourcesAsJson(doc);
      
        // Issue the REST call to the translation service
        this.logger.log('Invoking the translation service...');
        const url = 'https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=' + sourceLanguage + '&to=' + targetLanguage;
        var headers: trc.IHeaders = {};
        headers['Content-Type'] = 'application/json';
        headers['Ocp-Apim-Subscription-Key'] = this.translationServiceApiKey;
        var client = new httpm.HttpClient('xliffTranslator');
        var response = await client.post(url, sources, headers);
       
    // Handle the response
    var responseBody = await response.readBody();
    var jsonResponse = this.handleResponse(responseBody);
    if (util.isUndefined(jsonResponse)) {
        return;
    }

    const transUnitElements = doc.getElementsByTagName('trans-unit');
    for (var i = 0; i < jsonResponse.length; i++) {
        const translatedText = jsonResponse[i]['translations'][0]['text'];
        console.log(translatedText);

        const transUnitElement = transUnitElements[i];
        var existingTargets = transUnitElement.getElementsByTagName('target');
        var targetElement: Element;

        if (existingTargets.length === 0) {
            targetElement = doc.createElement('target');
            targetElement.textContent = translatedText;
            var sp = doc.createTextNode('  ');
            transUnitElement.appendChild(sp);
            transUnitElement.appendChild(targetElement);
            var cr = doc.createTextNode('\n        ');
            transUnitElement.appendChild(cr);

        } else {
            targetElement = existingTargets[0];
            targetElement.textContent = translatedText;
        }
    }

        this.logger.log(jsonResponse.length + ' strings translated');
        return doc;
    }

    // Parse the reponse body and handle possible errors
    // retuns the parsed JSON response if no errors or undefined otherwise
    private handleResponse(responseBody: string) : any | undefined {
        console.log('Response:\n' + responseBody);

        var jsonResponse: any;
        try {
            jsonResponse = JSON.parse(responseBody);

        } catch (error) {
            console.log('Error parsing JSON response: \n' + responseBody);
            return undefined;
        }

        if (!util.isNullOrUndefined(jsonResponse['error'])) {
            console.log('Error calling the translation service:');
            console.log(jsonResponse['error']['message']);
            vscode.window.showErrorMessage('Error calling the translation service: ' + jsonResponse['error']['message']);
            
            // Error 401000 indicates wrong credentials in which case the api key is most
            // likely invalid. Resetting the api key, so the user gets prompt again for it.
            if (jsonResponse['error']['code'] === 401000) {
                this.translationServiceApiKey = undefined;
            }

            return undefined;
        }

       return jsonResponse;
    }
}