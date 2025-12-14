/**
 * Role Manager Module
 * Handles user role display and switching across all pages
 * @module RoleManager
 */

class RoleManager {
    constructor() {
        this.currentRole = localStorage.getItem('currentRole') || 'SMO';
        this.userRoles = this.loadUserRoles();
        this.roleSwitcherVisible = false;
    }

    /**
     * Initialize the role manager
     */
    init() {
        this.injectRoleBadge();
        this.attachEventListeners();
        this.updateRoleDisplay();
    }

    /**
     * Load user roles from localStorage or API
     */
    loadUserRoles() {
        // In production, this would come from the API
        return [
            { 
                name: 'SMO', 
                displayNameAr: 'مكتب الإدارة الاستراتيجية',
                displayNameEn: 'Strategic Management Office',
                color: '#198754'
            },
            { 
                name: 'ExecutiveOffice', 
                displayNameAr: 'المكتب التنفيذي',
                displayNameEn: 'Executive Office',
                color: '#0d6efd'
            },
            { 
                name: 'VRO', 
                displayNameAr: 'مكتب تحقيق الرؤية',
                displayNameEn: 'Vision Realization Office',
                color: '#0dcaf0'
            },
            { 
                name: 'VRP', 
                displayNameAr: 'برنامج تحقيق الرؤية',
                displayNameEn: 'Vision Realization Program',
                color: '#ffc107'
            },
            { 
                name: 'ADAA', 
                displayNameAr: 'المركز الوطني لقياس الأداء',
                displayNameEn: 'ADAA',
                color: '#dc3545'
            },
            { 
                name: 'PerformanceManager', 
                displayNameAr: 'مدير الأداء',
                displayNameEn: 'Performance Manager',
                color: '#6c757d'
            }
        ];
    }

    /**
     * Inject role badge into the header if it doesn't exist
     */
    injectRoleBadge() {
        // Check if role badge already exists
        if (document.getElementById('currentRoleBadge')) {
            return;
        }

        // Find the header actions area
        const headerActions = document.querySelector('.page-header__actions');
        if (!headerActions) {
            // If no unified header, try to find old style header
            this.injectRoleBadgeOldStyle();
            return;
        }

        // Create role badge container
        const roleBadgeContainer = document.createElement('div');
        roleBadgeContainer.style.cssText = 'margin-left: 20px;';
        roleBadgeContainer.innerHTML = `
            <style>
                .role-badge-header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 6px 14px;
                    border-radius: 20px;
                    font-size: 0.85rem;
                    cursor: pointer;
                    transition: all 0.3s ease;
                    display: inline-flex;
                    align-items: center;
                    gap: 8px;
                    white-space: nowrap;
                    box-shadow: 0 2px 8px rgba(102, 126, 234, 0.2);
                }
                .role-badge-header:hover {
                    transform: translateY(-2px);
                    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
                }
                .role-badge-header i {
                    font-size: 0.9rem;
                }
                .role-switcher-mini {
                    position: absolute;
                    top: 60px;
                    left: 20px;
                    background: white;
                    border-radius: 12px;
                    box-shadow: 0 8px 24px rgba(0,0,0,0.15);
                    padding: 15px;
                    min-width: 300px;
                    z-index: 9999;
                    display: none;
                }
                .role-switcher-mini.show {
                    display: block;
                    animation: slideDown 0.3s ease;
                }
                @keyframes slideDown {
                    from {
                        opacity: 0;
                        transform: translateY(-10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
                .role-switcher-mini h6 {
                    color: #6c757d;
                    font-size: 0.8rem;
                    margin-bottom: 12px;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                }
                .role-option {
                    padding: 10px 14px;
                    border-radius: 8px;
                    cursor: pointer;
                    transition: all 0.2s ease;
                    margin-bottom: 6px;
                    border: 2px solid transparent;
                }
                .role-option:hover {
                    background: #f8f9fa;
                    border-color: #e9ecef;
                }
                .role-option.active {
                    background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
                    border-color: #667eea;
                }
                .role-option .role-name {
                    font-weight: 500;
                    display: block;
                    color: #212529;
                }
                .role-option .role-desc {
                    font-size: 0.75rem;
                    color: #6c757d;
                    margin-top: 2px;
                }
                .role-option.active .role-name {
                    color: #667eea;
                }
            </style>
            <div class="role-badge-header" id="currentRoleBadge">
                <i class="bi bi-shield-check"></i>
                <span id="currentRoleText">Loading...</span>
                <i class="bi bi-chevron-down" style="font-size: 0.75rem;"></i>
            </div>
            <div class="role-switcher-mini" id="roleSwitcherMini">
                <h6>الأدوار المتاحة</h6>
                <div id="roleOptions"></div>
            </div>
        `;

        // Insert before the header actions
        headerActions.parentElement.insertBefore(roleBadgeContainer, headerActions);
    }

