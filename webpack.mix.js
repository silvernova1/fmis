const mix = require('laravel-mix');

mix.webpackConfig({
    resolve: {
        alias: {
            '@': __dirname + '/VueSrc'
        },
    },
})

mix.ts('vuesrc/app.ts', 'wwwroot/vue/dist').vue()
    .sass('vuesrc/app.scss', 'wwwroot/vue/dist');