document.addEventListener('DOMContentLoaded', function () {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    const rootElement = document.documentElement;
    const bodyElement = document.body;
    const pageDirection = rootElement.dir;
    const pageLoader = document.querySelector('.page-preloader');
    const pageHeader = document.querySelector('.page-header');
    const searchMenu = document.querySelector('.search-menu');
    const searchToggleBtns = document.querySelectorAll('.search-toggle-btn');
    const grayscaleToggleBtn = document.querySelector('.grayscale-toggle-btn');
    const pageSidebar = document.querySelector('.page-sidebar');
    const sidebarToggleBtn = document.querySelector('.page-sidebar-toggle-btn');
    const subMenuToggles = document.querySelectorAll('.page__nav__link');
    const tables = document.querySelectorAll('.table');
    const tabs = document.querySelector('.nav-tabs');
    const copyrightsYear = document.querySelector('.copyrights__year');


    function closePageLoader() {
        bodyElement.classList.contains('overflow-hidden') ? bodyElement.classList.remove('overflow-hidden') : null;
        pageLoader.classList.add('page-loaded');
    }

    function stickHeader() {
        let scrollpos = window.scrollY;

        if (scrollpos > 60) {
            pageHeader.classList.add('page-header--sticky');
        } else {
            pageHeader.classList.remove('page-header--sticky');
        }
    }

    function toggleSidebar(e) {
        e.preventDefault();
        bodyElement.classList.toggle('page-sidebar-shown');
    }

    function closeSidebar(e) {
        if (!pageSidebar.contains(e.target) && (!sidebarToggleBtn.contains(e.target))) {
            bodyElement.classList.remove('page-sidebar-shown');
        }
    }

    function toggleSubMenu(e) {
        e.preventDefault();

        const currentSubMenu = this.parentElement;

        this.classList.toggle('active');

        if (currentSubMenu.classList.contains('submenu-shown')) {
            currentSubMenu.classList.remove('submenu-shown');
        } else {
            this.closest('.page__nav__list').querySelectorAll('.submenu-shown').forEach(item => item.classList.remove('submenu-shown'));
            currentSubMenu.classList.add('submenu-shown');
        }
    }

    function toggleSearchInput(e) {
        e.preventDefault();
        e.stopPropagation();

        this.parentElement.classList.toggle('active');
        if (this.parentElement.classList.contains('active')) {
            this.parentElement.querySelector('.form-control').focus();
        }
    }

    function closeSearchMenu(e) {
        if (!searchMenu.contains(e.target)) {
            searchMenu.classList.remove('active');
        }
    }

    function updateYopyrightsYear() {
        copyrightsYear.innerHTML = new Date().getFullYear()
    }

    function toggleGrayscale() {
        bodyElement.classList.toggle('grayscale-active');

        if (bodyElement.classList.contains('grayscale-active')) {
            localStorage.setItem('grayscaleHRDFIntra', 'active');
        } else {
            localStorage.setItem('grayscaleHRDFIntra', 'disabled');
        }
    }

    // Function to apply the grayscale setting from local storage
    function applyGrayscaleSetting() {
        const grayscaleSetting = localStorage.getItem('grayscaleHRDFIntra');
        if (grayscaleSetting === 'active') {
            bodyElement.classList.add('grayscale-active');
        } else {
            bodyElement.classList.remove('grayscale-active');
        }
    }

    tables.forEach(table => {
        var headertext = [],
            tableHeaders = table.querySelectorAll('thead th, thead td'),
            tableBody = table.querySelector('tbody');

        for (var i = 0; i < tableHeaders.length; i++) {
            var current = tableHeaders[i];
            let headerText = current.textContent.replace(/\r?\n|\r/, '');

            // Check if there is a element with class "cell-data" inside the current header cell
            let cellDataElement = current.querySelector('.cell-data');
            if (cellDataElement) {
                headertext.push(cellDataElement.textContent.trim());
            } else {
                headertext.push(headerText.trim());
            }
        }

        if (tableBody !== null) {
            for (var i = 0, row; row = tableBody.rows[i]; i++) {
                for (var j = 0, col; col = row.cells[j]; j++) {
                    if (headertext[j] != undefined) {
                        col.setAttribute('data-th', headertext[j]);
                    }
                }
            }
        }
    });

    applyGrayscaleSetting();
    pageLoader ? window.addEventListener('load', closePageLoader) : '';
    searchToggleBtns ? searchToggleBtns.forEach(btn => btn.addEventListener('click', toggleSearchInput)) : '';
    searchMenu ? document.addEventListener('click', closeSearchMenu) : '';
    sidebarToggleBtn ? sidebarToggleBtn.addEventListener('click', toggleSidebar) : '';
    pageSidebar ? document.addEventListener('click', closeSidebar) : '';
    subMenuToggles ? subMenuToggles.forEach(toggle => toggle.addEventListener('click', toggleSubMenu)) : '';
    grayscaleToggleBtn ? grayscaleToggleBtn.addEventListener('click', toggleGrayscale) : '';
    window.addEventListener('scroll', stickHeader);
    window.addEventListener('load', stickHeader);
    copyrightsYear ? window.addEventListener('load', updateYopyrightsYear) : '';

    // Horizontal scroll with mouse wheel
    tabs.addEventListener('wheel', function (e) {
        if (e.deltaY !== 0) {
            e.preventDefault();
            tabs.scrollLeft -= e.deltaY;
        }
    });

    // Drag to scroll
    let isDown = false;
    let startX;
    let scrollLeft;

    tabs.addEventListener('mousedown', (e) => {
        isDown = true;
        tabs.classList.add('dragging');
        startX = e.pageX - tabs.offsetLeft;
        scrollLeft = tabs.scrollLeft;
    });

    tabs.addEventListener('mouseleave', () => {
        isDown = false;
        tabs.classList.remove('dragging');
    });

    tabs.addEventListener('mouseup', () => {
        isDown = false;
        tabs.classList.remove('dragging');
    });

    tabs.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault();
        const x = e.pageX - tabs.offsetLeft;
        const walk = (x - startX) * 1; // scroll speed multiplier
        tabs.scrollLeft = scrollLeft - walk;
    });




    // Sliders

    const latestAchievementsMarquee = document.querySelector('.latest-achievements__marquee');

    if (latestAchievementsMarquee) {
        const marqueeSwiperWrapper = latestAchievementsMarquee.querySelector('.swiper-wrapper');
        const originalCount = marqueeSwiperWrapper.children.length;

        if (originalCount <= 5) {
            let i = 0;
            while (marqueeSwiperWrapper.children.length < 6) {
                marqueeSwiperWrapper.appendChild(marqueeSwiperWrapper.children[i].cloneNode(true));
                i = (i + 1) % originalCount;
            }
        }

        const marqueeSwiper = new Swiper('.latest-achievements__marquee', {
            slidesPerView: 'auto',
            spaceBetween: 40,
            loop: true,
            allowTouchMove: false,
            speed: 2000,
            autoplay: {
                delay: 0,
                disableOnInteraction: false,
                pauseOnMouseEnter: true
            },
            loopedSlides: marqueeSwiperWrapper.children.length,
        });

        latestAchievementsMarquee.addEventListener('mouseenter', () => {
            marqueeSwiper.autoplay.pause();
        });

        latestAchievementsMarquee.addEventListener('mouseleave', () => {
            marqueeSwiper.autoplay.resume();
        });
    }
});
