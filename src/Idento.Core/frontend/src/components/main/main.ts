import { Component, View } from 'angular2/core'
import { RouteConfig, ROUTER_DIRECTIVES } from 'angular2/router'

import { Dashboard } from '../dashboard/dashboard'
import { Application } from '../application/application'
import { LoginProvider } from '../login-provider/login-provider'

declare function require(id: string): string

@Component({
    selector: 'main'
})

@View({
    directives: [ ROUTER_DIRECTIVES ],
    template: require('./main.html'),
    styles: [ require('./main.css') ]
})

@RouteConfig([
    { path: '/', redirectTo: [ 'Dashboard' ] },
    { path: '/dashboard', name: 'Dashboard', component: Dashboard },
    { path: '/app', name: 'App', component: Application },
    { path: '/provider', name: 'Provider', component: LoginProvider }
])

export class Main {
    title: string;
    constructor() {
        this.title = 'Main title'
        console.log(LoginProvider)
    }
}
