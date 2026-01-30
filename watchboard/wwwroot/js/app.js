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
    const myModal = document.getElementById('search-modals-here');
    myModal.addEventListener('shown.bs.modal', function () {
        document.getElementById('searchInput').focus();
    });
});