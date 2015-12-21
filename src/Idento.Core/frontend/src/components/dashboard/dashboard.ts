import { Component, View } from 'angular2/core'

declare function require(id: string): string

@Component({
    selector: 'dashboard'
})

@View({
    directives: [],
    template: require('./dashboard.html'),
    styles: [require('./dashboard.css')]
})

export class Dashboard {
    constructor() { }
}