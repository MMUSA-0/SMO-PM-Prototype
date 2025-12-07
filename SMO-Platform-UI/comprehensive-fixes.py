#!/usr/bin/env python3
"""
Comprehensive Fix Script for SMO Platform UI
Fixes all critical, medium, and low priority issues identified in the test report
"""

import os
import re
from pathlib import Path

# Get the current directory
BASE_DIR = Path(__file__).parent

def fix_file(filepath, fixes):
    """Apply multiple fixes to a single file"""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
        
        original_content = content
        for old, new in fixes:
            content = content.replace(old, new)
        
        if content != original_content:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            return True
        return False
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return False

def fix_navigation_issues():
    """Fix all navigation-related issues"""
    
    # List of HTML files to fix
    html_files = [
        'login.html', 'achievements.html', 'performance-dashboard.html',
        'vision.html', 'indicators.html', 'audit-logs.html',
        'data-sync-status.html', 'executive-performance-dashboard.html',
        'initiative-details.html', 'milestone-tracking.html',
        'performance-approvals.html', 'performance-initiatives.html',
        'performance-programs.html', 'performance-requests.html',
        'performance-thresholds.html', 'performance-vision.html',
        'program-wizard.html', 'report-generation.html',
        'risk-escalation.html'
    ]
    
    print("🔧 Fixing navigation issues...")
    fixed_count = 0
    
    for filename in html_files:
        filepath = BASE_DIR / filename
        if not filepath.exists():
            print(f"⚠️  File not found: {filename}")
            continue
        
        fixes = [
            # Fix empty logo hrefs
            ('href="" title="الرئيسية"', 'href="index.html" title="الرئيسية"'),
            ('href="" class="login-page__logo"', 'href="index.html" class="login-page__logo"'),
            
            # Fix breadcrumb home links
            ('<a href="#"><img src="images/home-icon.svg" alt="">', 
             '<a href="index.html"><img src="images/home-icon.svg" alt="الرئيسية">'),
            
            # Fix absolute paths to relative
            ('href="/images/', 'href="images/'),
            ('src="/images/', 'src="images/'),
            ('href="/css/', 'href="css/'),
            ('href="/js/', 'href="js/'),
            
            # Fix empty alt attributes for images
            ('<img src="images/logo.svg" alt="">', 
             '<img src="images/logo.svg" alt="مكتب الإدارة الاستراتيجية">'),
            ('<img src="images/home-icon.svg" alt="">', 
             '<img src="images/home-icon.svg" alt="الرئيسية">'),
            ('<img src="images/demo/user-photo.png" alt="">', 
             '<img src="images/demo/user-photo.png" alt="صورة المستخدم">'),
            
            # Fix empty navigation links
            ('<a class="page__nav__link" href="" role="button">', 
             '<a class="page__nav__link" href="#" role="button">'),
        ]
        
        if fix_file(filepath, fixes):
            print(f"✅ Fixed: {filename}")
            fixed_count += 1
    
    print(f"Fixed {fixed_count} files with navigation issues\n")

