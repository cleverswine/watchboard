htmx.onLoad(function (content) {

    // SORTABLE
    let sortables = document.querySelectorAll(".sortable");
    for (let i = 0; i < sortables.length; i++) {
        const sortable = sortables[i];
        new Sortable(sortable, {
            animation: 150,
            group: 'shared',
            ghostClass: 'blue-background-class',
            draggable: '.list-item',
            handle: '.drag-handle'
        });
    }
});

function clearSearch() {
    const i = document.getElementById('searchInput');
    i.value = "";
    i.focus();
    i.click();
}

document.addEventListener("DOMContentLoaded", function () {
    // Find the button that opens the modal
    const modalTrigger = document.querySelector('[data-bs-target="#search-modals-here"]');
    if (modalTrigger) {
        modalTrigger.addEventListener('click', function () {
            setTimeout(function () {
                document.getElementById('searchInput').focus();
            }, 500); // Wait for modal animation
        });
    }
});
