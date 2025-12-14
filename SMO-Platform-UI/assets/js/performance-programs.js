/**
 * Performance Programs - Page Specific JavaScript
 */

// Initialize API Connection and Load Data
document.addEventListener('DOMContentLoaded', async function() {
    if (window.location.pathname.includes('login.html')) return;
    
    try {
        const isHealthy = await window.apiService.checkHealth();
        if (isHealthy) {
            console.log('Backend API is connected and healthy');
            loadProgramsFromAPI();
        } else {
            console.warn('Backend API is not available, using static data');
        }
    } catch (error) {
        console.error('API connection error:', error);
    }
    
    const form = document.getElementById('reportGenerationForm');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            const format = document.getElementById('reportFormat').value;
            if(format === 'html') previewHTMLReport();
            else alert(`جاري توليد التقرير بصيغة ${format}...`);
        });
    }
    
    const searchInput = document.getElementById('searchPrograms');
    if (searchInput) {
        searchInput.addEventListener('keyup', filterPrograms);
    }
});

async function loadProgramsFromAPI() {
    try {
        const programs = await window.apiService.getPrograms();
        console.log('Programs loaded from API:', programs);
    } catch (error) {
        console.error('Error loading programs:', error);
    }
}

function handleProgramFormSubmit(formData) {
    window.apiService.createProgram(formData)
        .then(response => {
            showNotification('success', 'نجح الإنشاء', 'تم إنشاء البرنامج بنجاح');
            window.location.reload();
        })
        .catch(error => {
            console.error('Error creating program:', error);
            showNotification('error', 'فشل الإنشاء', 'حدث خطأ في إنشاء البرنامج. يرجى المحاولة مرة أخرى.');
        });
}

function generateReport(programId, quarter) {
    window.location.href = `report-generation.html?type=program&id=${programId}&quarter=${quarter}`;
}

// Notification helper
function showNotification(type, title, message) {
    if (type === 'success' && typeof showSuccess !== 'undefined') {
        showSuccess(`${title}: ${message}`);
        return;
    }
    
    const alertClass = type === 'success' ? 'alert-success' : type === 'warning' ? 'alert-warning' : type === 'danger' ? 'alert-danger' : 'alert-info';
    const alertHtml = `<div class="alert ${alertClass} alert-dismissible fade show position-fixed top-0 end-0 m-3" style="z-index: 9999; min-width: 300px;"><strong>${title}</strong><br>${message}<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>`;
    
    const alertElement = document.createElement('div');
    alertElement.innerHTML = alertHtml;
    document.body.appendChild(alertElement.firstElementChild);
    
    setTimeout(() => {
        const alert = document.querySelector('.alert.position-fixed');
        if (alert) alert.remove();
    }, 5000);
}

function exportPrograms() {
    console.log('تصدير بيانات البرامج إلى Excel');
    showNotification('info', 'جاري التحضير', 'جاري تحضير ملف التصدير...');
    setTimeout(() => showNotification('success', 'نجح التصدير', 'تم تصدير البيانات بنجاح'), 2000);
}

function addDetailedKPI() {
    const table = document.getElementById('detailedKPITableBody');
    const newRow = table.insertRow();
    newRow.innerHTML = `
        <td><input type="text" class="form-control form-control-sm" placeholder="مثال: KPI-001"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="أدخل اسم مؤشر الأداء الرئيسي"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="Enter KPI Name"></td>
        <td><select class="form-select form-select-sm"><option>متزايد</option><option>متناقص</option></select></td>
        <td><select class="form-select form-select-sm"><option>رئيسي</option><option>ثانوي</option></select></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="وحدة القياس"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="القيمة المرجعية"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="سنة الأساس" value="2024"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="الهدف المخطط"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="القيمة المحققة"></td>
        <td><select class="form-select form-select-sm"><option class="text-success">أخضر</option><option class="text-warning">أصفر</option><option class="text-danger">أحمر</option></select></td>
    `;
}

