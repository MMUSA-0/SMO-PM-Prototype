/**
 * Development Indicators Management System
 * This module manages development status indicators across the SMO platform
 */

const DevelopmentIndicators = (function() {
    'use strict';

    // Configuration for pages and their development status
    const developmentConfig = {
        // Pages that don't exist yet (referenced in sidebar but missing)
        missingPages: [
            'vision-goals.html',
            'vision-indicators.html', 
            'vision-programs.html',
            'vision-progress.html',
            'users-management.html',
            'roles-permissions.html',
            'system-config.html'
        ],
        
        // Redirect missing pages to a 404 page with development message
        handleMissingPages: true,
        
        // Pages that exist but have incomplete features
        incompletePages: {
            'milestone-tracking.html': {
                status: 'partial',
                progress: 60,
                missingFeatures: [
                    'مخطط جانت (Gantt Chart)',
                    'التصدير إلى Excel',
                    'التقارير التفصيلية',
                    'إشعارات التأخير'
                ]
            },
            'achievements.html': {
                status: 'partial',
                progress: 70,
                missingFeatures: [
                    'رفع الملفات المرفقة',
                    'التحقق من الإنجازات',
                    'لوحة معلومات الإنجازات'
                ]
            },
            'performance-initiatives.html': {
                status: 'partial', 
                progress: 75,
                missingFeatures: [
                    'قسم الوثائق',
                    'ربط المؤشرات',
                    'التقارير المخصصة'
                ]
            },
            'risk-escalation.html': {
                status: 'partial',
                progress: 50,
                missingFeatures: [
                    'تصعيد المخاطر التلقائي',
                    'لوحة معلومات المخاطر',
                    'التنبيهات الذكية',
                    'تحليل الاتجاهات'
                ]
            },
            'audit-logs.html': {
                status: 'partial',
                progress: 40,
                missingFeatures: [
                    'البحث المتقدم',
                    'التصدير إلى PDF',
                    'تصفية حسب النوع',
                    'التقارير الدورية'
                ]
            },
            'profile.html': {
                status: 'partial',
                progress: 65,
                missingFeatures: [
                    'تغيير الصورة الشخصية',
                    'إعدادات الإشعارات',
                    'سجل النشاط'
                ]
            },
            'settings.html': {
                status: 'partial',
                progress: 55,
                missingFeatures: [
                    'إعدادات اللغة',
                    'إعدادات التنبيهات',
                    'النسخ الاحتياطي',
                    'إدارة API'
                ]
            }
        },

        // Messages in Arabic and English
        messages: {
            ar: {
                banner: '⚠️ هذه الصفحة قيد التطوير - بعض الميزات قد لا تعمل بشكل كامل',
                widget: 'حالة التطوير',
                progress: 'التقدم',
                missingFeatures: 'الميزات غير المكتملة',
                backToHome: 'العودة للصفحة الرئيسية',
                pageNotFound: 'الصفحة غير موجودة',
                pageUnderDevelopment: 'هذه الصفحة قيد التطوير حالياً',
                expectedCompletion: 'الإنجاز المتوقع: قريباً'
            },
            en: {
                banner: '⚠️ This page is under development - some features may not work fully',
                widget: 'Development Status',
                progress: 'Progress',
                missingFeatures: 'Incomplete Features',
                backToHome: 'Back to Home',
                pageNotFound: 'Page Not Found',
                pageUnderDevelopment: 'This page is currently under development',
                expectedCompletion: 'Expected completion: Soon'
            }
        }
    };

    // Determine current language
    function getCurrentLanguage() {
        return document.documentElement.lang === 'ar' || document.dir === 'rtl' ? 'ar' : 'en';
    }

    // Get current page name
    function getCurrentPage() {
        const path = window.location.pathname;
        return path.substring(path.lastIndexOf('/') + 1);
    }

    // Check if current page is under development
    function isPageUnderDevelopment() {
        const currentPage = getCurrentPage();
        return developmentConfig.incompletePages.hasOwnProperty(currentPage);
    }

    // Check if current page is missing
    function isPageMissing() {
        const currentPage = getCurrentPage();
        return developmentConfig.missingPages.includes(currentPage);
    }

    // Add development banner to page
    function addDevelopmentBanner() {
        const lang = getCurrentLanguage();
        const currentPage = getCurrentPage();
        
        if (!isPageUnderDevelopment()) return;

        const banner = document.createElement('div');
        banner.className = `dev-banner ${lang === 'ar' ? 'rtl' : ''}`;
        banner.innerHTML = `
            <i class="bi bi-exclamation-triangle"></i>
            <span>${developmentConfig.messages[lang].banner}</span>
            <button class="close-dev-banner" onclick="DevelopmentIndicators.closeBanner()">
                <i class="bi bi-x"></i>
            </button>
        `;
        
        document.body.insertBefore(banner, document.body.firstChild);
        document.body.classList.add('has-dev-banner');
    }

    // Add development widget
    function addDevelopmentWidget() {
        const lang = getCurrentLanguage();
        const currentPage = getCurrentPage();
        const pageInfo = developmentConfig.incompletePages[currentPage];
        
        if (!pageInfo) return;

        const widget = document.createElement('div');
        widget.className = `dev-widget ${lang === 'ar' ? 'rtl' : ''}`;
        widget.innerHTML = `
            <div class="dev-widget-header">
                <i class="bi bi-tools"></i>
                <span>${developmentConfig.messages[lang].widget}</span>
            </div>
            <div class="dev-status">
                ${developmentConfig.messages[lang].progress}: ${pageInfo.progress}%
            </div>
            <div class="dev-progress">
                <div class="dev-progress-bar" style="width: ${pageInfo.progress}%"></div>
            </div>
            ${pageInfo.missingFeatures ? `
                <details style="margin-top: 10px;">
                    <summary style="cursor: pointer; font-size: 12px;">
                        ${developmentConfig.messages[lang].missingFeatures} (${pageInfo.missingFeatures.length})
                    </summary>
                    <ul style="margin: 5px 0 0 20px; padding: 0; font-size: 11px;">
                        ${pageInfo.missingFeatures.map(f => `<li>${f}</li>`).join('')}
                    </ul>
                </details>
            ` : ''}
        `;
        
        document.body.appendChild(widget);
    }

    // Close development banner
    function closeBanner() {
        const banner = document.querySelector('.dev-banner');
        if (banner) {
            banner.remove();
            document.body.classList.remove('has-dev-banner');
        }
    }

    // Create missing page placeholder
    function createMissingPagePlaceholder() {
        const lang = getCurrentLanguage();
        const html = `
            <!DOCTYPE html>
            <html lang="${lang}" dir="${lang === 'ar' ? 'rtl' : 'ltr'}">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>${developmentConfig.messages[lang].pageNotFound} - SMO Platform</title>
                <link href="assets/css/bootstrap.min.css" rel="stylesheet">
                <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css" rel="stylesheet">
                <link href="assets/css/dev-indicators.css" rel="stylesheet">
            </head>
            <body>
                <div class="missing-page-container">
                    <div class="missing-page-card">
                        <i class="bi bi-cone-striped"></i>
                        <h1>${developmentConfig.messages[lang].pageUnderDevelopment}</h1>
                        <p>${developmentConfig.messages[lang].expectedCompletion}</p>
                        <a href="index.html" class="btn">
                            <i class="bi bi-house"></i> ${developmentConfig.messages[lang].backToHome}
                        </a>
                    </div>
                </div>
            </body>
            </html>
        `;
        return html;
    }

    // Update sidebar indicators
    function updateSidebarIndicators() {
        // Add dev badges to incomplete pages in sidebar
        const sidebarLinks = document.querySelectorAll('.page__nav__submenu__item, .page__nav__link');
        
        sidebarLinks.forEach(link => {
            const href = link.getAttribute('href');
            if (!href) return;
            
            const pageName = href.substring(href.lastIndexOf('/') + 1);
            
            // Check if page is missing or incomplete
            if (developmentConfig.missingPages.includes(pageName) || 
                developmentConfig.incompletePages.hasOwnProperty(pageName)) {
                
                // Add data-dev attribute
                link.setAttribute('data-dev', 'true');
                
                // Add badge if not already present
                if (!link.querySelector('.dev-badge')) {
                    const badge = document.createElement('span');
                    badge.className = 'dev-badge';
                    badge.textContent = getCurrentLanguage() === 'ar' ? 'قيد التطوير' : 'Dev';
                    link.appendChild(badge);
                }
            }
        });
    }

    // Handle clicks on missing pages
    function handleMissingPageLinks() {
        document.addEventListener('click', function(e) {
            const link = e.target.closest('a[href]');
            if (!link) return;
            
            const href = link.getAttribute('href');
            if (!href) return;
            
            const pageName = href.substring(href.lastIndexOf('/') + 1);
            
            if (developmentConfig.missingPages.includes(pageName)) {
                e.preventDefault();
                
                // Show alert with development message
                const lang = getCurrentLanguage();
                const message = lang === 'ar' 
                    ? `⚠️ هذه الصفحة قيد التطوير\n\n${pageName}\n\nسيتم إطلاقها قريباً`
                    : `⚠️ This page is under development\n\n${pageName}\n\nComing soon`;
                
                alert(message);
                
                // Optionally redirect to 404 page
                if (window.location.pathname !== '/404.html') {
                    // You can uncomment this to redirect to 404 page
                    // window.location.href = '404.html?page=' + pageName;
                }
            }
        });
    }
    
    // Initialize development indicators
    function init() {
        // Load CSS if not already loaded
        if (!document.querySelector('link[href*="dev-indicators.css"]')) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = 'assets/css/dev-indicators.css';
            document.head.appendChild(link);
        }

        // Add indicators based on page status
        if (isPageUnderDevelopment()) {
            addDevelopmentBanner();
            addDevelopmentWidget();
        }
        
        // Update sidebar if it exists
        if (document.querySelector('.page__nav')) {
            updateSidebarIndicators();
        }

        // Check for missing features and add indicators
        markIncompleteSections();
        
        // Handle clicks on missing pages
        if (developmentConfig.handleMissingPages) {
            handleMissingPageLinks();
        }
    }

    // Mark incomplete sections within pages
    function markIncompleteSections() {
        // Add indicators to specific incomplete features
        const incompleteButtons = document.querySelectorAll('[onclick*="قيد التطوير"], [onclick*="Under Development"]');
        incompleteButtons.forEach(btn => {
            btn.style.position = 'relative';
            if (!btn.querySelector('.dev-indicator')) {
                const indicator = document.createElement('span');
                indicator.className = 'dev-indicator';
                indicator.textContent = '🚧';
                indicator.style.position = 'absolute';
                indicator.style.top = '-5px';
                indicator.style.right = '-5px';
                btn.appendChild(indicator);
            }
        });
    }

    // Public API
    return {
        init: init,
        closeBanner: closeBanner,
        isPageUnderDevelopment: isPageUnderDevelopment,
        getDevelopmentStatus: function() {
            return developmentConfig;
        },
        markFeatureIncomplete: function(element) {
            if (element) {
                element.classList.add('dev-features-incomplete');
            }
        }
    };
})();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', DevelopmentIndicators.init);
} else {
    DevelopmentIndicators.init();
}

// Export for use in other scripts
window.DevelopmentIndicators = DevelopmentIndicators;
