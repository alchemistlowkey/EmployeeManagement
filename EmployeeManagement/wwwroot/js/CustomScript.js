function confirmDelete(uniqueId, isDeleteClicked) {
  var deleteSpan = document.getElementById("deleteSpan_" + uniqueId);
  var confirmDeleteSpan = document.getElementById(
    "confirmDeleteSpan_" + uniqueId,
  );

  if (isDeleteClicked) {
    deleteSpan.style.display = "none";
    confirmDeleteSpan.style.display = "inline";
  } else {
    deleteSpan.style.display = "inline";
    confirmDeleteSpan.style.display = "none";
  }
}
