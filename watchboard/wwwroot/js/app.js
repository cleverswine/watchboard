htmx.onLoad(function (content) {

    // SORTABLE
    let sortables = document.querySelectorAll(".sortable");
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

document.addEventListener("DOMContentLoaded", function() {
    const element = document.getElementById('searchDrawer')
    element.addEventListener('shown.bs.offcanvas', event => {
        document.getElementById('searchInput').focus();
    });
});

function handleImgChange(evt) {
    const img = document.getElementById("img-" + evt.getAttribute("ds-id"));
    img.setAttribute("src", "/app/tmdb/images" + evt.getAttribute("ds-url"));
    enableItemDetailSubmitBtn();
}

function enableItemDetailSubmitBtn() {
    document.getElementById('itemDetailSubmitBtn').removeAttribute('disabled');
}
