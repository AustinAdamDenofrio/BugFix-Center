export function afterWebStarted(blazor) {
    showActiveAccordion();
    window.dispatchEvent(new Event('resize'));

    blazor.addEventListener('enhancedload', () => {
        KTComponents.init();
        KTAppLayoutBuilder.init();
        KTThemeModeUser.init();
        KTThemeMode.init();
        KTAppSidebar.init();

        showActiveAccordion();
        window.dispatchEvent(new Event('resize'));
    });
}

function showActiveAccordion() {
    const activeAccordion = document.querySelector('.menu-accordion:has(.active)');
    if (activeAccordion) {
        activeAccordion.classList.add('show', 'here');
    }
}