function addKeyInitiative() {
    const table = document.getElementById('keyInitiativesTableBody');
    const newRow = table.insertRow();
    newRow.innerHTML = `
        <td><input type="text" class="form-control form-control-sm" placeholder="رمز المبادرة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="أدخل اسم المبادرة بالعربية"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="Enter Initiative Name"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="وصف مختصر"></td>
        <td><input type="number" class="form-control form-control-sm" min="0" max="100"></td>
        <td><input type="number" class="form-control form-control-sm" min="0" max="100"></td>
        <td><select class="form-select form-select-sm"><option>على المسار</option><option>متأخر</option><option>مكتمل</option></select></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الميزانية المعتمدة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="المبلغ المصروف"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="المعالم الرئيسية"></td>
    `;
}

function loadAllInitiatives() {
    const tbody = document.getElementById('allInitiativesTableBody');
    tbody.innerHTML = '';
    const initiatives = [
        {name: 'مبادرة التحول الرقمي', status: 'على المسار', progress: 85, budget: '75%'},
        {name: 'مبادرة تطوير الكوادر', status: 'متأخر', progress: 60, budget: '50%'},
        {name: 'مبادرة الاستدامة', status: 'مكتمل', progress: 100, budget: '95%'}
    ];
    initiatives.forEach(init => {
        const row = tbody.insertRow();
        row.innerHTML = `<td>${init.name}</td><td>${init.status}</td><td>${init.progress}%</td><td>${init.budget}</td>`;
    });
}

function addKPIAchievement() {
    const tbody = document.getElementById('kpiAchievementsTableBody');
    const row = tbody.insertRow();
    row.innerHTML = `
        <td><input type="text" class="form-control form-control-sm" placeholder="اسم البرنامج"></td>
        <td><select class="form-select form-select-sm"><option>إنجاز متميز</option><option>إنجاز مهم</option><option>إنجاز عادي</option></select></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="عنوان الإنجاز"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="وصف تفصيلي"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="السنة" value="2024"></td>
        <td><select class="form-select form-select-sm"><option>Q1</option><option>Q2</option><option>Q3</option><option>Q4</option></select></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="اسم المؤشر"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الهدف"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الوحدة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="المحقق"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="المبادرة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الهدف الاستراتيجي"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الجهات"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الشريحة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الأثر"></td>
    `;
}

function addInitiativeAchievement() {
    const tbody = document.getElementById('initiativeAchievementsTableBody');
    const row = tbody.insertRow();
    row.innerHTML = `
        <td><input type="text" class="form-control form-control-sm" placeholder="المبادرة"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="العنوان"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الوصف"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الأثر"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الشريحة المستهدفة"></td>
    `;
}

function addKPIRow() {
    const table = document.getElementById('reportKPITable').getElementsByTagName('tbody')[0];
    const newRow = table.insertRow();
    newRow.innerHTML = `
        <td><input type="text" class="form-control form-control-sm" placeholder="KPI-XXX"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="اسم المؤشر"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="الوحدة"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="0"></td>
        <td><input type="number" class="form-control form-control-sm" placeholder="0"></td>
        <td><input type="text" class="form-control form-control-sm" placeholder="0%" readonly></td>
        <td><select class="form-select form-select-sm"><option value="green">أخضر</option><option value="yellow">أصفر</option><option value="red">أحمر</option></select></td>
    `;
}

function previewHTMLReport() {
    const htmlContent = generateHTMLReportContent();
    document.getElementById('htmlReportContent').innerHTML = htmlContent;
    const modal = new bootstrap.Modal(document.getElementById('htmlReportModal'));
    modal.show();
}

