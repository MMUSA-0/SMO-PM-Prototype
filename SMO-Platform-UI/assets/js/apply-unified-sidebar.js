/**
 * Apply Unified Sidebar Script
 * This script helps implement the unified sidebar across all pages
 * Run this on each page to replace the existing sidebar with the unified version
 */

(function() {
    'use strict';

    /**
     * Configuration for unified sidebar implementation
     */
    const config = {
        sidebarComponentPath: 'assets/components/sidebar.html',
        sidebarManagerScript: 'assets/js/sidebar-manager.js',
        sidebarStyles: 'assets/css/sidebar-unified.css',
        replaceExisting: true,
        addContainer: true
    };

    /**
     * Apply unified sidebar to current page
     */
    function applyUnifiedSidebar() {
        console.log('Applying unified sidebar to current page...');
        
        // 1. Check if sidebar manager is already loaded
        if (window.sidebarManager) {
            console.log('Sidebar Manager already initialized.');
            return;
        }
        
        // 2. Add sidebar styles if not present
        if (!document.querySelector(`link[href*="${config.sidebarStyles}"]`)) {
            const styleLink = document.createElement('link');
            styleLink.rel = 'stylesheet';
            styleLink.href = config.sidebarStyles;
            document.head.appendChild(styleLink);
            console.log('Added unified sidebar styles.');
        }
        
        // 3. Find existing sidebar or create container
        let sidebarContainer = document.getElementById('sidebarContainer');
        const existingSidebar = document.querySelector('.page-sidebar');
        
        if (!sidebarContainer) {
            if (existingSidebar && config.replaceExisting) {
                // Create container and replace existing sidebar
                sidebarContainer = document.createElement('div');
                sidebarContainer.id = 'sidebarContainer';
                sidebarContainer.className = 'sidebar-container';
                
                // Add loading state
                sidebarContainer.classList.add('loading');
                sidebarContainer.innerHTML = `
                    <div class="sidebar-skeleton">
                        <div class="sidebar-skeleton-item"></div>
                        <div class="sidebar-skeleton-item"></div>
                        <div class="sidebar-skeleton-item"></div>
                        <div class="sidebar-skeleton-item"></div>
                    </div>
                `;
                
                existingSidebar.parentNode.replaceChild(sidebarContainer, existingSidebar);
                console.log('Replaced existing sidebar with container.');
            } else if (config.addContainer) {
                // Add container to main element if no sidebar exists
                const mainElement = document.querySelector('main');
                if (mainElement) {
                    sidebarContainer = document.createElement('div');
                    sidebarContainer.id = 'sidebarContainer';
                    sidebarContainer.className = 'sidebar-container loading';
                    mainElement.insertBefore(sidebarContainer, mainElement.firstChild);
                    console.log('Added sidebar container to main element.');
                }
            }
        }
        
        // 4. Load sidebar manager script
        if (!document.querySelector(`script[src*="${config.sidebarManagerScript}"]`)) {
            const script = document.createElement('script');
            script.src = config.sidebarManagerScript;
            script.onload = () => {
                console.log('Sidebar Manager script loaded successfully.');
                // Remove loading state
                if (sidebarContainer) {
                    setTimeout(() => {
                        sidebarContainer.classList.remove('loading');
                    }, 500);
                }
            };
            script.onerror = () => {
                console.error('Failed to load Sidebar Manager script.');
            };
            document.body.appendChild(script);
        }
    }

    /**
     * Update page references in existing HTML
     */
    function updatePageReferences() {
        // Update all internal links to ensure consistency
        const internalLinks = document.querySelectorAll('a[href*=".html"]');
        internalLinks.forEach(link => {
            // Ensure links are relative
            const href = link.getAttribute('href');
            if (href && href.startsWith('/')) {
                link.setAttribute('href', href.substring(1));
            }
        });
        
        console.log('Updated page references.');
    }

    /**
     * Add keyboard shortcuts info
     */
    function addKeyboardShortcutsInfo() {
        // Check if shortcuts info already exists
        if (document.querySelector('.keyboard-shortcuts-info')) {
            return;
        }
        
        // Create shortcuts tooltip
        const shortcutsInfo = document.createElement('div');
        shortcutsInfo.className = 'keyboard-shortcuts-info';
        shortcutsInfo.innerHTML = `
            <div class="shortcuts-tooltip" style="
                position: fixed;
                bottom: 20px;
                left: 20px;
                background: rgba(0, 0, 0, 0.8);
                color: white;
                padding: 10px 15px;
                border-radius: 5px;
                font-size: 12px;
                z-index: 1000;
                display: none;
            ">
                <strong>اختصارات لوحة المفاتيح:</strong><br>
                Ctrl+B - تبديل الشريط الجانبي<br>
                Ctrl+K - البحث في القائمة
            </div>
        `;
        
        document.body.appendChild(shortcutsInfo);
        
        // Show tooltip on first visit
        const hasSeenShortcuts = localStorage.getItem('hasSeenShortcuts');
        if (!hasSeenShortcuts) {
            const tooltip = shortcutsInfo.querySelector('.shortcuts-tooltip');
            tooltip.style.display = 'block';
            setTimeout(() => {
                tooltip.style.display = 'none';
                localStorage.setItem('hasSeenShortcuts', 'true');
            }, 5000);
        }
    }

    /**
     * Initialize unified sidebar implementation
     */
    function init() {
        // Wait for DOM to be fully loaded
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', init);
            return;
        }
        
        console.log('Initializing unified sidebar implementation...');
        
        // Apply unified sidebar
        applyUnifiedSidebar();
        
        // Update page references
        updatePageReferences();
        
        // Add keyboard shortcuts info
        addKeyboardShortcutsInfo();
        
        // Set up automatic page detection
        setupPageDetection();
        
        console.log('Unified sidebar implementation complete.');
    }

    /**
     * Setup automatic page detection for active states
     */
    function setupPageDetection() {
        // Detect current page from URL
        const currentPage = window.location.pathname.split('/').pop().replace('.html', '') || 'index';
        
        // Store in data attribute for sidebar manager
        document.body.setAttribute('data-current-page', currentPage);
        
        console.log(`Current page detected: ${currentPage}`);
    }

    /**
     * Public API for manual implementation
     */
    window.UnifiedSidebarImplementation = {
        apply: applyUnifiedSidebar,
        updateReferences: updatePageReferences,
        init: init,
        config: config
    };

    // Auto-initialize
    init();

})();

/**
 * Helper function to update all pages at once (for development)
 * Run this in console on each page: UnifiedSidebarImplementation.init()
 */
console.info(`
╔════════════════════════════════════════════╗
║     Unified Sidebar Implementation Ready   ║
╠════════════════════════════════════════════╣
║ To apply unified sidebar to this page:     ║
║ > UnifiedSidebarImplementation.init()      ║
║                                            ║
║ Keyboard Shortcuts:                       ║
║ • Ctrl+B - Toggle sidebar                 ║
║ • Ctrl+K - Focus search                   ║
╚════════════════════════════════════════════╝
`);

