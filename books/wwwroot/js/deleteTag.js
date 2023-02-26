
async function deleteTag(e) {
    console.log(e);
    console.log(e.target);
    const tagInputContainer = e.target.closest('.tag-input-container');
    tagInputContainer.remove();
}