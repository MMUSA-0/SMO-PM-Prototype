/**
 * Sidebar Manager Module
 * Handles dynamic loading and management of the unified sidebar component
 * @module SidebarManager
 */

class SidebarManager {
    constructor() {
        this.sidebarUrl = 'assets/components/sidebar.html';
        this.currentPage = this.getCurrentPage();
        this.sidebarContainer = null;
        this.userPreferences = this.loadUserPreferences();
    }

    /**
     * Initialize the sidebar manager
     */
    async init() {
        try {
            // Load sidebar HTML
            await this.loadSidebar();
            
            // Inject cross-module styles
            this.injectCrossModuleStyles();
            
            // Set active states
            this.setActiveStates();
            
            // Initialize event handlers
            this.initEventHandlers();
            
            // Restore user preferences
            this.restoreUserPreferences();
            
            // Initialize submenu states
            this.initSubmenus();
            
            // Setup cross-module navigation
            this.setupCrossModuleNavigation();
            
            console.log('Sidebar Manager initialized successfully');
        } catch (error) {
            console.error('Error initializing sidebar:', error);
        }
    }
    
    /**
     * Inject styles for cross-module linking
     */
    injectCrossModuleStyles() {
        if (!document.getElementById('cross-module-styles')) {
            const styles = document.createElement('style');
            styles.id = 'cross-module-styles';
            styles.textContent = `
                /* Cross-module linking styles */
                .page__nav__submenu__item.linked-active {
                    background: linear-gradient(90deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%);
                    border-right: 3px solid #667eea;
                    position: relative;
                }
                
                .page__nav__submenu__item.linked-active::after {
                    content: '🔗';
                    position: absolute;
                    left: 10px;
                    top: 50%;
                    transform: translateY(-50%);
                    font-size: 12px;
                    opacity: 0.7;
                }
                
                .page__nav__submenu__item[data-linked] {
                    position: relative;
                }
                
                .page__nav__submenu__item[data-linked="vision"]::before {
                    content: '👁️';
                    margin-left: 5px;
                    font-size: 14px;
                    opacity: 0.6;
                }
                
                .page__nav__submenu__item[data-linked="performance"]::before {
                    content: '📊';
                    margin-left: 5px;
                    font-size: 14px;
                    opacity: 0.6;
                }
                
                /* Quick navigation badges */
                .quick-nav-badge {
                    display: inline-block;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    font-size: 10px;
                    padding: 2px 6px;
                    border-radius: 10px;
                    margin-right: 5px;
                }
                
                /* Hover effect for linked items */
                .page__nav__submenu__item[data-linked]:hover {
                    background: linear-gradient(90deg, rgba(102, 126, 234, 0.05) 0%, rgba(118, 75, 162, 0.05) 100%);
                    transform: translateX(-2px);
                    transition: all 0.3s ease;
                }
                
                /* Animation for newly highlighted linked items */
                @keyframes linkHighlight {
                    0% { background-color: rgba(102, 126, 234, 0.3); }
                    100% { background-color: transparent; }
                }
                
                .page__nav__submenu__item.highlight-animation {
                    animation: linkHighlight 1s ease;
                }
            `;
            document.head.appendChild(styles);
        }
    }
    
    /**
     * Setup cross-module navigation features
     */
    setupCrossModuleNavigation() {
        // Add tooltips to linked items
        const linkedItems = document.querySelectorAll('[data-linked]');
        linkedItems.forEach(item => {
            const linkedType = item.getAttribute('data-linked');
            const tooltipText = linkedType === 'vision' 
                ? 'مرتبط برؤية 2030' 
                : 'مرتبط بوحدة الأداء';
            
            item.setAttribute('title', tooltipText);
            
            // Add click handler for smooth navigation
            item.addEventListener('click', (e) => {
                // Store navigation source
                sessionStorage.setItem('navigationSource', linkedType);
                
                // Add visual feedback
                item.classList.add('highlight-animation');
            });
        });
        
        // Check if coming from linked module
        const navigationSource = sessionStorage.getItem('navigationSource');
        if (navigationSource) {
            this.showNavigationFeedback(navigationSource);
            sessionStorage.removeItem('navigationSource');
        }
    }
    
