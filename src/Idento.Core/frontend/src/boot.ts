import { bootstrap } from 'angular2/platform/browser'
import { ROUTER_PROVIDERS } from 'angular2/router'
import { Main } from './components/main/main'

declare function require(id: string): string

// Include default stylesheet
require('!style!css?minimize!./default.css')

bootstrap(Main, [ ROUTER_PROVIDERS ])