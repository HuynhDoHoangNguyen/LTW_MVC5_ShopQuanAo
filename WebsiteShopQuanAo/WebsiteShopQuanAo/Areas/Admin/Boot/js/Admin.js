document.addEventListener("DOMContentLoaded", function () {
    const tableBody = document.querySelector("table tbody");
    function fetchPage(pageNumber) {
        tableBody.style.opacity = "0.3";
        tableBody.style.transition = "opacity 0.3s ease";
        fetch(`/Admin/GetProductPage?page=${pageNumber}`)
            .then(response => response.text())
            .then(html => {
                tableBody.innerHTML = html;
                tableBody.style.opacity = "1";
                updatePaginationUI(pageNumber);
            })
            .catch(error => {
                console.error("Lỗi chuyển trang:", error);
                tableBody.style.opacity = "1";
            });
    }
    function updatePaginationUI(currentPage) {
        document.querySelectorAll(".btn-pagination").forEach(btn => {
            const parent = btn.parentElement;
            if (btn.getAttribute("data-page") == currentPage) {
                parent.classList.add("active");
                btn.classList.add("bg-dark", "text-white");
            } else {
                parent.classList.remove("active");
                btn.classList.remove("bg-dark", "text-white");
            }
        });
    }
    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("btn-pagination")) {
            e.preventDefault();
            const page = e.target.getAttribute("data-page");
            if (page > 0) {
                fetchPage(page);
            }
        }
    });
});