    /**
     * Show navigation feedback when coming from linked module
     */
    showNavigationFeedback(source) {
        const message = source === 'vision' 
            ? 'تم الانتقال من رؤية 2030' 
            : 'تم الانتقال من وحدة الأداء';
        
        // Create feedback toast
        const toast = document.createElement('div');
        toast.className = 'navigation-feedback-toast';
        toast.innerHTML = `
            <div style="
                position: fixed;
                top: 20px;
                left: 50%;
                transform: translateX(-50%);
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                padding: 12px 24px;
                border-radius: 25px;
                font-size: 14px;
                z-index: 9999;
                box-shadow: 0 4px 15px rgba(0,0,0,0.2);
                animation: slideDown 0.5s ease;
            ">
                ${message} 🔗
            </div>
        `;
        
        document.body.appendChild(toast);
        
        // Remove after 3 seconds
        setTimeout(() => {
            toast.remove();
        }, 3000);
    }

    /**
     * Load the sidebar HTML from the template file
     */
    async loadSidebar() {
        try {
            const response = await fetch(this.sidebarUrl);
            if (!response.ok) {
                throw new Error(`Failed to load sidebar: ${response.statusText}`);
            }
            
            const sidebarHtml = await response.text();
            
            // Find or create sidebar container
            this.sidebarContainer = document.getElementById('sidebarContainer');
            if (!this.sidebarContainer) {
                // If no container exists, check for existing sidebar and replace it
                const existingSidebar = document.querySelector('.page-sidebar');
                if (existingSidebar) {
                    const container = document.createElement('div');
                    container.id = 'sidebarContainer';
                    existingSidebar.parentNode.replaceChild(container, existingSidebar);
                    this.sidebarContainer = container;
                }
            }
            
            if (this.sidebarContainer) {
                this.sidebarContainer.innerHTML = sidebarHtml;
            }
        } catch (error) {
            console.error('Error loading sidebar:', error);
            // Fallback: Keep existing sidebar if loading fails
        }
    }

    /**
     * Get the current page name from URL
     */
    getCurrentPage() {
        const path = window.location.pathname;
        const page = path.substring(path.lastIndexOf('/') + 1);
        return page.replace('.html', '') || 'index';
    }

    /**
     * Set active states for current page
     */
    setActiveStates() {
        // Remove all active states
        const allLinks = document.querySelectorAll('.page__nav__link, .page__nav__submenu__item');
        allLinks.forEach(link => link.classList.remove('active', 'linked-active'));
        
        // Find and activate current page link
        const currentPageLinks = document.querySelectorAll(`[data-page="${this.currentPage}"]`);
        currentPageLinks.forEach(link => {
            link.classList.add('active');
            
            // If it's a submenu item, expand parent submenu
            const submenuContainer = link.closest('.page__nav__submenu-container');
            if (submenuContainer) {
                submenuContainer.classList.add('show');
                const parentItem = submenuContainer.closest('.page__nav__list__item');
                if (parentItem) {
                    parentItem.classList.add('submenu-shown');
                }
            }
            
            // Highlight linked items in other modules
            this.highlightLinkedItems(link);
        });
    }
    
    /**
     * Highlight linked items across modules
     */
    highlightLinkedItems(activeLink) {
        const linkedType = activeLink.getAttribute('data-linked');
        if (linkedType) {
            // Find all items with matching linked type
            const linkedItems = document.querySelectorAll(`[data-linked="${linkedType}"]`);
            linkedItems.forEach(item => {
                if (item !== activeLink) {
                    item.classList.add('linked-active');
                }
            });
        }
    }

