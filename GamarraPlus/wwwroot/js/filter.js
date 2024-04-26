document.addEventListener("DOMContentLoaded", function() {
  const filterButtons = document.querySelectorAll(".filter-btn");
  const productItems = document.querySelectorAll(".product-item");

  filterButtons.forEach(function(button) {
    button.addEventListener("click", function() {
      // Remueve la clase "active" de todos los botones
      filterButtons.forEach(function(btn) {
        btn.classList.remove("active");
      });
      
      // Agrega la clase "active" solo al botón clickeado
      this.classList.add("active");
      
      const filter = this.getAttribute("data-filter");
      filterItems(filter);
    });
  });

  function filterItems(filter) {
    productItems.forEach(function(item) {
      item.style.display = "none";
      if (item.classList.contains(filter) || filter === "todos") {
        item.style.display = "block";
      }
    });
  }
});