    /**
     * Inject role badge for pages without unified header
     */
    injectRoleBadgeOldStyle() {
        // Create a floating role badge for pages without proper header
        const floatingBadge = document.createElement('div');
        floatingBadge.style.cssText = `
            position: fixed;
            top: 20px;
            left: 20px;
            z-index: 9998;
        `;
        floatingBadge.innerHTML = `
            <style>
                .role-badge-floating {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 8px 16px;
                    border-radius: 24px;
                    font-size: 0.9rem;
                    cursor: pointer;
                    transition: all 0.3s ease;
                    display: inline-flex;
                    align-items: center;
                    gap: 8px;
                    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
                }
                .role-badge-floating:hover {
                    transform: scale(1.05);
                    box-shadow: 0 6px 16px rgba(102, 126, 234, 0.4);
                }
                .role-switcher-floating {
                    position: fixed;
                    top: 70px;
                    left: 20px;
                    background: white;
                    border-radius: 12px;
                    box-shadow: 0 8px 24px rgba(0,0,0,0.15);
                    padding: 15px;
                    min-width: 300px;
                    z-index: 9999;
                    display: none;
                }
                .role-switcher-floating.show {
                    display: block;
                }
            </style>
            <div class="role-badge-floating" id="currentRoleBadge">
                <i class="bi bi-shield-check"></i>
                <span id="currentRoleText">Loading...</span>
                <i class="bi bi-chevron-down"></i>
            </div>
            <div class="role-switcher-floating" id="roleSwitcherMini">
                <h6 style="color: #6c757d; font-size: 0.8rem; margin-bottom: 12px;">الأدوار المتاحة</h6>
                <div id="roleOptions"></div>
            </div>
        `;
        document.body.appendChild(floatingBadge);
    }

    /**
     * Attach event listeners
     */
    attachEventListeners() {
        // Wait for DOM to be ready
        setTimeout(() => {
            const badge = document.getElementById('currentRoleBadge');
            if (badge) {
                badge.addEventListener('click', () => this.toggleRoleSwitcher());
            }

            // Close on click outside
            document.addEventListener('click', (e) => {
                if (!e.target.closest('#currentRoleBadge') && !e.target.closest('#roleSwitcherMini')) {
                    this.hideRoleSwitcher();
                }
            });
        }, 100);
    }

    /**
     * Toggle role switcher visibility
     */
    toggleRoleSwitcher() {
        const switcher = document.getElementById('roleSwitcherMini');
        if (switcher) {
            this.roleSwitcherVisible = !this.roleSwitcherVisible;
            if (this.roleSwitcherVisible) {
                switcher.classList.add('show');
                this.renderRoleOptions();
            } else {
                switcher.classList.remove('show');
            }
        }
    }

    /**
     * Hide role switcher
     */
    hideRoleSwitcher() {
        const switcher = document.getElementById('roleSwitcherMini');
        if (switcher) {
            switcher.classList.remove('show');
            this.roleSwitcherVisible = false;
        }
    }

    /**
     * Render role options
     */
    renderRoleOptions() {
        const container = document.getElementById('roleOptions');
        if (!container) return;

        const optionsHtml = this.userRoles.map(role => `
            <div class="role-option ${role.name === this.currentRole ? 'active' : ''}" 
                 data-role="${role.name}"
                 style="${role.name === this.currentRole ? `border-color: ${role.color}30;` : ''}">
                <span class="role-name" style="${role.name === this.currentRole ? `color: ${role.color};` : ''}">
                    ${role.displayNameAr}
                </span>
                <span class="role-desc">${role.displayNameEn}</span>
            </div>
        `).join('');

        container.innerHTML = optionsHtml;

        // Attach click handlers
        container.querySelectorAll('.role-option').forEach(option => {
            option.addEventListener('click', () => {
                const roleName = option.dataset.role;
                this.switchRole(roleName);
            });
        });
    }

    /**
     * Switch to a different role
     */
    switchRole(roleName) {
        if (roleName === this.currentRole) {
            this.hideRoleSwitcher();
            return;
        }

        // Update current role
        this.currentRole = roleName;
        localStorage.setItem('currentRole', roleName);

        // Update display
        this.updateRoleDisplay();
        this.hideRoleSwitcher();

        // Show notification
        this.showNotification('تم تبديل الدور بنجاح', 'success');

        // In production, call API to switch role
        if (typeof apiService !== 'undefined') {
            apiService.post('/api/UserProfile/switch-role', { roleName })
                .then(response => {
                    if (response.requiresReauth) {
                        setTimeout(() => {
                            window.location.href = 'login.html';
                        }, 1500);
                    }
                })
                .catch(error => {
                    console.error('Error switching role:', error);
                });
        }

        // Reload page after a delay to reflect new permissions
        setTimeout(() => {
            window.location.reload();
        }, 1500);
    }

    /**
     * Update role display
     */
    updateRoleDisplay() {
        const roleText = document.getElementById('currentRoleText');
        if (roleText) {
            const role = this.userRoles.find(r => r.name === this.currentRole);
            if (role) {
                roleText.textContent = role.displayNameAr;
                
                // Update badge color
                const badge = document.getElementById('currentRoleBadge');
                if (badge) {
                    badge.style.background = `linear-gradient(135deg, ${role.color} 0%, ${role.color}dd 100%)`;
                }
            }
        }
    }

    /**
     * Show notification
     */
    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} position-fixed`;
        notification.style.cssText = `
            top: 80px;
            left: 50%;
            transform: translateX(-50%);
            z-index: 10000;
            min-width: 300px;
            text-align: center;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            animation: slideDown 0.3s ease;
        `;
        notification.innerHTML = `
            <i class="bi bi-check-circle-fill me-2"></i>${message}
        `;
        
        document.body.appendChild(notification);
        
        // Remove after 3 seconds
        setTimeout(() => {
            notification.style.animation = 'slideUp 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }
}

// Auto-initialize on DOM ready
document.addEventListener('DOMContentLoaded', function() {
    // Only initialize if not on login page
    if (!window.location.pathname.includes('login')) {
        const roleManager = new RoleManager();
        roleManager.init();
    }
});

// Export for global use
window.RoleManager = RoleManager;


