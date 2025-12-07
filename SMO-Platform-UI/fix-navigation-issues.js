/**
 * Script to Fix Navigation Issues in SMO Platform UI
 * This script addresses the critical navigation bugs found during testing
 */

const fs = require('fs');
const path = require('path');

// List of files that need logo href fixes
const filesToFix = [
    'login.html',
    'achievements.html',
    'performance-dashboard.html',
    'vision.html',
    'indicators.html',
    'audit-logs.html',
    'data-sync-status.html',
    'executive-performance-dashboard.html',
    'initiative-details.html',
    'milestone-tracking.html',
    'performance-approvals.html',
    'performance-initiatives.html',
    'performance-programs.html',
    'performance-requests.html',
    'performance-thresholds.html',
    'performance-vision.html',
    'program-wizard.html',
    'report-generation.html',
    'risk-escalation.html'
];

// Function to fix empty href attributes
function fixEmptyHrefs(filePath) {
    try {
        let content = fs.readFileSync(filePath, 'utf8');
        let changesMade = false;
        
        // Fix empty logo href attributes
        // Pattern 1: <a class="page-header__logo" href="" title="الرئيسية">
        if (content.includes('href="" title="الرئيسية"')) {
            content = content.replace(/href="" title="الرئيسية"/g, 'href="index.html" title="الرئيسية"');
            changesMade = true;
            console.log(`✅ Fixed logo link in ${path.basename(filePath)}`);
        }
        
        // Pattern 2: <a href="" class="login-page__logo">
        if (content.includes('href="" class="login-page__logo"')) {
            content = content.replace(/href="" class="login-page__logo"/g, 'href="index.html" class="login-page__logo"');
            changesMade = true;
            console.log(`✅ Fixed login page logo link in ${path.basename(filePath)}`);
        }
        
        // Fix breadcrumb home links
        // Pattern: <a href="#"><img src="images/home-icon.svg"
        if (content.includes('<a href="#"><img src="images/home-icon.svg"')) {
            content = content.replace(/<a href="#">(<img src="images\/home-icon\.svg"[^>]*>)/g, '<a href="index.html">$1');
            changesMade = true;
            console.log(`✅ Fixed breadcrumb home link in ${path.basename(filePath)}`);
        }
        
        // Fix user dropdown empty links
        // Pattern: <a class="user-dropdown__actions__item" href="">
        const userDropdownPattern = /<a class="user-dropdown__actions__item" href="">([^<]*<span[^>]*>[^<]*<\/span>[^<]*<span[^>]*>الملف الشخصي<\/span>)/g;
        if (userDropdownPattern.test(content)) {
            content = content.replace(userDropdownPattern, '<a class="user-dropdown__actions__item" href="profile.html">$1');
            changesMade = true;
            console.log(`✅ Fixed user profile link in ${path.basename(filePath)}`);
        }
        
        // Fix logout links
        const logoutPattern = /<a class="user-dropdown__actions__item" href="">([^<]*<span[^>]*>[^<]*<\/span>[^<]*<span[^>]*>تسجيل الخروج<\/span>)/g;
        if (logoutPattern.test(content)) {
            content = content.replace(logoutPattern, '<a class="user-dropdown__actions__item" href="login.html" onclick="logout()">$1');
            changesMade = true;
            console.log(`✅ Fixed logout link in ${path.basename(filePath)}`);
        }
        
        // Fix settings links
        const settingsPattern = /<a class="user-dropdown__actions__item" href="">([^<]*<span[^>]*>[^<]*<\/span>[^<]*<span[^>]*>إعداداتي<\/span>)/g;
        if (settingsPattern.test(content)) {
            content = content.replace(settingsPattern, '<a class="user-dropdown__actions__item" href="settings.html">$1');
            changesMade = true;
            console.log(`✅ Fixed settings link in ${path.basename(filePath)}`);
        }
        
        // Write the fixed content back to file
        if (changesMade) {
            fs.writeFileSync(filePath, content, 'utf8');
            return true;
        }
        return false;
        
    } catch (error) {
        console.error(`❌ Error processing ${filePath}:`, error.message);
        return false;
    }
}

// Add logout function to handle logout properly
const logoutFunction = `
<!-- Add this script before closing body tag if not already present -->
<script>
function logout() {
    // Clear any stored tokens
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
    sessionStorage.clear();
    
    // Redirect to login page
    window.location.href = 'login.html';
    return false;
}
</script>
`;

// Main execution
console.log('🔧 Starting Navigation Fix Script...\n');
console.log('📋 Files to process:', filesToFix.length);
console.log('=' .repeat(50));

let fixedCount = 0;
let errorCount = 0;

filesToFix.forEach(file => {
    const filePath = path.join(__dirname, file);
    if (fs.existsSync(filePath)) {
        if (fixEmptyHrefs(filePath)) {
            fixedCount++;
        }
    } else {
        console.log(`⚠️  File not found: ${file}`);
        errorCount++;
    }
});

console.log('=' .repeat(50));
console.log(`\n✅ Fixed ${fixedCount} files`);
if (errorCount > 0) {
    console.log(`⚠️  ${errorCount} files were not found`);
}

// Create a proper logout handler file
const logoutHandlerContent = `/**
 * Logout Handler for SMO Platform
 */

function logout() {
    // Clear authentication data
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
    sessionStorage.clear();
    
    // Call API logout if available
    if (window.apiService && window.apiService.logout) {
        window.apiService.logout().catch(err => {
            console.log('Logout API call failed, redirecting anyway');
        });
    }
    
    // Redirect to login page
    window.location.href = 'login.html';
    return false;
}

// Auto-attach to any logout links
document.addEventListener('DOMContentLoaded', function() {
    const logoutLinks = document.querySelectorAll('a[href="login.html"][onclick*="logout"]');
    logoutLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            logout();
        });
    });
});
`;

fs.writeFileSync(path.join(__dirname, 'js', 'logout-handler.js'), logoutHandlerContent);
console.log('\n✅ Created logout-handler.js');

console.log('\n📌 Next Steps:');
console.log('1. Review the changes made by this script');
console.log('2. Add <script src="js/logout-handler.js"></script> to all pages');
console.log('3. Create profile.html and settings.html pages if they don\'t exist');
console.log('4. Test all navigation links');
console.log('\n✨ Navigation fix script completed!');