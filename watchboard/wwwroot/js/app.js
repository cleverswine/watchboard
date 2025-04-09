htmx.onLoad(function (content) {

    // SORTABLE
    let sortables = document.querySelectorAll(".sortable");
    for (let i = 0; i < sortables.length; i++) {
        const sortable = sortables[i];
        new Sortable(sortable, {
            animation: 150,
            group: 'shared',
            ghostClass: 'blue-background-class'
        });
    }
});

document.addEventListener("DOMContentLoaded", function() {
    const element = document.getElementById('searchDrawer')
    element.addEventListener('shown.bs.offcanvas', event => {
        document.getElementById('searchInput').focus();
    });
});

function enableItemDetailSubmitBtn() {
    document.getElementById('itemDetailSubmitBtn').removeAttribute('disabled');
}
