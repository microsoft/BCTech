import * as vscode from 'vscode';

// Logger utility class
export class Logger {
    private readonly outputChannel : vscode.OutputChannel;

    constructor() {
        this.outputChannel = vscode.window.createOutputChannel('XLIFF translator');
    }

    // log to the debub console and the output channel
    public log(text: string) {
        console.log(text);
        this.outputChannel.appendLine(text);
    }
}