function generateHTMLReportContent() {
    const reportType = document.getElementById('reportType').value;
    const program = document.getElementById('reportProgram').value;
    const year = document.getElementById('reportYear').value;
    const period = document.getElementById('reportPeriod').value;
    const executiveSummary = document.getElementById('executiveSummary').value;
    const keyAchievements = document.getElementById('keyAchievements').value;
    const mainChallenges = document.getElementById('mainChallenges').value;
    const recommendations = document.getElementById('strategicRecommendations').value;
    
    const overallPerformance = document.getElementById('overallPerformance').value;
    const achievedKPIs = document.getElementById('achievedKPIs').value;
    const completedInitiatives = document.getElementById('completedInitiatives').value;
    const financialExecution = document.getElementById('financialExecution').value;
    
    const approvedBudget = document.getElementById('approvedBudget').value;
    const actualSpent = document.getElementById('actualSpent').value;
    const commitments = document.getElementById('commitments').value;
    const savings = document.getElementById('savings').value;
    
    const criticalRisks = document.getElementById('criticalRisks').value;
    const highRisks = document.getElementById('highRisks').value;
    const mediumRisks = document.getElementById('mediumRisks').value;
    const lowRisks = document.getElementById('lowRisks').value;
    
    return `<!DOCTYPE html><html dir="rtl" lang="ar"><head><meta charset="UTF-8"><title>تقرير الأداء - ${program} - ${period} ${year}</title><style>body{font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;direction:rtl;padding:20px;max-width:1200px;margin:0 auto;}.header{text-align:center;border-bottom:3px solid #1e5288;padding-bottom:20px;margin-bottom:30px;}h1{color:#1e5288;margin:10px 0;}h2{background:#f0f4f8;padding:10px;color:#1e5288;border-right:5px solid #1e5288;}.section{margin-bottom:30px;}.metric-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:20px;margin:20px 0;}.metric-card{color:white;padding:20px;border-radius:10px;text-align:center;}.metric-card h3{margin:0;font-size:2em;}.metric-card p{margin:5px 0 0 0;opacity:0.9;}table{width:100%;border-collapse:collapse;margin:20px 0;}th,td{border:1px solid #ddd;padding:12px;text-align:right;}th{background:#f8f9fa;color:#1e5288;font-weight:bold;}.footer{margin-top:50px;padding-top:20px;border-top:2px solid #ddd;text-align:center;color:#666;}@media print{.metric-card{break-inside:avoid;}table{page-break-inside:auto;}tr{page-break-inside:avoid;}}</style></head><body><div class="header"><h1>مكتب الإدارة الاستراتيجية</h1><h2 style="background:none;border:none;">تقرير ${reportType === 'quarterly' ? 'ربعي' : reportType === 'annual' ? 'سنوي' : 'الأداء'}</h2><p><strong>البرنامج:</strong> ${program} | <strong>الفترة:</strong> ${period} ${year}</p><p><strong>تاريخ التقرير:</strong> ${new Date().toLocaleDateString('ar-SA')}</p></div><div class="section"><h2>الملخص التنفيذي</h2><p>${executiveSummary || 'لم يتم إدخال ملخص تنفيذي'}</p></div><div class="section"><h2>مؤشرات الأداء الرئيسية</h2><div class="metric-grid"><div class="metric-card" style="background:linear-gradient(135deg,#667eea 0%,#764ba2 100%);"><h3>${overallPerformance}%</h3><p>الأداء العام</p></div><div class="metric-card" style="background:linear-gradient(135deg,#f093fb 0%,#f5576c 100%);"><h3>${achievedKPIs}</h3><p>المؤشرات المحققة</p></div><div class="metric-card" style="background:linear-gradient(135deg,#4facfe 0%,#00f2fe 100%);"><h3>${completedInitiatives}</h3><p>المبادرات المكتملة</p></div><div class="metric-card" style="background:linear-gradient(135deg,#43e97b 0%,#38f9d7 100%);"><h3>${financialExecution}%</h3><p>نسبة الصرف</p></div></div></div><div class="section"><h2>تفصيل مؤشرات الأداء</h2><table><thead><tr><th>المؤشر</th><th>المستهدف</th><th>المحقق</th><th>نسبة الإنجاز</th><th>الحالة</th></tr></thead><tbody>${getKPITableRows()}</tbody></table></div><div class="section"><h2>تحليل الميزانية</h2><table><tr><th>البند</th><th>القيمة (مليون ريال)</th></tr><tr><td>الميزانية المعتمدة</td><td>${approvedBudget}</td></tr><tr><td>المنصرف الفعلي</td><td>${actualSpent}</td></tr><tr><td>الارتباطات</td><td>${commitments}</td></tr><tr><td>الوفورات المحققة</td><td>${savings}</td></tr></table></div><div class="section"><h2>تقييم المخاطر</h2><table><tr><th>مستوى الخطر</th><th>العدد</th></tr><tr style="background-color:#ffebee;"><td>مخاطر حرجة</td><td>${criticalRisks}</td></tr><tr style="background-color:#fff3e0;"><td>مخاطر عالية</td><td>${highRisks}</td></tr><tr style="background-color:#fff9c4;"><td>مخاطر متوسطة</td><td>${mediumRisks}</td></tr><tr style="background-color:#f1f8e9;"><td>مخاطر منخفضة</td><td>${lowRisks}</td></tr></table></div><div class="section"><h2>أهم الإنجازات</h2><div style="white-space:pre-line;">${keyAchievements || 'لم يتم إدخال إنجازات'}</div></div><div class="section"><h2>التحديات الرئيسية</h2><div style="white-space:pre-line;">${mainChallenges || 'لم يتم إدخال تحديات'}</div></div><div class="section"><h2>التوصيات</h2><div style="white-space:pre-line;">${recommendations || 'لم يتم إدخال توصيات'}</div></div><div class="footer"><p>تم إعداد هذا التقرير بواسطة نظام إدارة الأداء - مكتب الإدارة الاستراتيجية</p><p>© ${new Date().getFullYear()} جميع الحقوق محفوظة</p></div></body></html>`;
}

