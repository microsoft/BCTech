import * as vscode from 'vscode';
import { XLIFFTranslator } from './XLIFFTranslator';

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	const translator = new XLIFFTranslator();

	console.log('translatexliff extension activated');

	// Register the translate command
	let disposable = vscode.commands.registerCommand('extension.translatexliff', () => {
		// Display a message box to the user
		translator.translate();
	});

	context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
export function deactivate() {}