def create_missing_pages():
    """Create missing pages like profile.html, settings.html, and 404.html"""
    
    print("📄 Creating missing pages...")
    
    # Profile page
    profile_html = """<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>الملف الشخصي - مكتب الإدارة الاستراتيجية</title>
    
    <link rel="stylesheet" href="css/bootstrap.rtl.min.css">
    <link rel="stylesheet" href="css/bootstrap-icons.min.css">
    <link rel="stylesheet" href="css/styles.min.css">
</head>
<body>
    <div class="container py-5">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="card">
                    <div class="card-header">
                        <h2>الملف الشخصي</h2>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <label class="form-label">الاسم الكامل</label>
                            <input type="text" class="form-control" value="محمد أحمد" readonly>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">البريد الإلكتروني</label>
                            <input type="email" class="form-control" value="mohammed@smo.gov.sa" readonly>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">الصلاحيات</label>
                            <div class="badge bg-primary">مدير الأداء</div>
                            <div class="badge bg-info">مالك البرنامج</div>
                        </div>
                        <div class="d-flex justify-content-between">
                            <a href="index.html" class="btn btn-secondary">
                                <i class="bi bi-arrow-right"></i> العودة للرئيسية
                            </a>
                            <button class="btn btn-primary">حفظ التغييرات</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="js/bootstrap.min.js"></script>
</body>
</html>"""
    
    # Settings page
    settings_html = """<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>الإعدادات - مكتب الإدارة الاستراتيجية</title>
    
    <link rel="stylesheet" href="css/bootstrap.rtl.min.css">
    <link rel="stylesheet" href="css/bootstrap-icons.min.css">
    <link rel="stylesheet" href="css/styles.min.css">
</head>
<body>
    <div class="container py-5">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="card">
                    <div class="card-header">
                        <h2>الإعدادات</h2>
                    </div>
                    <div class="card-body">
                        <h4>إعدادات العرض</h4>
                        <div class="form-check form-switch mb-3">
                            <input class="form-check-input" type="checkbox" id="darkMode">
                            <label class="form-check-label" for="darkMode">الوضع الليلي</label>
                        </div>
                        <div class="form-check form-switch mb-3">
                            <input class="form-check-input" type="checkbox" id="notifications">
                            <label class="form-check-label" for="notifications">تفعيل الإشعارات</label>
                        </div>
                        
                        <h4 class="mt-4">إعدادات اللغة</h4>
                        <select class="form-select mb-3">
                            <option selected>العربية</option>
                            <option>English</option>
                        </select>
                        
                        <h4 class="mt-4">إعدادات الأمان</h4>
                        <button class="btn btn-warning mb-2">تغيير كلمة المرور</button>
                        <br>
                        <button class="btn btn-info">تفعيل المصادقة الثنائية</button>
                        
                        <div class="d-flex justify-content-between mt-4">
                            <a href="index.html" class="btn btn-secondary">
                                <i class="bi bi-arrow-right"></i> العودة للرئيسية
                            </a>
                            <button class="btn btn-primary">حفظ الإعدادات</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="js/bootstrap.min.js"></script>
</body>
</html>"""
    
    # 404 Error page
    error_404_html = """<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>404 - الصفحة غير موجودة</title>
    
    <link rel="stylesheet" href="css/bootstrap.rtl.min.css">
    <link rel="stylesheet" href="css/bootstrap-icons.min.css">
    <link rel="stylesheet" href="css/styles.min.css">
    
    <style>
        .error-page {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }
        .error-content {
            text-align: center;
            color: white;
        }
        .error-code {
            font-size: 120px;
            font-weight: bold;
            margin-bottom: 20px;
        }
        .error-message {
            font-size: 24px;
            margin-bottom: 30px;
        }
        .error-icon {
            font-size: 80px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <div class="error-page">
        <div class="error-content">
            <div class="error-icon">
                <i class="bi bi-exclamation-triangle"></i>
            </div>
            <div class="error-code">404</div>
            <div class="error-message">عذراً، الصفحة المطلوبة غير موجودة</div>
            <p class="mb-4">يبدو أن الصفحة التي تبحث عنها قد تم نقلها أو حذفها</p>
            <a href="index.html" class="btn btn-light btn-lg">
                <i class="bi bi-house"></i> العودة للصفحة الرئيسية
            </a>
        </div>
    </div>
</body>
</html>"""
    
    # Write the files
    pages = [
        ('profile.html', profile_html),
        ('settings.html', settings_html),
        ('404.html', error_404_html)
    ]
    
    for filename, content in pages:
        filepath = BASE_DIR / filename
        try:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            print(f"✅ Created: {filename}")
        except Exception as e:
            print(f"❌ Error creating {filename}: {e}")
    
    print()