function getKPITableRows() {
    const tbody = document.querySelector('#reportKPITable tbody');
    let rows = '';
    for(let row of tbody.rows) {
        rows += '<tr>';
        for(let cell of row.cells) {
            rows += `<td>${cell.innerHTML}</td>`;
        }
        rows += '</tr>';
    }
    return rows;
}

function printHTMLReport() {
    const content = document.getElementById('htmlReportContent').innerHTML;
    const printWindow = window.open('', '_blank');
    printWindow.document.write(content);
    printWindow.document.close();
    printWindow.print();
}

function downloadHTMLReport() {
    const content = generateHTMLReportContent();
    const blob = new Blob([content], { type: 'text/html;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `report_${Date.now()}.html`;
    a.click();
    URL.revokeObjectURL(url);
}

function saveReportDraft() {
    const reportData = {
        type: document.getElementById('reportType').value,
        program: document.getElementById('reportProgram').value,
        year: document.getElementById('reportYear').value,
        period: document.getElementById('reportPeriod').value,
        executiveSummary: document.getElementById('executiveSummary').value,
        keyAchievements: document.getElementById('keyAchievements').value,
        mainChallenges: document.getElementById('mainChallenges').value,
        recommendations: document.getElementById('strategicRecommendations').value,
        savedAt: new Date().toISOString()
    };
    localStorage.setItem('reportDraft', JSON.stringify(reportData));
    alert('تم حفظ المسودة بنجاح');
}

function scheduleReport() {
    alert('سيتم جدولة التقرير للإرسال التلقائي');
}

function sendToManagement() {
    if(confirm('هل أنت متأكد من إرسال التقرير للإدارة العليا؟')) {
        alert('تم إرسال التقرير بنجاح');
    }
}

// Program Management
let currentProgramId = null;
let currentReportFormat = null;
let currentExportType = null;

function createReport(programId, format) {
    currentProgramId = programId;
    currentReportFormat = format;
    
    const progressModal = new bootstrap.Modal(document.getElementById('reportProgressModal'));
    progressModal.show();
    
    generateReportAsync(programId, format);
}

async function generateReportAsync(programId, format) {
    try {
        let progress = 0;
        const progressBar = document.getElementById('reportProgress');
        
        const progressInterval = setInterval(() => {
            progress += Math.random() * 20;
            if (progress > 90) progress = 90;
            progressBar.style.width = progress + '%';
        }, 500);
        
        const response = await window.apiService.fetchWithAuth(
            `/api/programs/${programId}/reports/generate`,
            { method: 'POST', body: JSON.stringify({ format: format, period: 'Q4-2024', type: 'quarterly' }) }
        );
        
        clearInterval(progressInterval);
        progressBar.style.width = '100%';
        
        setTimeout(() => {
            bootstrap.Modal.getInstance(document.getElementById('reportProgressModal')).hide();
            showNotification('success', 'تم إنشاء التقرير بنجاح', 'التقرير جاهز للتحميل. الرابط صالح لمدة 30 يوماً.');
            downloadReport(`/reports/${programId}_Q4-2024.${format}`, programId, format);
        }, 1500);
        
    } catch (error) {
        console.error('Report generation error:', error);
        bootstrap.Modal.getInstance(document.getElementById('reportProgressModal')).hide();
        showNotification('danger', 'فشل إنشاء التقرير', 'يرجى المحاولة لاحقاً');
    }
}

async function sendReport(programId) {
    currentProgramId = programId;
    
    const isDataComplete = await checkDataCompleteness(programId);
    
    const modal = new bootstrap.Modal(document.getElementById('sendReportModal'));
    const statusDiv = document.getElementById('dataCompletionStatus');
    const sendBtn = document.getElementById('sendReportBtn');
    const checkbox = document.getElementById('dataApprovalCheck');
    
    if (isDataComplete) {
        statusDiv.innerHTML = `<div class="alert alert-success"><i class="bi bi-check-circle"></i><strong>البيانات مكتملة</strong><br>الملخص التنفيذي للمؤشرات والمبادرات مكتمل للربع الحالي.</div>`;
        checkbox.addEventListener('change', function() {
            sendBtn.disabled = !this.checked;
        });
    } else {
        statusDiv.innerHTML = `<div class="alert alert-danger"><i class="bi bi-exclamation-triangle"></i><strong>البيانات غير مكتملة</strong><br>رجاء قم بإدخال بيانات الملخص التنفيذي للمؤشرات والمبادرات للربع الحالي قبل إرسال التقرير.<br><br><a href="program-wizard.html?id=${programId}&section=executive-summary" class="btn btn-sm btn-primary">إكمال البيانات الناقصة</a></div>`;
        sendBtn.disabled = true;
    }
    
    modal.show();
}

async function checkDataCompleteness(programId) {
    try {
        const response = await window.apiService.fetchWithAuth(`/api/programs/${programId}/data-status?period=Q4-2024`);
        if (response.ok) {
            const status = await response.json();
            return status.indicatorsSummary && status.initiativesSummary;
        }
    } catch (error) {
        console.error('Error checking data completeness:', error);
        return false;
    }
    return false;
}

async function confirmSendReport() {
    if (!currentProgramId) return;
    
    try {
        const existingReport = await checkExistingReport(currentProgramId, 'Q4-2024');
        
        if (existingReport) {
            showNotification('warning', 'التقرير مُرسَل بالفعل', 'يجب إلغاء التقرير السابق قبل إرسال تقرير جديد للفترة نفسها');
            return;
        }
        
        const response = await window.apiService.fetchWithAuth(
            `/api/programs/${currentProgramId}/reports/submit`,
            { method: 'POST', body: JSON.stringify({ period: 'Q4-2024', approval: true, timestamp: new Date().toISOString() }) }
        );
        
        bootstrap.Modal.getInstance(document.getElementById('sendReportModal')).hide();
        showNotification('success', 'تم إرسال التقرير', 'تم إرسال التقرير لدورة الموافقات. الحالة: قيد المراجعة');
            
    } catch (error) {
        console.error('Error sending report:', error);
        showNotification('danger', 'فشل إرسال التقرير', 'يرجى المحاولة مرة أخرى');
    }
}

async function checkExistingReport(programId, period) {
    try {
        const response = await window.apiService.fetchWithAuth(`/api/programs/${programId}/reports/check?period=${period}`);
        if (response.ok) {
            const result = await response.json();
            return result.exists;
        }
    } catch (error) {
        console.error('Error checking existing report:', error);
    }
    return false;
}

function exportToExcel(programId, type) {
    currentProgramId = programId;
    currentExportType = type;
    executeExport(type);
}

async function executeExport(type) {
    try {
        showNotification('info', 'جاري التصدير', `جاري تصدير ${getExportTypeName(type)}...`);
        
        const response = await window.apiService.fetchWithAuth(
            `/api/programs/${currentProgramId || 'FSDP-2024'}/export`,
            { method: 'POST', body: JSON.stringify({ type: type, format: 'xlsx', period: 'Q4-2024' }) }
        );
        
        if (response.ok) {
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `${currentProgramId || 'program'}_${type}_Q4-2024.xlsx`;
            document.body.appendChild(link);
            link.click();
            link.remove();
            
            showNotification('success', 'تم التصدير بنجاح', `تم تصدير ${getExportTypeName(type)} إلى Excel`);
        }
    } catch (error) {
        console.error('Export error:', error);
        showNotification('error', 'خطأ في التصدير', 'حدث خطأ في تصدير البيانات. يرجى المحاولة مرة أخرى.');
    }
}

function getExportTypeName(type) {
    const types = {
        'indicators': 'مؤشرات البرنامج',
        'initiatives': 'مستوى المبادرات',
        'achievements': 'أبرز إنجازات البرنامج',
        'risks': 'المخاطر',
        'support': 'طلبات دعم البرنامج'
    };
    return types[type] || type;
}

function downloadReport(url, programId, format) {
    const link = document.createElement('a');
    link.href = url;
    link.download = `report_${programId}_Q4-2024.${format}`;
    link.click();
}

function filterPrograms() {
    const search = document.getElementById('searchPrograms').value.toLowerCase();
    const status = document.getElementById('filterStatus').value;
    const quarter = document.getElementById('filterQuarter').value;
    const performance = document.getElementById('filterPerformance').value;
    
    const cards = document.querySelectorAll('.col-lg-6.col-xl-4');
    cards.forEach(card => {
        const title = card.querySelector('.card-title')?.textContent.toLowerCase() || '';
        const cardStatus = card.querySelector('.badge')?.textContent || '';
        const cardPerformance = parseFloat(card.querySelector('.progress-bar')?.style.width || '0');
        
        let show = true;
        
        if (search && !title.includes(search)) show = false;
        if (status && cardStatus !== status) show = false;
        
        if (performance) {
            if (performance.includes('>90') && cardPerformance <= 90) show = false;
            if (performance.includes('70-90') && (cardPerformance < 70 || cardPerformance > 90)) show = false;
            if (performance.includes('<70') && cardPerformance >= 70) show = false;
            if (performance.includes('<50') && cardPerformance >= 50) show = false;
        }
        
        card.style.display = show ? '' : 'none';
    });
}

function clearFilters() {
    document.getElementById('searchPrograms').value = '';
    document.getElementById('filterStatus').value = '';
    document.getElementById('filterQuarter').value = 'Q4 2024';
    document.getElementById('filterPerformance').value = '';
    
    const cards = document.querySelectorAll('.col-lg-6.col-xl-4');
    cards.forEach(card => {
        card.style.display = '';
    });
}

function checkPeriodValidity() {
    const currentDate = new Date();
    const periodStartDate = new Date('2024-10-01');
    
    if (currentDate < periodStartDate) {
        document.querySelectorAll('button[onclick^="sendReport"]').forEach(btn => {
            btn.disabled = true;
            btn.title = 'الفترة الحالية لم تبدأ بعد';
        });
    }
}

checkPeriodValidity();


