async function addNewTag() {
    const response = await fetch("/Books/AddNewTagPartial");
    const message = await response.text();
    document
        .getElementById("tag-inputs")
        .insertAdjacentHTML("beforeend", message);
}
