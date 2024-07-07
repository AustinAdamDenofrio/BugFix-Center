import { refreshDataTables } from './utils.js';

export function afterWebStarted(blazor) {
    customInit();

    blazor.addEventListener('enhancedload', ktInit);
}

export function ktInit() {
    KTComponents.init();
    KTAppLayoutBuilder.init();
    KTThemeModeUser.init();
    KTThemeMode.init();
    KTAppSidebar.init();

    customInit();
}

function customInit() {
    const activeAccordion = document.querySelector('.menu-accordion:has(.active)');
    if (activeAccordion) {
        activeAccordion.classList.add('show', 'here');
    }

    refreshDataTables();

    window.dispatchEvent(new Event('resize'));
}