import { Component, View } from 'angular2/core'

declare function require(id: string): string

@Component({
    selector: 'login-provider'
})

@View({
    directives: [],
    template: require('./login-provider.html'),
    styles: [require('./login-provider.css')]
})

export class LoginProvider {
    constructor() { }
}