    /**
     * Initialize event handlers
     */
    initEventHandlers() {
        // Toggle sidebar
        const toggleBtn = document.querySelector('.page-sidebar-toggle-btn');
        if (toggleBtn) {
            toggleBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleSidebar();
            });
        }
        
        // Handle submenu toggles and navigation
        const submenuToggles = document.querySelectorAll('[data-has-submenu="true"] > .page__nav__link');
        submenuToggles.forEach(toggle => {
            toggle.addEventListener('click', (e) => {
                const href = toggle.getAttribute('href');
                const hasValidHref = href && href !== '#' && href !== '';
                const isArrowClick = e.target.closest('.submenu-arrow');
                
                if (isArrowClick) {
                    // Arrow clicked - just toggle submenu
                    e.preventDefault();
                    this.toggleSubmenu(toggle);
                } else if (hasValidHref) {
                    // Has valid href - navigate to page AND toggle submenu
                    this.toggleSubmenu(toggle);
                    // Let the default navigation happen
                } else {
                    // No valid href - just toggle submenu
                    e.preventDefault();
                    this.toggleSubmenu(toggle);
                }
            });
        });
        
        // Handle development indicators
        this.handleDevelopmentLinks();
        
        // Prevent active state persistence
        this.preventActiveStatePersistence();
        
        // Search functionality
        this.initSearchFunctionality();
        
        // Keyboard shortcuts
        this.initKeyboardShortcuts();
    }

    /**
     * Toggle sidebar visibility
     */
    toggleSidebar() {
        const sidebar = document.querySelector('.page-sidebar');
        const body = document.body;
        
        if (sidebar) {
            sidebar.classList.toggle('collapsed');
            body.classList.toggle('sidebar-collapsed');
            
            // Save preference
            this.saveUserPreference('sidebarCollapsed', sidebar.classList.contains('collapsed'));
        }
    }

    /**
     * Toggle submenu visibility
     */
    toggleSubmenu(toggleElement) {
        const targetId = toggleElement.getAttribute('data-bs-target');
        if (targetId) {
            const submenu = document.querySelector(targetId);
            const parentItem = toggleElement.closest('.page__nav__list__item');
            
            if (submenu) {
                submenu.classList.toggle('show');
                if (parentItem) {
                    parentItem.classList.toggle('submenu-shown');
                }
                
                // Save submenu state
                const submenuStates = this.userPreferences.submenuStates || {};
                submenuStates[targetId] = submenu.classList.contains('show');
                this.saveUserPreference('submenuStates', submenuStates);
            }
        }
    }

    /**
     * Initialize submenu states from user preferences
     */
    initSubmenus() {
        const submenuStates = this.userPreferences.submenuStates || {};
        
        Object.keys(submenuStates).forEach(submenuId => {
            if (submenuStates[submenuId]) {
                const submenu = document.querySelector(submenuId);
                if (submenu && !submenu.classList.contains('show')) {
                    const parentItem = submenu.closest('.page__nav__list__item');
                    submenu.classList.add('show');
                    if (parentItem) {
                        parentItem.classList.add('submenu-shown');
                    }
                }
            }
        });
    }

    /**
     * Initialize search functionality within sidebar
     */
    initSearchFunctionality() {
        const searchContainer = document.createElement('div');
        searchContainer.className = 'sidebar-search';
        searchContainer.innerHTML = `
            <div class="sidebar-search__container">
                <input type="search" class="sidebar-search__input form-control" placeholder="البحث في القائمة..." />
                <i class="bi bi-search sidebar-search__icon"></i>
            </div>
            <div class="quick-nav-container" style="padding: 10px; border-bottom: 1px solid rgba(255,255,255,0.1);">
                <div class="quick-nav-title" style="font-size: 12px; color: #888; margin-bottom: 8px;">الوصول السريع</div>
                <button class="quick-nav-btn" data-quick-nav="vision" style="
                    margin-right: 5px;
                    padding: 5px 10px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    border: none;
                    border-radius: 15px;
                    font-size: 12px;
                    cursor: pointer;
                ">
                    👁️ رؤية 2030
                </button>
                <button class="quick-nav-btn" data-quick-nav="performance" style="
                    padding: 5px 10px;
                    background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
                    color: white;
                    border: none;
                    border-radius: 15px;
                    font-size: 12px;
                    cursor: pointer;
                ">
                    📊 الأداء
                </button>
            </div>
        `;
        
        const sidebar = document.querySelector('.page__nav');
        if (sidebar && !document.querySelector('.sidebar-search')) {
            sidebar.insertBefore(searchContainer, sidebar.firstChild);
            
            const searchInput = searchContainer.querySelector('.sidebar-search__input');
            searchInput.addEventListener('input', (e) => this.filterMenuItems(e.target.value));
            
            // Add quick navigation handlers
            const quickNavBtns = searchContainer.querySelectorAll('.quick-nav-btn');
            quickNavBtns.forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const navType = btn.getAttribute('data-quick-nav');
                    this.quickNavigate(navType);
                });
            });
        }
    }
    
    /**
     * Quick navigation to specific module
     */
    quickNavigate(moduleType) {
        if (moduleType === 'vision') {
            // Expand Vision 2030 submenu
            const visionSubmenu = document.getElementById('visionSubmenu');
            const visionLink = document.querySelector('[data-bs-target="#visionSubmenu"]');
            if (visionSubmenu && !visionSubmenu.classList.contains('show')) {
                visionSubmenu.classList.add('show');
                const parentItem = visionSubmenu.closest('.page__nav__list__item');
                if (parentItem) {
                    parentItem.classList.add('submenu-shown');
                }
            }
            // Highlight all vision-related items
            const visionItems = document.querySelectorAll('[data-linked="vision"], #visionSubmenu .page__nav__submenu__item');
            visionItems.forEach(item => {
                item.classList.add('highlight-animation');
                setTimeout(() => item.classList.remove('highlight-animation'), 1000);
            });
            // Scroll to vision section
            if (visionLink) {
                visionLink.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        } else if (moduleType === 'performance') {
            // Expand Performance Management submenu
            const perfSubmenu = document.getElementById('perfMgmtSubmenu');
            const perfLink = document.querySelector('[data-bs-target="#perfMgmtSubmenu"]');
            if (perfSubmenu && !perfSubmenu.classList.contains('show')) {
                perfSubmenu.classList.add('show');
                const parentItem = perfSubmenu.closest('.page__nav__list__item');
                if (parentItem) {
                    parentItem.classList.add('submenu-shown');
                }
            }
            // Highlight all performance-related items
            const perfItems = document.querySelectorAll('[data-linked="performance"], #perfMgmtSubmenu .page__nav__submenu__item');
            perfItems.forEach(item => {
                item.classList.add('highlight-animation');
                setTimeout(() => item.classList.remove('highlight-animation'), 1000);
            });
            // Scroll to performance section
            if (perfLink) {
                perfLink.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    }

    /**
     * Filter menu items based on search query
     */
    filterMenuItems(query) {
        const menuItems = document.querySelectorAll('.page__nav__list__item');
        const normalizedQuery = query.toLowerCase().trim();
        
        if (!normalizedQuery) {
            // Show all items if query is empty
            menuItems.forEach(item => {
                item.style.display = '';
            });
            return;
        }
        
        menuItems.forEach(item => {
            const text = item.textContent.toLowerCase();
            const matches = text.includes(normalizedQuery);
            
            if (matches) {
                item.style.display = '';
                // Expand parent if it's a submenu item
                const submenuContainer = item.querySelector('.page__nav__submenu-container');
                if (submenuContainer && !submenuContainer.classList.contains('show')) {
                    submenuContainer.classList.add('show');
                    item.classList.add('submenu-shown');
                }
            } else {
                // Check if any submenu items match
                const submenuItems = item.querySelectorAll('.page__nav__submenu__item');
                let hasMatchingSubmenu = false;
                
                submenuItems.forEach(subItem => {
                    if (subItem.textContent.toLowerCase().includes(normalizedQuery)) {
                        hasMatchingSubmenu = true;
                    }
                });
                
                if (hasMatchingSubmenu) {
                    item.style.display = '';
                    const submenuContainer = item.querySelector('.page__nav__submenu-container');
                    if (submenuContainer && !submenuContainer.classList.contains('show')) {
                        submenuContainer.classList.add('show');
                        item.classList.add('submenu-shown');
                    }
                } else {
                    item.style.display = 'none';
                }
            }
        });
    }

    /**
     * Initialize keyboard shortcuts
     */
    initKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + B to toggle sidebar
            if ((e.ctrlKey || e.metaKey) && e.key === 'b') {
                e.preventDefault();
                this.toggleSidebar();
            }
            
            // Ctrl/Cmd + K to focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.querySelector('.sidebar-search__input');
                if (searchInput) {
                    searchInput.focus();
                }
            }
        });
    }

    /**
     * Load user preferences from localStorage
     */
    loadUserPreferences() {
        try {
            const preferences = localStorage.getItem('sidebarPreferences');
            return preferences ? JSON.parse(preferences) : {};
        } catch (error) {
            console.error('Error loading preferences:', error);
            return {};
        }
    }

    /**
     * Save user preference
     */
    saveUserPreference(key, value) {
        this.userPreferences[key] = value;
        try {
            localStorage.setItem('sidebarPreferences', JSON.stringify(this.userPreferences));
        } catch (error) {
            console.error('Error saving preferences:', error);
        }
    }

    /**
     * Restore user preferences
     */
    restoreUserPreferences() {
        if (this.userPreferences.sidebarCollapsed) {
            const sidebar = document.querySelector('.page-sidebar');
            if (sidebar) {
                sidebar.classList.add('collapsed');
                document.body.classList.add('sidebar-collapsed');
            }
        }
    }

    /**
     * Highlight menu item (useful for dynamic navigation)
     */
    highlightMenuItem(pageId) {
        // Remove existing highlights
        const allLinks = document.querySelectorAll('.page__nav__link, .page__nav__submenu__item');
        allLinks.forEach(link => link.classList.remove('highlighted'));
        
        // Add highlight to specified page
        const targetLink = document.querySelector(`[data-page="${pageId}"]`);
        if (targetLink) {
            targetLink.classList.add('highlighted');
            // Scroll into view if needed
            targetLink.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }
    
    /**
     * Handle development links
     */
    handleDevelopmentLinks() {
        const devLinks = document.querySelectorAll('[data-dev="true"]');
        
        devLinks.forEach(link => {
            // Add visual indicator
            if (!link.querySelector('.dev-indicator')) {
                const indicator = document.createElement('span');
                indicator.className = 'dev-indicator';
                indicator.innerHTML = '🚧';
                indicator.style.marginRight = '5px';
                link.prepend(indicator);
            }
            
            // Handle clicks on development links
            link.addEventListener('click', (e) => {
                if (!link.getAttribute('href') || link.getAttribute('href') === '#') {
                    e.preventDefault();
                    this.showDevelopmentMessage();
                }
            });
        });
    }
    
    /**
     * Show development message
     */
    showDevelopmentMessage() {
        // Check if toast already exists
        let toast = document.getElementById('devToast');
        
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'devToast';
            toast.className = 'development-toast';
            toast.innerHTML = `
                <div class="toast-content">
                    <span class="toast-icon">🚧</span>
                    <span class="toast-message">هذه الصفحة قيد التطوير حالياً</span>
                </div>
            `;
            document.body.appendChild(toast);
        }
        
        // Show toast
        toast.classList.add('show');
        
        // Hide after 3 seconds
        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    }
    
    /**
     * Prevent active state persistence
     */
    preventActiveStatePersistence() {
        // Remove active class when clicking on links
        const allLinks = document.querySelectorAll('.page__nav__link, .page__nav__submenu__item');
        
        allLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                // Don't prevent default unless it's a dev link
                if (!link.hasAttribute('data-dev')) {
                    // Remove active class after a short delay
                    setTimeout(() => {
                        link.classList.remove('active');
                    }, 100);
                }
            });
            
            // Remove focus style after click
            link.addEventListener('mouseup', function() {
                this.blur();
            });
        });
    }

    /**
     * Public API to refresh sidebar
     */
    async refresh() {
        await this.init();
    }
}

// Initialize sidebar manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.sidebarManager = new SidebarManager();
    window.sidebarManager.init();
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = SidebarManager;
}
