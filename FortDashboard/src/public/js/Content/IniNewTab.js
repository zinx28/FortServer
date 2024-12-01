document.addEventListener("DOMContentLoaded", () => {
  const buttons = document.querySelectorAll("[data-context]");

  if (buttons.length === 0) {
    console.warn("No buttons with [data-context] attribute found.");
    return;
  }

  buttons.forEach((button) => {
    console.log("Button initialized.");
    button.addEventListener("click", (event) => {
      const target = event.currentTarget;
      const context = target.getAttribute("data-context");
      const contentID = target.getAttribute("data-contextId");
      const DataItem = target.getAttribute("data-item");

      if (context) {
        populateModalForEditNewsTab(context, contentID, DataItem);
      } else {
        console.warn("Button clicked without a valid data-context attribute.");
      }
    });
  });

  async function populateModalForEditNewsTab(context, contentID, DataItem) {
    console.log(`Editing context: ${context} with ID: ${contentID}`);
  }
});
