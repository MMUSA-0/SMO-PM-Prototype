/**
 * Script to add development indicators to all existing pages
 * Run this with Node.js to update all HTML files
 */

const fs = require('fs');
const path = require('path');

// Configuration for pages and their development status
const pageStatus = {
    // Pages with incomplete features  
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
    },
    'indicators.html': {
        status: 'partial',
        progress: 65,
        missingFeatures: [
            'استيراد المؤشرات',
            'التصدير إلى Excel',
            'التحليلات المتقدمة'
        ]
    },
    'initiative-details.html': {
        status: 'partial',
        progress: 70,
        missingFeatures: [
            'المرفقات والوثائق',
            'سجل التغييرات',
            'التعليقات والملاحظات'
        ]
    },
    'performance-thresholds.html': {
        status: 'partial',
        progress: 45,
        missingFeatures: [
            'إعدادات الحدود المخصصة',
            'التنبيهات التلقائية',
            'التقارير التحليلية'
        ]
    },
    'performance-requests.html': {
        status: 'partial',
        progress: 60,
        missingFeatures: [
            'تتبع حالة الطلبات',
            'الموافقات المتعددة',
            'قوالب الطلبات'
        ]
    },
    'data-sync-status.html': {
        status: 'partial',
        progress: 50,
        missingFeatures: [
            'المزامنة التلقائية',
            'سجل المزامنة التفصيلي',
            'إعادة المحاولة التلقائية'
        ]
    }
};

// Function to add dev indicators to HTML file
function addDevIndicators(filePath) {
    const fileName = path.basename(filePath);
    const isUnderDevelopment = pageStatus.hasOwnProperty(fileName);
    
    if (!isUnderDevelopment) {
        console.log(`✓ ${fileName} - No development indicators needed`);
        return;
    }
    
    let html = fs.readFileSync(filePath, 'utf8');
    
    // Check if dev indicators are already added
    if (html.includes('dev-indicators.css') && html.includes('dev-indicators.js')) {
        console.log(`✓ ${fileName} - Already has development indicators`);
        return;
    }
    
    // Add CSS link if not present
    if (!html.includes('dev-indicators.css')) {
        const cssLink = '    <link href="assets/css/dev-indicators.css" rel="stylesheet">\n';
        
        // Add after the last CSS link in head
        const lastCssIndex = html.lastIndexOf('</head>');
        if (lastCssIndex !== -1) {
            html = html.slice(0, lastCssIndex) + cssLink + html.slice(lastCssIndex);
        }
    }
    
    // Add JS script if not present
    if (!html.includes('dev-indicators.js')) {
        const jsScript = '    <script src="assets/js/dev-indicators.js"></script>\n';
        
        // Add before closing body tag
        const bodyCloseIndex = html.lastIndexOf('</body>');
        if (bodyCloseIndex !== -1) {
            html = html.slice(0, bodyCloseIndex) + jsScript + html.slice(bodyCloseIndex);
        }
    }
    
    // Add data attribute to body for page status
    const pageInfo = pageStatus[fileName];
    if (pageInfo) {
        html = html.replace('<body>', 
            `<body data-dev-status="${pageInfo.status}" data-dev-progress="${pageInfo.progress}">`);
        
        // Also handle body with existing attributes
        html = html.replace(/<body\s+([^>]+)>/gi, (match, attrs) => {
            if (!attrs.includes('data-dev-status')) {
                return `<body ${attrs} data-dev-status="${pageInfo.status}" data-dev-progress="${pageInfo.progress}">`;
            }
            return match;
        });
    }
    
    // Save the updated file
    fs.writeFileSync(filePath, html, 'utf8');
    console.log(`✅ ${fileName} - Added development indicators (${pageInfo.progress}% complete)`);
}

// Process all HTML files
function processAllHTMLFiles() {
    const directory = __dirname;
    const files = fs.readdirSync(directory);
    
    console.log('Adding development indicators to pages...\n');
    
    files.forEach(file => {
        if (file.endsWith('.html') && !file.includes('template') && !file.includes('START-HERE')) {
            const filePath = path.join(directory, file);
            addDevIndicators(filePath);
        }
    });
    
    console.log('\n✅ Development indicators added successfully!');
    console.log('\nPages with development status:');
    Object.entries(pageStatus).forEach(([page, info]) => {
        console.log(`  - ${page}: ${info.progress}% complete`);
    });
}

// Run the script
processAllHTMLFiles();

