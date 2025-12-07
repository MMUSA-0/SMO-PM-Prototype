/**
 * Loading States Manager for SMO Platform
 * Provides consistent loading indicators across the application
 */

(function() {
    'use strict';

    // Loading state configuration
    const LoadingConfig = {
        defaultMessage: 'جاري التحميل...',
        errorMessage: 'حدث خطأ في تحميل البيانات',
        emptyMessage: 'لا توجد بيانات',
        retryMessage: 'إعادة المحاولة',
        fadeInDuration: 300,
        fadeOutDuration: 200,
        skeletonRows: 5
    };

    // Loading state manager
    const LoadingState = {
        activeLoaders: new Map(),

        /**
         * Show loading overlay on element
         */
        show: function(elementId, options = {}) {
            const element = document.getElementById(elementId);
            if (!element) {
                console.warn(`Element with ID "${elementId}" not found`);
                return;
            }

            // Merge options with defaults
            const config = {
                message: options.message || LoadingConfig.defaultMessage,
                type: options.type || 'spinner', // spinner, skeleton, progress
                overlay: options.overlay !== false,
                blur: options.blur || false
            };

            // Store original content
            if (!this.activeLoaders.has(elementId)) {
                this.activeLoaders.set(elementId, {
                    originalContent: element.innerHTML,
                    originalPosition: element.style.position
                });
            }

            // Apply loading state based on type
            switch (config.type) {
                case 'skeleton':
                    this.showSkeleton(element, options.rows || LoadingConfig.skeletonRows);
                    break;
                case 'progress':
                    this.showProgress(element, config);
                    break;
                default:
                    this.showSpinner(element, config);
            }
        },

        /**
         * Show spinner loading state
         */
        showSpinner: function(element, config) {
            const loadingHTML = `
                <div class="loading-overlay ${config.blur ? 'loading-blur' : ''}" 
                     style="opacity: 0; ${config.overlay ? '' : 'background: transparent;'}">
                    <div class="loading-content">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">${config.message}</span>
                        </div>
                        <p class="loading-message mt-3">${config.message}</p>
                    </div>
                </div>
            `;

            // Set relative position if needed
            if (config.overlay && element.style.position === '') {
                element.style.position = 'relative';
            }

            // Add loading overlay
            element.insertAdjacentHTML('beforeend', loadingHTML);
            
            // Fade in animation
            const overlay = element.querySelector('.loading-overlay');
            setTimeout(() => {
                overlay.style.transition = `opacity ${LoadingConfig.fadeInDuration}ms`;
                overlay.style.opacity = '1';
            }, 10);
        },

        /**
         * Show skeleton loader
         */
        showSkeleton: function(element, rows) {
            const skeletonHTML = this.generateSkeleton(rows);
            element.innerHTML = `
                <div class="skeleton-loader" style="opacity: 0;">
                    ${skeletonHTML}
                </div>
            `;

            // Fade in animation
            const skeleton = element.querySelector('.skeleton-loader');
            setTimeout(() => {
                skeleton.style.transition = `opacity ${LoadingConfig.fadeInDuration}ms`;
                skeleton.style.opacity = '1';
            }, 10);
        },

        /**
         * Show progress bar
         */
        showProgress: function(element, config) {
            const progressHTML = `
                <div class="loading-progress-container" style="opacity: 0;">
                    <div class="loading-progress-message">${config.message}</div>
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped progress-bar-animated" 
                             role="progressbar" 
                             style="width: 0%"
                             aria-valuenow="0" 
                             aria-valuemin="0" 
                             aria-valuemax="100">
                        </div>
                    </div>
                    <div class="loading-progress-percent">0%</div>
                </div>
            `;

            element.innerHTML = progressHTML;
            
            // Fade in animation
            const container = element.querySelector('.loading-progress-container');
            setTimeout(() => {
                container.style.transition = `opacity ${LoadingConfig.fadeInDuration}ms`;
                container.style.opacity = '1';
            }, 10);
        },

        /**
         * Update progress bar
         */
        updateProgress: function(elementId, percent, message) {
            const element = document.getElementById(elementId);
            if (!element) return;

            const progressBar = element.querySelector('.progress-bar');
            const percentText = element.querySelector('.loading-progress-percent');
            const messageText = element.querySelector('.loading-progress-message');

            if (progressBar) {
                progressBar.style.width = `${percent}%`;
                progressBar.setAttribute('aria-valuenow', percent);
            }

            if (percentText) {
                percentText.textContent = `${percent}%`;
            }

            if (messageText && message) {
                messageText.textContent = message;
            }
        },

        /**
         * Hide loading state
         */
        hide: function(elementId, options = {}) {
            const element = document.getElementById(elementId);
            if (!element) return;

            const loaderData = this.activeLoaders.get(elementId);
            if (!loaderData) return;

            // Find loading overlay or skeleton
            const loader = element.querySelector('.loading-overlay, .skeleton-loader, .loading-progress-container');
            
            if (loader) {
                // Fade out animation
                loader.style.transition = `opacity ${LoadingConfig.fadeOutDuration}ms`;
                loader.style.opacity = '0';

                setTimeout(() => {
                    // Restore original content if specified
                    if (options.restoreContent !== false && loaderData.originalContent) {
                        element.innerHTML = loaderData.originalContent;
                    } else {
                        loader.remove();
                    }

                    // Restore original position
                    if (loaderData.originalPosition !== undefined) {
                        element.style.position = loaderData.originalPosition;
                    }

                    // Clean up
                    this.activeLoaders.delete(elementId);
                }, LoadingConfig.fadeOutDuration);
            }
        },

        /**
         * Show error state
         */
        showError: function(elementId, options = {}) {
            const element = document.getElementById(elementId);
            if (!element) return;

            const config = {
                message: options.message || LoadingConfig.errorMessage,
                icon: options.icon || 'bi-exclamation-triangle',
                retry: options.retry || false,
                details: options.details || null
            };

            const errorHTML = `
                <div class="error-state">
                    <div class="error-icon">
                        <i class="bi ${config.icon} text-danger"></i>
                    </div>
                    <h5 class="error-message">${config.message}</h5>
                    ${config.details ? `<p class="error-details text-muted">${config.details}</p>` : ''}
                    ${config.retry ? `
                        <button class="btn btn-primary mt-3" onclick="${config.retry}">
                            <i class="bi bi-arrow-clockwise"></i> ${LoadingConfig.retryMessage}
                        </button>
                    ` : ''}
                </div>
            `;

            element.innerHTML = errorHTML;
        },

        /**
         * Show empty state
         */
        showEmpty: function(elementId, options = {}) {
            const element = document.getElementById(elementId);
            if (!element) return;

            const config = {
                message: options.message || LoadingConfig.emptyMessage,
                icon: options.icon || 'bi-inbox',
                action: options.action || null,
                actionText: options.actionText || 'إضافة'
            };

            const emptyHTML = `
                <div class="empty-state">
                    <div class="empty-icon">
                        <i class="bi ${config.icon} text-muted"></i>
                    </div>
                    <h5 class="empty-message text-muted">${config.message}</h5>
                    ${config.action ? `
                        <button class="btn btn-outline-primary mt-3" onclick="${config.action}">
                            <i class="bi bi-plus-circle"></i> ${config.actionText}
                        </button>
                    ` : ''}
                </div>
            `;

            element.innerHTML = emptyHTML;
        },

        /**
         * Generate skeleton HTML
         */
        generateSkeleton: function(rows) {
            const skeletons = [];
            
            for (let i = 0; i < rows; i++) {
                const widths = [
                    60 + Math.random() * 40,
                    40 + Math.random() * 30,
                    50 + Math.random() * 40
                ];
                
                skeletons.push(`
                    <div class="skeleton-item">
                        <div class="skeleton-line" style="width: ${widths[0]}%"></div>
                        <div class="skeleton-line" style="width: ${widths[1]}%"></div>
                        ${Math.random() > 0.5 ? `<div class="skeleton-line" style="width: ${widths[2]}%"></div>` : ''}
                    </div>
                `);
            }
            
            return skeletons.join('');
        },

        /**
         * Show toast notification
         */
        showToast: function(message, type = 'info', duration = 3000) {
            const toastHTML = `
                <div class="toast toast-${type}" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="toast-header">
                        <i class="bi ${this.getToastIcon(type)} me-2"></i>
                        <strong class="me-auto">${this.getToastTitle(type)}</strong>
                        <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            `;

            // Create toast container if it doesn't exist
            let toastContainer = document.getElementById('toastContainer');
            if (!toastContainer) {
                toastContainer = document.createElement('div');
                toastContainer.id = 'toastContainer';
                toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
                toastContainer.style.zIndex = '9999';
                document.body.appendChild(toastContainer);
            }

            // Add toast to container
            toastContainer.insertAdjacentHTML('beforeend', toastHTML);
            
            // Initialize and show toast
            const toastElement = toastContainer.lastElementChild;
            const toast = new bootstrap.Toast(toastElement, {
                autohide: true,
                delay: duration
            });
            toast.show();

            // Remove toast element after it's hidden
            toastElement.addEventListener('hidden.bs.toast', function() {
                this.remove();
            });
        },

        /**
         * Get toast icon based on type
         */
        getToastIcon: function(type) {
            const icons = {
                success: 'bi-check-circle-fill text-success',
                error: 'bi-x-circle-fill text-danger',
                warning: 'bi-exclamation-triangle-fill text-warning',
                info: 'bi-info-circle-fill text-info'
            };
            return icons[type] || icons.info;
        },

        /**
         * Get toast title based on type
         */
        getToastTitle: function(type) {
            const titles = {
                success: 'نجاح',
                error: 'خطأ',
                warning: 'تحذير',
                info: 'معلومة'
            };
            return titles[type] || titles.info;
        }
    };

    // Add CSS styles if not already added
    if (!document.getElementById('loadingStyles')) {
        const styles = document.createElement('style');
        styles.id = 'loadingStyles';
        styles.innerHTML = `
            .loading-overlay {
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(255, 255, 255, 0.95);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 1000;
                min-height: 100px;
            }
            
            .loading-blur {
                backdrop-filter: blur(3px);
                background: rgba(255, 255, 255, 0.85) !important;
            }
            
            .loading-content {
                text-align: center;
                padding: 20px;
            }
            
            .loading-message {
                color: #6c757d;
                margin-top: 15px;
            }
            
            .skeleton-loader {
                padding: 20px;
            }
            
            .skeleton-item {
                margin-bottom: 20px;
            }
            
            .skeleton-line {
                height: 12px;
                background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                background-size: 200% 100%;
                animation: skeleton-loading 1.5s infinite;
                margin-bottom: 8px;
                border-radius: 4px;
            }
            
            @keyframes skeleton-loading {
                0% { background-position: 200% 0; }
                100% { background-position: -200% 0; }
            }
            
            .loading-progress-container {
                padding: 20px;
                text-align: center;
            }
            
            .loading-progress-message {
                margin-bottom: 15px;
                color: #6c757d;
            }
            
            .loading-progress-percent {
                margin-top: 10px;
                font-weight: bold;
                color: #495057;
            }
            
            .error-state,
            .empty-state {
                text-align: center;
                padding: 50px 20px;
            }
            
            .error-icon,
            .empty-icon {
                font-size: 4em;
                margin-bottom: 20px;
            }
            
            .error-message,
            .empty-message {
                margin-bottom: 10px;
            }
            
            .error-details {
                font-size: 0.9em;
            }
            
            .toast-success .toast-header {
                background-color: #d1e7dd;
                color: #0f5132;
            }
            
            .toast-error .toast-header {
                background-color: #f8d7da;
                color: #842029;
            }
            
            .toast-warning .toast-header {
                background-color: #fff3cd;
                color: #664d03;
            }
            
            .toast-info .toast-header {
                background-color: #cff4fc;
                color: #055160;
            }
        `;
        document.head.appendChild(styles);
    }

    // Export to global scope
    window.LoadingState = LoadingState;

    // Helper functions for common loading patterns
    window.withLoading = async function(elementId, asyncFunction, options = {}) {
        try {
            LoadingState.show(elementId, options);
            const result = await asyncFunction();
            LoadingState.hide(elementId);
            return result;
        } catch (error) {
            LoadingState.showError(elementId, {
                message: options.errorMessage || 'حدث خطأ',
                details: error.message,
                retry: options.retry
            });
            throw error;
        }
    };

    // Auto-apply loading states to forms
    document.addEventListener('DOMContentLoaded', function() {
        // Auto-loading for forms with data-loading attribute
        document.querySelectorAll('form[data-loading]').forEach(form => {
            form.addEventListener('submit', function(e) {
                const button = form.querySelector('button[type="submit"]');
                if (button) {
                    button.disabled = true;
                    const originalText = button.innerHTML;
                    button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>جاري الحفظ...';
                    
                    // Re-enable after 5 seconds (safety measure)
                    setTimeout(() => {
                        button.disabled = false;
                        button.innerHTML = originalText;
                    }, 5000);
                }
            });
        });
        
        // Auto-loading for buttons with data-loading attribute
        document.querySelectorAll('button[data-loading]').forEach(button => {
            button.addEventListener('click', function() {
                if (!this.disabled) {
                    const originalText = this.innerHTML;
                    this.disabled = true;
                    this.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>' + (this.dataset.loading || 'جاري التحميل...');
                    
                    // Re-enable after 3 seconds (safety measure)
                    setTimeout(() => {
                        this.disabled = false;
                        this.innerHTML = originalText;
                    }, 3000);
                }
            });
        });
    });

})();