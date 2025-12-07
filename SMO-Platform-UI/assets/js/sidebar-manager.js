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
            
            // Set active states
            this.setActiveStates();
            
            // Initialize event handlers
            this.initEventHandlers();
            
            // Restore user preferences
            this.restoreUserPreferences();
            
            // Initialize submenu states
            this.initSubmenus();
            
            console.log('Sidebar Manager initialized successfully');
        } catch (error) {
            console.error('Error initializing sidebar:', error);
        }
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
        allLinks.forEach(link => link.classList.remove('active'));
        
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
        });
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
        `;
        
        const sidebar = document.querySelector('.page__nav');
        if (sidebar && !document.querySelector('.sidebar-search')) {
            sidebar.insertBefore(searchContainer, sidebar.firstChild);
            
            const searchInput = searchContainer.querySelector('.sidebar-search__input');
            searchInput.addEventListener('input', (e) => this.filterMenuItems(e.target.value));
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
