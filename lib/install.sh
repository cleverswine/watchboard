#!/usr/bin/env bash

set -e

# run from the ./lib directory

npm ci
rm -rf ../watchboard/wwwroot/lib/*
cp ./node_modules/bootstrap/dist/css/bootstrap.min.css ../watchboard/wwwroot/lib/
cp ./node_modules/bootstrap/dist/js/bootstrap.bundle.min.js ../watchboard/wwwroot/lib/
cp ./node_modules/bootstrap-icons/font/bootstrap-icons.min.css ../watchboard/wwwroot/lib/
cp -r ./node_modules/bootstrap-icons/font/fonts ../watchboard/wwwroot/lib/
cp ./node_modules/htmx.org/dist/htmx.min.js ../watchboard/wwwroot/lib/       
cp ./node_modules/sortablejs/Sortable.min.js ../watchboard/wwwroot/lib/