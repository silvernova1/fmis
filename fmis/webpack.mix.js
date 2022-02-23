const mix = require('laravel-mix');

mix.js('VueSrc/app.js', 'wwwroot/vue/dist').vue()
    .sass('VueSrc/app.scss', 'wwwroot/vue/dist');