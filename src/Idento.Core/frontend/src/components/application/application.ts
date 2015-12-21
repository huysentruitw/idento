import { Component, View } from 'angular2/core'

declare function require(id: string): string

@Component({
    selector: 'application'
})

@View({
    directives: [],
    template: require('./application.html'),
    styles: [require('./application.css')]
})

export class Application {
    constructor() {
    }
}