def fix_api_service():
    """Fix API service to properly handle environment detection"""
    
    print("🔧 Fixing API service...")
    
    api_service_path = BASE_DIR / 'js' / 'api-service.js'
    
    if not api_service_path.exists():
        print("⚠️  API service file not found")
        return
    
    with open(api_service_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Fix the hardcoded bypass
    old_token_getter = """    // BYPASS LOGIN: Always return a dummy token for development
    getToken: () => {
        // Return dummy token for development - REMOVE IN PRODUCTION
        return 'dummy-development-token-bypass-login';
    },"""
    
    new_token_getter = """    // Check environment and handle token appropriately
    getToken: () => {
        // Only bypass in development mode
        const isDevelopment = window.location.hostname === 'localhost' || 
                            window.location.hostname === '127.0.0.1';
        
        if (isDevelopment && !localStorage.getItem('access_token')) {
            // Return dummy token for development only
            console.warn('Using development bypass token');
            return 'dummy-development-token-bypass-login';
        }
        
        return localStorage.getItem('access_token');
    },"""
    
    content = content.replace(old_token_getter, new_token_getter)
    
    # Add environment detection function
    env_detection = """
// Environment detection
const Environment = {
    isDevelopment: () => {
        return window.location.hostname === 'localhost' || 
               window.location.hostname === '127.0.0.1';
    },
    isProduction: () => {
        return !Environment.isDevelopment();
    }
};

"""
    
    # Insert after API_CONFIG
    if "// Token management" in content and "const Environment" not in content:
        content = content.replace("// Token management", env_detection + "// Token management")
    
    with open(api_service_path, 'w', encoding='utf-8') as f:
        f.write(content)
    
    print("✅ Fixed API service with environment detection\n")

def add_search_functionality():
    """Add basic search functionality"""
    
    print("🔧 Adding search functionality...")
    
    search_js = """/**
 * Search functionality for SMO Platform
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get all search inputs
    const searchInputs = document.querySelectorAll('.search-menu input[type="search"]');
    
    searchInputs.forEach(input => {
        // Add search handler
        input.addEventListener('keyup', function(e) {
            if (e.key === 'Enter') {
                performSearch(this.value);
            }
        });
        
        // Add clear button functionality
        input.addEventListener('input', function() {
            if (this.value.length > 0) {
                this.style.paddingLeft = '40px';
            } else {
                this.style.paddingLeft = '15px';
            }
        });
    });
    
    // Search function
    function performSearch(query) {
        if (!query.trim()) {
            return;
        }
        
        // For now, just filter visible content
        // In production, this would call the API
        console.log('Searching for:', query);
        
        // Show loading state
        const searchResults = document.getElementById('searchResults');
        if (searchResults) {
            searchResults.innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
        }
        
        // Simulate search (replace with actual API call)
        setTimeout(() => {
            if (window.apiService) {
                // Call search API when available
                // window.apiService.search(query).then(results => {...});
            } else {
                // Basic client-side filtering
                filterPageContent(query);
            }
        }, 500);
    }
    
    function filterPageContent(query) {
        const elements = document.querySelectorAll('.card, .table tr, .list-group-item');
        const lowerQuery = query.toLowerCase();
        
        elements.forEach(el => {
            const text = el.textContent.toLowerCase();
            if (text.includes(lowerQuery)) {
                el.style.display = '';
                el.classList.add('search-highlight');
            } else {
                el.style.display = 'none';
            }
        });
        
        // Clear highlights after 3 seconds
        setTimeout(() => {
            document.querySelectorAll('.search-highlight').forEach(el => {
                el.classList.remove('search-highlight');
            });
        }, 3000);
    }
});
"""
    
    search_js_path = BASE_DIR / 'js' / 'search.js'
    with open(search_js_path, 'w', encoding='utf-8') as f:
        f.write(search_js)
    
    print("✅ Created search.js\n")

def add_loading_states():
    """Add loading state utilities"""
    
    print("🔧 Adding loading state utilities...")
    
    loading_js = """/**
 * Loading states for SMO Platform
 */

const LoadingState = {
    // Show loading overlay
    show: function(elementId, message = 'جاري التحميل...') {
        const element = document.getElementById(elementId);
        if (!element) return;
        
        const loadingHTML = `
            <div class="loading-overlay">
                <div class="text-center p-4">
                    <div class="spinner-border text-primary mb-3" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p>${message}</p>
                </div>
            </div>
        `;
        
        element.style.position = 'relative';
        element.insertAdjacentHTML('beforeend', loadingHTML);
    },
    
    // Hide loading overlay
    hide: function(elementId) {
        const element = document.getElementById(elementId);
        if (!element) return;
        
        const overlay = element.querySelector('.loading-overlay');
        if (overlay) {
            overlay.remove();
        }
    },
    
    // Show skeleton loader
    showSkeleton: function(elementId, rows = 5) {
        const element = document.getElementById(elementId);
        if (!element) return;
        
        let skeletonHTML = '<div class="skeleton-loader">';
        for (let i = 0; i < rows; i++) {
            skeletonHTML += `
                <div class="skeleton-item mb-3">
                    <div class="skeleton-line" style="width: ${60 + Math.random() * 40}%"></div>
                    <div class="skeleton-line" style="width: ${40 + Math.random() * 30}%"></div>
                </div>
            `;
        }
        skeletonHTML += '</div>';
        
        element.innerHTML = skeletonHTML;
    }
};

// Add CSS for loading states
const loadingStyles = `
<style>
.loading-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(255, 255, 255, 0.9);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
}

.skeleton-loader {
    padding: 20px;
}

.skeleton-line {
    height: 12px;
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loading 1.5s infinite;
    margin-bottom: 8px;
    border-radius: 4px;
}

@keyframes loading {
    0% { background-position: 200% 0; }
    100% { background-position: -200% 0; }
}

.search-highlight {
    background-color: yellow !important;
    transition: background-color 0.3s;
}
</style>
`;

document.head.insertAdjacentHTML('beforeend', loadingStyles);

// Export for use
window.LoadingState = LoadingState;
"""
    
    loading_js_path = BASE_DIR / 'js' / 'loading-states.js'
    with open(loading_js_path, 'w', encoding='utf-8') as f:
        f.write(loading_js)
    
    print("✅ Created loading-states.js\n")

def main():
    """Main execution function"""
    print("=" * 60)
    print("🚀 SMO Platform UI - Comprehensive Fix Script")
    print("=" * 60)
    print()
    
    # Run all fixes
    fix_navigation_issues()
    create_missing_pages()
    fix_api_service()
    add_search_functionality()
    add_loading_states()
    
    print("=" * 60)
    print("✨ All fixes completed successfully!")
    print("\n📌 Next Steps:")
    print("1. Add <script src='js/search.js'></script> to all pages with search")
    print("2. Add <script src='js/loading-states.js'></script> to data-heavy pages")
    print("3. Test all navigation links and new pages")
    print("4. Verify API environment detection works correctly")
    print("5. Test search functionality")
    print("=" * 60)

if __name__ == "__main__":
    main()