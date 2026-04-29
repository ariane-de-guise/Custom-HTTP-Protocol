document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("form, li").forEach((el, i) => {
        setTimeout(() => {
            el.classList.add("fade-in");
        }, i * 75);
    });

    const deleteForm = document.getElementById("deleteForm");

    if (deleteForm) {
        deleteForm.addEventListener("submit", e => {
            if (!confirm("Are you sure you want to delete this message?")) {
                e.preventDefault();
            }
        });
    }
});