htmx.onLoad(function (content) {

    // SORTABLE
    let sortables = document.querySelectorAll(".sortable");
    console.log(sortables);
    for (let i = 0; i < sortables.length; i++) {
        const sortable = sortables[i];
        new Sortable(sortable, {
            animation: 150,
            group: 'shared',
            ghostClass: 'blue-background-class',

            // Make the `.htmx-indicator` unsortable
            // filter: ".htmx-indicator",
            // onMove: function (evt) {
            //     return evt.related.className.indexOf('htmx-indicator') === -1;
            // }
        });
    }
});