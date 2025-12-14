/**
 * Role Manager Integration - Silent Version
 * Only adds "تغيير الدور" link to user dropdown
 * No widgets, no popups, no toasts, completely silent
 */

(function() {
    'use strict';
    
    // Suppress any console warnings during initialization
    const originalWarn = console.warn;
    const originalError = console.error;
    
    function suppressConsole() {
        console.warn = function() {};
        console.error = function() {};
    }
    
    function restoreConsole() {
        console.warn = originalWarn;
        console.error = originalError;
    }
    
    function initializeRoleManager() {
        // Skip if we're on login or profile selection pages
        if (window.location.pathname.includes('login.html') || 
            window.location.pathname.includes('profile-selection.html')) {
            return;
        }
        
        // Check if user is authenticated (silently)
        const accessToken = localStorage.getItem('access_token');
        if (!accessToken) {
            return;
        }
        
        // Get current user and role (silently)
        try {
            const user = JSON.parse(localStorage.getItem('user') || '{}');
            const userRoles = user.roles || ['SMO'];
            
            // Only add role switcher link if user has multiple roles
            if (userRoles.length > 1) {
                // Add the dropdown link after a small delay
                setTimeout(function() {
                    suppressConsole();
                    addRoleSwitcherToDropdowns();
                    restoreConsole();
                }, 200);
            }
        } catch (e) {
            // Silently ignore any errors
        }
    }
    
    function addRoleSwitcherToDropdowns() {
        try {
            // Find all user dropdown menus on the page
            const dropdownMenus = document.querySelectorAll('.user-dropdown__menu');
            
            dropdownMenus.forEach(function(dropdownMenu) {
                // Check if role switcher link already added
                if (dropdownMenu.querySelector('.role-switcher-link')) {
                    return;
                }
                
                // Find the profile link (first item)
                const profileLink = dropdownMenu.querySelector('.user-dropdown__actions__item');
                
                if (profileLink) {
                    // Create the role switcher link
                    const roleSwitcherLink = document.createElement('a');
                    roleSwitcherLink.className = 'user-dropdown__actions__item role-switcher-link';
                    roleSwitcherLink.href = '#';
                    
                    // Create the icon
                    const iconSpan = document.createElement('span');
                    iconSpan.className = 'user-dropdown__actions__item__icon';
                    
                    // Try to use an image first, fallback to icon
                    const img = document.createElement('img');
                    img.src = 'images/switch-icon.svg';
                    img.alt = '';
                    img.onerror = function() {
                        iconSpan.innerHTML = '<i class="bi bi-arrow-repeat"></i>';
                    };
                    iconSpan.appendChild(img);
                    
                    // Create the title
                    const titleSpan = document.createElement('span');
                    titleSpan.className = 'user-dropdown__actions__item__title';
                    titleSpan.textContent = 'تغيير الدور';
                    
                    // Append elements
                    roleSwitcherLink.appendChild(iconSpan);
                    roleSwitcherLink.appendChild(titleSpan);
                    
                    // Add click handler (silent navigation)
                    roleSwitcherLink.onclick = function(e) {
                        e.preventDefault();
                        e.stopPropagation();
                        
                        // Navigate silently without any notifications
                        window.location.href = 'profile-selection.html';
                        return false;
                    };
                    
                    // Insert after the profile link
                    profileLink.parentNode.insertBefore(roleSwitcherLink, profileLink.nextSibling);
                }
            });
        } catch (e) {
            // Silently ignore any errors
        }
    }
    
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeRoleManager);
    } else {
        initializeRoleManager();
    }
    
    // Minimal export
    window.RoleManager = {
        goToRoleSelection: function() {
            window.location.href = 'profile-selection.html';
        }
    };
    
    // Hide any existing alerts or toasts that might appear on page load
    try {
        // Hide toast notifications
        const toastContainer = document.getElementById('toastContainer');
        if (toastContainer) {
            const toasts = toastContainer.querySelectorAll('.toast');
            toasts.forEach(function(toast) {
                const toastText = toast.textContent || '';
                if (toastText.includes('role') || toastText.includes('Role') || 
                    toastText.includes('صلاحية') || toastText.includes('دور')) {
                    toast.style.display = 'none';
                }
            });
        }
        
        // Hide any alert divs that might be validation errors from page load
        setTimeout(function() {
            const alerts = document.querySelectorAll('.alert.alert-dismissible');
            alerts.forEach(function(alert) {
                const alertText = alert.textContent || '';
                // Hide alerts about creating alerts or brief descriptions
                if (alertText.includes('إدخال شرح') || alertText.includes('إنشاء التنبيه') ||
                    alertText.includes('brief description') || alertText.includes('create alert')) {
                    alert.style.display = 'none';
                    alert.remove(); // Completely remove it
                }
            });
        }, 100);
        
    } catch (e) {
        // Ignore
    }
})();