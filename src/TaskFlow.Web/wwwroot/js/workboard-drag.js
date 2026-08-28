(() => {
    const cardSelector = "[data-workboard-card]";
    const columnSelector = "[data-kanban-column]";

    const closest = (target, selector) =>
        target instanceof Element ? target.closest(selector) : null;

    const clearDragState = () => {
        document.querySelectorAll(`${cardSelector}.is-dragging`).forEach(card =>
            card.classList.remove("is-dragging"));
        document.querySelectorAll(`${columnSelector}.is-drag-over`).forEach(column =>
            column.classList.remove("is-drag-over"));
    };

    document.addEventListener("dragstart", event => {
        const card = closest(event.target, cardSelector);
        if (!card) {
            return;
        }

        if (card.getAttribute("aria-disabled") === "true" || !event.dataTransfer) {
            event.preventDefault();
            return;
        }

        event.dataTransfer.effectAllowed = "move";
        event.dataTransfer.setData("text/plain", card.dataset.workboardId ?? "");
        card.classList.add("is-dragging");
    });

    document.addEventListener("dragover", event => {
        const column = closest(event.target, columnSelector);
        if (!column || !event.dataTransfer) {
            return;
        }

        event.preventDefault();
        event.dataTransfer.dropEffect = "move";
        document.querySelectorAll(`${columnSelector}.is-drag-over`).forEach(otherColumn => {
            if (otherColumn !== column) {
                otherColumn.classList.remove("is-drag-over");
            }
        });
        column.classList.add("is-drag-over");
    });

    document.addEventListener("dragleave", event => {
        const column = closest(event.target, columnSelector);
        if (!column || (event.relatedTarget instanceof Node && column.contains(event.relatedTarget))) {
            return;
        }

        column.classList.remove("is-drag-over");
    });

    document.addEventListener("drop", clearDragState);
    document.addEventListener("dragend", clearDragState);
})();
