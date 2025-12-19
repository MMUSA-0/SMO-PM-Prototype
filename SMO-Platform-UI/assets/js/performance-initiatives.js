/**
 * Performance Initiatives - Page Specific JavaScript
 * Requires: shared-utilities.js (formatCurrency, showNotification, initializeTooltips)
 */

// ============= Initiative Management =============
let selectedInitiative = null;

// Complete initiative data structure as per BRD (UC-14 to UC-18)
const initiativesData = {
    '1-18-139-1377': {
        // Basic Information (UC-14)
        id: '1-18-139-1377',
        name: 'تطوير منصة رقمية للخدمات المالية',
        nameEn: 'Digital Financial Services Platform',
        program: 'برنامج تطوير القطاع المالي',
        responsibleEntity: 'البنك المركزي السعودي',
        status: 'قيد التنفيذ',
        isPivotal: true,  // مبادرة محورية
        
        // Dates and Timeline
        startDate: '2024-01-01',
        endDate: '2025-12-31',
        
        // Progress Metrics
        plannedProgress: 85,
        actualProgress: 82,
        progress: 82, // Keep for backward compatibility
        scheduleDeviation: -3, // انحراف الجدول الزمني (أيام)
        costDeviation: -2.5,  // انحراف التكلفة (مليون ريال)
        
        // Budget Information (UC-16)
        totalBudget: 1250,  // إجمالي الميزانية (مليون ريال)
        allocatedToDate: 850, // الميزانية المصروفة حتى تاريخه
        spentToDate: 782,  // المنصرف الفعلي
        allocatedQuarterly: 200,  // الميزانية المصروفة ربع سنوياً
        spentQuarterly: 180,  // المنفقة ربع سنوياً
        
        // Description and Performance Notes
        description: 'تطوير منصة رقمية متكاملة لتقديم الخدمات المالية الرقمية للعملاء', // الوصف
        notes: 'تم صرف كامل الميزانية المعتمدة على المبادرة لعام 2024',
        performanceOverview: 'المبادرة تسير وفق الخطة المعتمدة مع تحقيق معظم المعالم الرئيسية',
        
        // Counts for quick reference
        kpiCount: 5,
        milestoneCount: 8,
        riskCount: 2,
        documentCount: 12
    },
    '1-18-140-1378': {
        // Basic Information
        id: '1-18-140-1378',
        name: 'برنامج التوعية المالية للشباب',
        nameEn: 'Youth Financial Literacy Program',
        program: 'برنامج تطوير القطاع المالي',
        responsibleEntity: 'وزارة التعليم',
        status: 'متأخرة',
        isPivotal: false,
        
        // Dates and Timeline
        startDate: '2024-03-01',
        endDate: '2026-02-28',
        
        // Progress Metrics
        plannedProgress: 60,
        actualProgress: 45,
        progress: 45, // Keep for backward compatibility
        scheduleDeviation: -15,  // متأخر 15 يوم
        costDeviation: 5.2,  // تجاوز 5.2 مليون
        
        // Budget Information
        totalBudget: 450,
        allocatedToDate: 280,
        spentToDate: 285.2,  // تجاوز
        allocatedQuarterly: 50,
        spentQuarterly: 55,
        
        // Description and Performance Notes
        description: 'برنامج شامل لتعزيز الوعي المالي لدى الشباب وتمكينهم من اتخاذ قرارات مالية مدروسة', // الوصف
        notes: 'تحديات في التنسيق مع المدارس والجامعات',
        performanceOverview: 'المبادرة تواجه تحديات في الوصول للفئة المستهدفة',
        
        // Counts
        kpiCount: 3,
        milestoneCount: 6,
        riskCount: 4,
        documentCount: 8
    }
};

const kpiMockData = {
    '1-18-139-1377': [
        { code: 'KPI-INI-001', name: 'نسبة الإنجاز في المبادرة', nameEn: 'Initiative Completion Rate', unit: '%', baseline: 0, target: 100, actual: 82, frequency: 'quarterly', source: 'إدارة الأداء', lastUpdate: '2024-11-30', status: 'near', drivers: ['التزام الفريق القوي', 'توفر الموارد المطلوبة'], obstacles: ['تأخر بعض الموافقات'], notes: 'الأداء جيد ومتوافق مع الخطة المعتمدة' },
        { code: 'KPI-INI-002', name: 'عدد المستفيدين', nameEn: 'Number of Beneficiaries', unit: 'مستفيد', baseline: 0, target: 10000, actual: 8500, frequency: 'monthly', source: 'قاعدة بيانات المستفيدين', lastUpdate: '2024-11-30', status: 'achieved', drivers: ['حملة التوعية الناجحة'], obstacles: [], notes: 'تجاوزنا التوقعات للربع الحالي' },
        { code: 'KPI-INI-003', name: 'معدل رضا المستفيدين', nameEn: 'Beneficiary Satisfaction Rate', unit: '%', baseline: 70, target: 90, actual: 75, frequency: 'quarterly', source: 'استبيان رضا المستفيدين', lastUpdate: '2024-11-30', status: 'behind', drivers: [], obstacles: ['الحاجة لتحسين جودة الخدمات', 'قلة التدريب للموظفين'], notes: 'يتطلب تدخل عاجل لتحسين مستوى الرضا' },
        { code: 'KPI-INI-004', name: 'نسبة الصرف من الميزانية', nameEn: 'Budget Utilization Rate', unit: '%', baseline: 0, target: 95, actual: 88, frequency: 'monthly', source: 'الإدارة المالية', lastUpdate: '2024-11-30', status: 'near', drivers: ['الإدارة الفعالة للموارد'], obstacles: ['تأخر بعض المشتريات'], notes: 'الصرف يسير وفق الخطة المالية' },
        { code: 'KPI-INI-005', name: 'عدد الشراكات المفعلة', nameEn: 'Number of Active Partnerships', unit: 'شراكة', baseline: 2, target: 8, actual: 6, frequency: 'quarterly', source: 'إدارة الشراكات', lastUpdate: '2024-11-30', status: 'near', drivers: ['التواصل الفعال مع الشركاء'], obstacles: ['طول إجراءات التوقيع'], notes: 'شراكتان إضافيتان قيد التوقيع' }
    ],
    '1-18-140-1378': [
        { code: 'KPI-YFL-001', name: 'عدد المتدربين في برامج التوعية المالية', nameEn: 'Number of Financial Literacy Trainees', unit: 'متدرب', baseline: 0, target: 5000, actual: 2250, frequency: 'monthly', source: 'منصة التدريب', lastUpdate: '2024-11-30', status: 'behind', drivers: [], obstacles: ['ضعف الإقبال', 'قلة الحملات التسويقية'], notes: 'يتطلب تكثيف الجهود للوصول للهدف' },
        { code: 'KPI-YFL-002', name: 'نسبة اجتياز الاختبار', nameEn: 'Test Pass Rate', unit: '%', baseline: 60, target: 80, actual: 72, frequency: 'quarterly', source: 'نظام الاختبارات', lastUpdate: '2024-11-30', status: 'near', drivers: ['تحسين المحتوى التدريبي'], obstacles: ['صعوبة بعض المفاهيم'], notes: 'تحسن ملحوظ مقارنة بالربع السابق' },
        { code: 'KPI-YFL-003', name: 'عدد المدارس المشاركة', nameEn: 'Number of Participating Schools', unit: 'مدرسة', baseline: 10, target: 50, actual: 25, frequency: 'quarterly', source: 'إدارة البرنامج', lastUpdate: '2024-11-30', status: 'behind', drivers: ['دعم وزارة التعليم'], obstacles: ['التحديات اللوجستية', 'قيود الجدول الدراسي'], notes: 'خطة توسع جديدة قيد الإعداد' }
    ]
};

// ============= Tab Navigation =============
function onInitiativeSelected() {
    const selector = document.getElementById('initiativeSelector');
    const id = selector.value;
    
    if (!id) {
        selectedInitiative = null;
        document.getElementById('selectedInitiativeInfo')?.classList.add('d-none');
        return;
    }
    
    selectedInitiative = initiativesData[id];
    
    if (!selectedInitiative) {
        console.warn('Initiative not found:', id);
        return;
    }
    
    // Update selected initiative info panel
    const programEl = document.getElementById('selectedProgram');
    const statusEl = document.getElementById('selectedStatus');
    const progressEl = document.getElementById('selectedProgress');
    const infoPanel = document.getElementById('selectedInitiativeInfo');
    
    if (programEl) programEl.textContent = selectedInitiative.program;
    if (statusEl) {
        const statusColors = {
            'قيد التنفيذ': 'success',
            'مكتملة': 'info',
            'متأخرة': 'warning',
            'لم تبدأ': 'secondary',
            'ملغاة': 'danger'
        };
        const statusColor = statusColors[selectedInitiative.status] || 'secondary';
        statusEl.innerHTML = `<span class="badge bg-${statusColor}">${selectedInitiative.status}</span>`;
    }
    if (progressEl) progressEl.textContent = (selectedInitiative.actualProgress || selectedInitiative.progress || 0) + '%';
    if (infoPanel) infoPanel.classList.remove('d-none');
    
    // Navigate to details tab and load content
    navigateToTab('details');
}

function selectAndNavigate(id, tab) {
    document.getElementById('initiativeSelector').value = id;
    onInitiativeSelected();
    navigateToTab(tab);
}

function navigateToTab(tabName) {
    const tabMap = { 
        'details': { id: 'detailsTab', loader: loadInitiativeDetails },
        'kpis': { id: 'kpisTab', loader: loadInitiativeKPIs },
        'budget': { id: 'budgetTab', loader: loadInitiativeBudget },
        'milestones': { id: 'milestonesTab', loader: loadInitiativeMilestones },
        'risks': { id: 'risksTab', loader: loadInitiativeRisks },
        'docs': { id: 'docsTab', loader: loadInitiativeDocuments }
    };
    
    const tabInfo = tabMap[tabName];
    if (tabInfo) {
        const tabEl = document.getElementById(tabInfo.id);
        if (tabEl && !tabEl.classList.contains('disabled')) {
            // Use Bootstrap Tab API to show the tab (this will trigger 'shown.bs.tab' event)
            const tab = new bootstrap.Tab(tabEl);
            tab.show();
            
            // Also ensure content loads immediately if initiative is selected
            // The event listener will also fire, but this ensures it works
            if (selectedInitiative && tabInfo.loader) {
                setTimeout(() => {
                    tabInfo.loader();
                }, 50);
            }
        }
    }
}

// ============= Tab Content Loaders =============
document.addEventListener('DOMContentLoaded', function() {
    // Set up tab event listeners for all tabs
    const tabLoaders = {
        'detailsTab': () => { if (selectedInitiative) loadInitiativeDetails(); },
        'kpisTab': () => { if (selectedInitiative) loadInitiativeKPIs(); },
        'budgetTab': () => { if (selectedInitiative) loadInitiativeBudget(); },
        'milestonesTab': () => { if (selectedInitiative) loadInitiativeMilestones(); },
        'risksTab': () => { if (selectedInitiative) loadInitiativeRisks(); },
        'docsTab': () => { if (selectedInitiative) loadInitiativeDocuments(); }
    };
    
    // Attach event listeners to all tabs
    Object.keys(tabLoaders).forEach(tabId => {
        const tabElement = document.getElementById(tabId);
        if (tabElement) {
            tabElement.addEventListener('shown.bs.tab', tabLoaders[tabId]);
        }
    });
    
    // Auto-select default initiative and load content
    setTimeout(() => {
        const selector = document.getElementById('initiativeSelector');
        if (selector) {
            if (selector.value) {
                // If there's a selected value (from the selected attribute), trigger the selection
                onInitiativeSelected();
            } else {
                // If no value is selected, select the first available option
                const firstOption = selector.querySelector('option[value]:not([value=""])');
                if (firstOption) {
                    selector.value = firstOption.value;
                    onInitiativeSelected();
                }
            }
        }
    }, 100);
});

// ============= Initiative Details Tab (UC-14) =============
// As per BRD screen 40: تفاصيل المبادرة - التبويب 1
function loadInitiativeDetails() {
    const init = selectedInitiative;
    document.getElementById('details-content').innerHTML = `
        <div class="card">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="bi bi-info-circle me-2"></i>تفاصيل مستوى المبادرة</h5>
            </div>
            <div class="card-body">
                <form id="detailsForm_${init.id}">
                    <div class="row g-4">
                        <!-- Read-only fields as per BR-158 -->
                        <div class="col-md-4">
                            <label class="form-label text-muted fw-bold">المبادرة (كود)</label>
                            <input type="text" class="form-control-plaintext fw-bold fs-5 text-primary" value="${init.id}" readonly>
                        </div>
                        <div class="col-md-8">
                            <label class="form-label text-muted fw-bold">اسم المبادرة (عربي)</label>
                            <input type="text" class="form-control-plaintext fw-bold" value="${init.name}" readonly>
                        </div>
                        <div class="col-12">
                            <label class="form-label text-muted fw-bold">اسم المبادرة (إنجليزي)</label>
                            <input type="text" class="form-control-plaintext" value="${init.nameEn}" readonly>
                        </div>
                        
                        <!-- Additional read-only information -->
                        <div class="col-md-4">
                            <label class="form-label text-muted fw-bold">البرنامج</label>
                            <input type="text" class="form-control-plaintext" value="${init.program}" readonly>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label text-muted fw-bold">الجهة المسؤولة</label>
                            <input type="text" class="form-control-plaintext" value="${init.responsibleEntity || 'غير محدد'}" readonly>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label text-muted fw-bold">الحالة</label>
                            <div class="form-control-plaintext">
                                <span class="badge bg-${getStatusColor(init.status)}">${init.status}</span>
                                ${init.isPivotal ? '<span class="badge bg-warning ms-2"><i class="bi bi-star-fill"></i> محورية</span>' : ''}
                            </div>
                        </div>
                        
                        <!-- Timeline Information -->
                        <div class="col-md-3">
                            <label class="form-label text-muted fw-bold">تاريخ البداية</label>
                            <input type="text" class="form-control-plaintext" value="${init.startDate || 'غير محدد'}" readonly>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label text-muted fw-bold">تاريخ النهاية</label>
                            <input type="text" class="form-control-plaintext" value="${init.endDate || 'غير محدد'}" readonly>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label text-muted fw-bold">التقدم المخطط</label>
                            <div class="progress" style="height: 25px; margin-top: 8px;">
                                <div class="progress-bar bg-info" style="width: ${init.plannedProgress || 0}%">${init.plannedProgress || 0}%</div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label text-muted fw-bold">التقدم الفعلي</label>
                            <div class="progress" style="height: 25px; margin-top: 8px;">
                                <div class="progress-bar bg-${init.actualProgress >= init.plannedProgress ? 'success' : 'warning'}" style="width: ${init.actualProgress || init.progress || 0}%">${init.actualProgress || init.progress || 0}%</div>
                            </div>
                        </div>
                        
                        <div class="col-12"><hr></div>
                        
                        <!-- Editable fields as per BR-159, BR-160, BR-161 -->
                        <div class="col-12">
                            <label class="form-label fw-bold">ملاحظات حول المبادرة</label>
                            <textarea class="form-control" id="notes_${init.id}" rows="4" placeholder="أدخل ملاحظات حول المبادرة...">${init.notes || ''}</textarea>
                            <small class="text-muted">اختياري - لا يوجد حد أقصى للأحرف (BR-160)</small>
                        </div>
                        <div class="col-12">
                            <label class="form-label fw-bold">لمحة عامة حول أداء المبادرات <span class="text-danger">*</span></label>
                            <textarea class="form-control" id="overview_${init.id}" rows="6" placeholder="أدخل لمحة عامة شاملة عن أداء المبادرة..." required>${init.performanceOverview || ''}</textarea>
                            <small class="text-muted">إلزامي لإصدار التقارير (BR-159) - لا يوجد حد أقصى للأحرف</small>
                        </div>
                        
                        <!-- Navigation buttons as per UC-14 -->
                        <div class="col-12 border-top pt-3">
                            <button type="button" class="btn btn-primary btn-lg" onclick="saveInitiativeDetails('${init.id}')">
                                <i class="bi bi-save me-2"></i>حفظ
                            </button>
                            <button type="button" class="btn btn-outline-primary btn-lg ms-2" onclick="navigateToTab('kpis')">
                                التالي <i class="bi bi-arrow-left ms-2"></i>
                            </button>
                            <button type="button" class="btn btn-outline-secondary btn-lg ms-2" onclick="backToInitiativesList()">
                                <i class="bi bi-arrow-right me-2"></i>رجوع
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    `;
}

// Helper function for status colors
function getStatusColor(status) {
    const statusColors = {
        'قيد التنفيذ': 'success',
        'مكتملة': 'info',
        'متأخرة': 'warning',
        'لم تبدأ': 'secondary',
        'ملغاة': 'danger'
    };
    return statusColors[status] || 'secondary';
}

// Navigate back to initiatives list
function backToInitiativesList() {
    selectedInitiative = null;
    document.getElementById('initiativeSelector').value = '';
    document.getElementById('selectedInitiativeInfo').classList.add('d-none');
    navigateToTab('details');
}

function saveInitiativeDetails(id) {
    const overview = document.getElementById(`overview_${id}`)?.value;
    if (!overview || overview.trim() === '') {
        alert('لمحة عامة حول أداء المبادرات حقل إلزامي');
        return;
    }
    console.log('Saving details:', id, { notes: document.getElementById(`notes_${id}`)?.value, overview });
    alert('تم حفظ التفاصيل بنجاح');
}

// ============= KPI Management (UC-15) =============
// As per BRD UC-15: إدارة مؤشرات المبادرة (التبويب 2)
function loadInitiativeKPIs() {
    if (!selectedInitiative) {
        document.getElementById('kpis-content').innerHTML = `
            <div class="alert alert-info">
                <i class="bi bi-info-circle"></i> اختر مبادرة من التبويب الأول لعرض مؤشراتها
            </div>
        `;
        return;
    }
    
    const init = selectedInitiative;
    const kpis = kpiMockData[init.id] || [];
    
    // Build complete KPIs tab content
    let kpisHTML = `
        <div class="card mb-4">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                <h5 class="mb-0"><i class="bi bi-graph-up me-2"></i>مؤشرات مستوى المبادرة - ${init.name}</h5>
                <div>
                    <button class="btn btn-light btn-sm me-2" onclick="addNewKPI()">
                        <i class="bi bi-plus-circle me-1"></i>إضافة مؤشر
                    </button>
                    <button class="btn btn-light btn-sm" onclick="exportKPIs()">
                        <i class="bi bi-download me-1"></i>تصدير
                    </button>
                </div>
            </div>
            <div class="card-body">
                <!-- KPI Statistics Cards -->
                <div class="row g-3 mb-4" id="kpi-statistics">
                    <div class="col-md-3">
                        <div class="card border-success h-100">
                            <div class="card-body text-center">
                                <h3 class="text-success mb-0" id="kpi-achieved-count">0</h3>
                                <small class="text-muted">محقق (≥90%)</small>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="card border-warning h-100">
                            <div class="card-body text-center">
                                <h3 class="text-warning mb-0" id="kpi-near-count">0</h3>
                                <small class="text-muted">قريب (70-89%)</small>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="card border-danger h-100">
                            <div class="card-body text-center">
                                <h3 class="text-danger mb-0" id="kpi-behind-count">0</h3>
                                <small class="text-muted">متأخر (&lt;70%)</small>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="card border-info h-100">
                            <div class="card-body text-center">
                                <h3 class="text-info mb-0" id="kpi-avg-performance">0%</h3>
                                <small class="text-muted">متوسط الأداء</small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Search and Filters -->
                <div class="row g-3 mb-3">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-text"><i class="bi bi-search"></i></span>
                            <input type="text" class="form-control" id="kpi-search" placeholder="ابحث عن مؤشر...">
                        </div>
                    </div>
                    <div class="col-md-6">
                        <button class="btn btn-outline-secondary" onclick="toggleKPIFilters()">
                            <i class="bi bi-funnel" id="kpi-filter-toggle-icon"></i> فلاتر متقدمة
                        </button>
                    </div>
                </div>
                
                <!-- Advanced Filters (Collapsible) -->
                <div class="card mb-3 d-none" id="kpi-filters-section">
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-md-3">
                                <label class="form-label">حالة المؤشر</label>
                                <select class="form-select" id="kpi-status-filter">
                                    <option value="">الكل</option>
                                    <option value="achieved">محقق</option>
                                    <option value="near">قريب</option>
                                    <option value="behind">متأخر</option>
                                </select>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">التواتر</label>
                                <select class="form-select" id="kpi-frequency-filter">
                                    <option value="">الكل</option>
                                    <option value="monthly">شهري</option>
                                    <option value="quarterly">ربع سنوي</option>
                                    <option value="yearly">سنوي</option>
                                </select>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">الحد الأدنى للمستهدف</label>
                                <input type="number" class="form-control" id="kpi-target-min" placeholder="0">
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">الحد الأقصى للمستهدف</label>
                                <input type="number" class="form-control" id="kpi-target-max" placeholder="999999">
                            </div>
                        </div>
                        <div class="mt-3">
                            <button class="btn btn-primary btn-sm" onclick="applyKPIFilters()">تطبيق الفلاتر</button>
                            <button class="btn btn-outline-secondary btn-sm ms-2" onclick="document.getElementById('kpi-filters-section').classList.add('d-none')">إلغاء</button>
                        </div>
                    </div>
                </div>
                
                <!-- KPIs Table -->
                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead class="table-light">
                            <tr>
                                <th width="3%"><input type="checkbox" class="form-check-input" id="selectAllKPIs"></th>
                                <th width="10%">كود المؤشر</th>
                                <th width="25%">اسم المؤشر</th>
                                <th width="8%">الوحدة</th>
                                <th width="10%">خط الأساس</th>
                                <th width="10%">المستهدف</th>
                                <th width="12%">القيمة الفعلية</th>
                                <th width="8%">الانحراف</th>
                                <th width="8%">الحالة</th>
                                <th width="6%">الإجراءات</th>
                            </tr>
                        </thead>
                        <tbody id="kpis-table-body">
                            ${kpis.length === 0 ? '<tr><td colspan="10" class="text-center text-muted py-4">لا توجد مؤشرات متاحة</td></tr>' : ''}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        
        <!-- Performance Factors Section -->
        ${kpis.length > 0 ? `
        <div class="card">
            <div class="card-header bg-light">
                <h5 class="mb-0"><i class="bi bi-bar-chart me-2"></i>دوافع ومعوقات الأداء</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <h6 class="text-success mb-3"><i class="bi bi-check-circle me-2"></i>دوافع الأداء</h6>
                        <ul class="list-group" id="performance-drivers-list"></ul>
                        <button class="btn btn-success btn-sm mt-2" onclick="addPerformanceDriver()">
                            <i class="bi bi-plus-circle me-1"></i>إضافة دافع
                        </button>
                    </div>
                    <div class="col-md-6">
                        <h6 class="text-danger mb-3"><i class="bi bi-x-circle me-2"></i>معوقات الأداء</h6>
                        <ul class="list-group" id="performance-obstacles-list"></ul>
                        <button class="btn btn-danger btn-sm mt-2" onclick="addPerformanceObstacle()">
                            <i class="bi bi-plus-circle me-1"></i>إضافة معوق
                        </button>
                    </div>
                </div>
                <hr>
                <div class="mt-3">
                    <label class="form-label fw-bold">ملاحظات ربع سنوية</label>
                    <textarea class="form-control" id="kpi-quarterly-notes" rows="3" placeholder="أدخل ملاحظات حول أداء المؤشرات..."></textarea>
                    <small class="text-muted">آخر تحديث: <span id="kpi-notes-timestamp">-</span></small>
                </div>
            </div>
        </div>
        ` : ''}
    `;
    
    document.getElementById('kpis-content').innerHTML = kpisHTML;
    
    // Update statistics and populate table
    if (kpis.length > 0) {
        updateKPIStatistics(kpis);
        populateKPITable(kpis);
        loadPerformanceFactors(kpis[0]);
    }
}

function updateKPIStatistics(kpis) {
    if (!kpis || kpis.length === 0) {
        const achievedEl = document.getElementById('kpi-achieved-count');
        const nearEl = document.getElementById('kpi-near-count');
        const behindEl = document.getElementById('kpi-behind-count');
        const avgEl = document.getElementById('kpi-avg-performance');
        if (achievedEl) achievedEl.textContent = '0';
        if (nearEl) nearEl.textContent = '0';
        if (behindEl) behindEl.textContent = '0';
        if (avgEl) avgEl.textContent = '0%';
        return;
    }
    
    let achieved = 0, near = 0, behind = 0, totalPerformance = 0;
    kpis.forEach(kpi => {
        const performance = calculateKPIPerformance(kpi);
        totalPerformance += performance;
        if (performance >= 90) achieved++;
        else if (performance >= 70) near++;
        else behind++;
    });
    
    const achievedEl = document.getElementById('kpi-achieved-count');
    const nearEl = document.getElementById('kpi-near-count');
    const behindEl = document.getElementById('kpi-behind-count');
    const avgEl = document.getElementById('kpi-avg-performance');
    
    if (achievedEl) achievedEl.textContent = achieved;
    if (nearEl) nearEl.textContent = near;
    if (behindEl) behindEl.textContent = behind;
    if (avgEl) avgEl.textContent = kpis.length > 0 ? Math.round(totalPerformance / kpis.length) + '%' : '0%';
}

function calculateKPIPerformance(kpi) {
    if (kpi.target === kpi.baseline) return 0;
    const performance = ((kpi.actual - kpi.baseline) / (kpi.target - kpi.baseline)) * 100;
    return Math.max(0, Math.min(100, Math.round(performance)));
}

function calculateDeviation(kpi) {
    if (kpi.target === 0) return 0;
    return Math.round(((kpi.actual - kpi.target) / kpi.target) * 100);
}

function populateKPITable(kpis) {
    const tbody = document.getElementById('kpis-table-body');
    if (!tbody) return;
    
    tbody.innerHTML = '';
    
    if (!kpis || kpis.length === 0) {
        tbody.innerHTML = '<tr><td colspan="10" class="text-center text-muted py-4">لا توجد مؤشرات متاحة</td></tr>';
        return;
    }
    
    kpis.forEach(kpi => {
        const performance = calculateKPIPerformance(kpi);
        const deviation = calculateDeviation(kpi);
        const statusBadge = getKPIStatusBadge(performance);
        tbody.innerHTML += `
            <tr>
                <td><input type="checkbox" class="form-check-input kpi-checkbox" value="${kpi.code}"></td>
                <td class="fw-bold">${kpi.code}</td>
                <td><div>${kpi.name}</div><small class="text-muted">${kpi.nameEn || ''}</small></td>
                <td>${kpi.unit || '-'}</td>
                <td>${(kpi.baseline || 0).toLocaleString('ar-SA')}</td>
                <td class="fw-semibold text-primary">${(kpi.target || 0).toLocaleString('ar-SA')}</td>
                <td><span class="fw-bold">${(kpi.actual || 0).toLocaleString('ar-SA')}</span><br><small class="text-muted">آخر تحديث: ${kpi.lastUpdate || '-'}</small></td>
                <td><span class="badge ${deviation >= 0 ? 'bg-success' : 'bg-danger'}">${deviation > 0 ? '+' : ''}${deviation}%</span></td>
                <td>${statusBadge}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <button class="btn btn-outline-primary" onclick="editKPI('${kpi.code}')" title="تعديل"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-info" onclick="viewKPIDetails('${kpi.code}')" title="تفاصيل"><i class="bi bi-eye"></i></button>
                        <button class="btn btn-outline-success" onclick="updateKPIValue('${kpi.code}')" title="تحديث القيمة"><i class="bi bi-arrow-clockwise"></i></button>
                        <button class="btn btn-outline-warning" onclick="calculateKPIFormula('${kpi.code}')" title="حساب المعادلة"><i class="bi bi-calculator"></i></button>
                    </div>
                </td>
            </tr>
        `;
    });
}

function getKPIStatusBadge(performance) {
    if (performance >= 90) return '<span class="badge bg-success"><i class="bi bi-check-circle"></i> محقق</span>';
    else if (performance >= 70) return '<span class="badge bg-warning"><i class="bi bi-exclamation-triangle"></i> قريب</span>';
    else if (performance >= 50) return '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> متأخر</span>';
    else return '<span class="badge bg-dark"><i class="bi bi-exclamation-octagon"></i> حرج</span>';
}

function loadPerformanceFactors(kpi) {
    if (!kpi) return;
    
    const driversList = document.getElementById('performance-drivers-list');
    if (driversList) {
        driversList.innerHTML = '';
        if (kpi.drivers && kpi.drivers.length > 0) {
            kpi.drivers.forEach(driver => {
                driversList.innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center"><span><i class="bi bi-check-circle text-success me-2"></i>${driver}</span><button class="btn btn-sm btn-outline-danger" onclick="removeDriver(this)"><i class="bi bi-trash"></i></button></li>`;
            });
        } else {
            driversList.innerHTML = '<li class="list-group-item text-muted">لا توجد دوافع أداء</li>';
        }
    }
    
    const obstaclesList = document.getElementById('performance-obstacles-list');
    if (obstaclesList) {
        obstaclesList.innerHTML = '';
        if (kpi.obstacles && kpi.obstacles.length > 0) {
            kpi.obstacles.forEach(obstacle => {
                obstaclesList.innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center"><span><i class="bi bi-x-circle text-danger me-2"></i>${obstacle}</span><button class="btn btn-sm btn-outline-danger" onclick="removeObstacle(this)"><i class="bi bi-trash"></i></button></li>`;
            });
        } else {
            obstaclesList.innerHTML = '<li class="list-group-item text-muted">لا توجد معوقات أداء</li>';
        }
    }
    
    const notesField = document.getElementById('kpi-quarterly-notes');
    const timestampField = document.getElementById('kpi-notes-timestamp');
    if (notesField && kpi.notes) {
        notesField.value = kpi.notes;
    }
    if (timestampField && kpi.lastUpdate) {
        timestampField.textContent = kpi.lastUpdate;
    }
}

function toggleKPIFilters() {
    const filtersSection = document.getElementById('kpi-filters-section');
    const icon = document.getElementById('kpi-filter-toggle-icon');
    if (!filtersSection || !icon) return;
    
    if (filtersSection.classList.contains('d-none')) {
        filtersSection.classList.remove('d-none');
        icon.className = 'bi bi-chevron-up';
    } else {
        filtersSection.classList.add('d-none');
        icon.className = 'bi bi-funnel';
    }
}

function applyKPIFilters() {
    const searchText = document.getElementById('kpi-search').value.toLowerCase();
    const statusFilter = document.getElementById('kpi-status-filter').value;
    const targetMin = document.getElementById('kpi-target-min').value;
    const targetMax = document.getElementById('kpi-target-max').value;
    const frequencyFilter = document.getElementById('kpi-frequency-filter').value;
    console.log('Applying filters:', { searchText, statusFilter, targetMin, targetMax, frequencyFilter });
    showNotification('تم تطبيق الفلاتر بنجاح', 'success');
}

function linkToIndicatorsPage() {
    const initId = selectedInitiative ? selectedInitiative.id : '';
    window.open(`indicators.html?initiative=${initId}&source=performance-initiatives`, '_blank');
}

function calculateKPIFormula(kpiCode) {
    alert(`حساب المعادلة للمؤشر: ${kpiCode}\nيتم فتح نافذة حساب المعادلة وفقاً للمتطلب BR-26`);
}

function addNewKPI() { alert('فتح نموذج إضافة مؤشر جديد'); }
function updateKPIValue(kpiCode) {
    const newValue = prompt(`أدخل القيمة الجديدة للمؤشر ${kpiCode}:`);
    if (newValue) showNotification(`تم تحديث قيمة المؤشر ${kpiCode} إلى ${newValue}`, 'success');
}
function editKPI(kpiCode) { alert(`تعديل المؤشر: ${kpiCode}`); }
function viewKPIDetails(kpiCode) { alert(`عرض تفاصيل المؤشر: ${kpiCode}`); }
function exportKPIs() {
    showNotification('جاري تصدير المؤشرات إلى Excel...', 'info');
    setTimeout(() => showNotification('تم تصدير المؤشرات بنجاح', 'success'), 2000);
}

function addPerformanceDriver() {
    const driver = prompt('أدخل دافع الأداء الجديد:');
    if (driver) {
        document.getElementById('performance-drivers-list').innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center"><span><i class="bi bi-check-circle text-success me-2"></i>${driver}</span><button class="btn btn-sm btn-outline-danger" onclick="removeDriver(this)"><i class="bi bi-trash"></i></button></li>`;
        showNotification('تم إضافة دافع الأداء', 'success');
    }
}

function addPerformanceObstacle() {
    const obstacle = prompt('أدخل معوق الأداء الجديد:');
    if (obstacle) {
        document.getElementById('performance-obstacles-list').innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center"><span><i class="bi bi-x-circle text-danger me-2"></i>${obstacle}</span><button class="btn btn-sm btn-outline-danger" onclick="removeObstacle(this)"><i class="bi bi-trash"></i></button></li>`;
        showNotification('تم إضافة معوق الأداء', 'success');
    }
}

function removeDriver(btn) { btn.closest('li').remove(); showNotification('تم حذف دافع الأداء', 'info'); }
function removeObstacle(btn) { btn.closest('li').remove(); showNotification('تم حذف معوق الأداء', 'info'); }

// ============= Budget Tab (UC-16) =============
function loadInitiativeBudget() {
    if (!selectedInitiative) return;
    const id = selectedInitiative.id;
    const name = selectedInitiative.name;
    const implementationPlan = [{ year: 2024, budget: 20000000 }, { year: 2025, budget: 30000000 }];
    const totalPlannedBudget = implementationPlan.reduce((sum, item) => sum + item.budget, 0);
    
    document.getElementById('budget-content').innerHTML = `
        <div class="alert alert-info border-0"><i class="bi bi-building me-2"></i><strong>${name}</strong><span class="badge bg-primary ms-2">${id}</span></div>
        <div class="card mb-4 shadow-sm">
            <div class="card-header bg-light"><h5 class="mb-0"><i class="bi bi-calendar-check me-2"></i>خطة التنفيذ</h5></div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-bordered mb-0">
                        <thead class="table-light">
                            <tr><th>السنة</th>${implementationPlan.map(item => `<th class="text-center">${item.year}</th>`).join('')}<th class="table-primary text-center"><strong>الإجمالي</strong></th></tr>
                        </thead>
                        <tbody>
                            <tr><td><strong>الميزانية المخططة</strong></td>${implementationPlan.map(item => `<td class="text-end">${formatCurrency(item.budget)}</td>`).join('')}<td class="table-primary text-end"><strong>${formatCurrency(totalPlannedBudget)}</strong></td></tr>
                        </tbody>
                    </table>
                </div>
                <div class="text-muted small mt-3"><i class="bi bi-lock-fill me-1"></i>خطة التنفيذ معتمدة ولا يمكن تعديلها</div>
            </div>
        </div>
        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white"><h5 class="mb-0"><i class="bi bi-cash-stack me-2"></i>ميزانية المبادرة</h5></div>
            <div class="card-body">
                <form id="budgetForm_${id}">
                    <div class="row g-4">
                        <div class="col-md-6">
                            <label class="form-label fw-bold">الميزانية المصروفة حتى تاريخه <span class="text-danger">*</span></label>
                            <div class="input-group input-group-lg">
                                <input type="number" class="form-control" id="allocatedToDate_${id}" min="0" step="0.01" placeholder="0.00" onchange="validateBudget('${id}')">
                                <span class="input-group-text">ريال</span>
                            </div>
                            <small class="text-muted">إجمالي المخصصات منذ البداية</small>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold">الميزانية المصروفة بشكل ربع سنوي <span class="text-danger">*</span></label>
                            <div class="input-group input-group-lg">
                                <input type="number" class="form-control" id="allocatedQuarter_${id}" min="0" step="0.01" placeholder="0.00" onchange="validateBudget('${id}')">
                                <span class="input-group-text">ريال</span>
                            </div>
                            <small class="text-muted">المخصصات للربع الحالي</small>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold">المنصرف الفعلي حتى تاريخه <span class="text-danger">*</span></label>
                            <div class="input-group input-group-lg">
                                <input type="number" class="form-control" id="spentToDate_${id}" min="0" step="0.01" placeholder="0.00" onchange="validateBudget('${id}')">
                                <span class="input-group-text">ريال</span>
                            </div>
                            <small class="text-muted">إجمالي الإنفاق الفعلي</small>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold">الميزانية المنفقة بشكل ربع سنوي <span class="text-danger">*</span></label>
                            <div class="input-group input-group-lg">
                                <input type="number" class="form-control" id="spentQuarter_${id}" min="0" step="0.01" placeholder="0.00" onchange="validateBudget('${id}')">
                                <span class="input-group-text">ريال</span>
                            </div>
                            <small class="text-muted">الإنفاق للربع الحالي</small>
                        </div>
                        <div class="col-12"><div id="budgetErrors_${id}" class="d-none"></div></div>
                        <div class="col-12 border-top pt-3">
                            <button type="button" class="btn btn-primary btn-lg" onclick="saveBudget('${id}')"><i class="bi bi-save me-2"></i>حفظ الميزانية</button>
                            <button type="button" class="btn btn-outline-secondary btn-lg ms-2" onclick="document.getElementById('budgetForm_${id}').reset(); validateBudget('${id}')"><i class="bi bi-arrow-counterclockwise me-2"></i>إعادة تعيين</button>
                        </div>
                    </div>
                </form>
                <hr class="my-4">
                <h6 class="text-muted mb-3"><i class="bi bi-graph-up me-2"></i>التحليل المالي</h6>
                <div class="row g-3">
                    <div class="col-md-3"><div class="card border-primary"><div class="card-body text-center"><small class="text-muted d-block mb-2">الميزانية المصروفة</small><h5 class="text-primary mb-0" id="totalAllocated_${id}">0.00 ريال</h5></div></div></div>
                    <div class="col-md-3"><div class="card border-success"><div class="card-body text-center"><small class="text-muted d-block mb-2">المنصرف الفعلي</small><h5 class="text-success mb-0" id="totalSpent_${id}">0.00 ريال</h5></div></div></div>
                    <div class="col-md-3"><div class="card border-info"><div class="card-body text-center"><small class="text-muted d-block mb-2">المتبقي</small><h5 class="text-info mb-0" id="remaining_${id}">0.00 ريال</h5></div></div></div>
                    <div class="col-md-3"><div class="card border-warning"><div class="card-body text-center"><small class="text-muted d-block mb-2">نسبة الصرف</small><h5 class="text-warning mb-0" id="spendingRate_${id}">0%</h5></div></div></div>
                </div>
            </div>
        </div>
    `;
}

function validateBudget(id) {
    const allocatedToDate = parseFloat(document.getElementById(`allocatedToDate_${id}`)?.value) || 0;
    const allocatedQuarter = parseFloat(document.getElementById(`allocatedQuarter_${id}`)?.value) || 0;
    const spentToDate = parseFloat(document.getElementById(`spentToDate_${id}`)?.value) || 0;
    const spentQuarter = parseFloat(document.getElementById(`spentQuarter_${id}`)?.value) || 0;
    
    const errors = [];
    if (spentToDate > allocatedToDate) errors.push('المنصرف الفعلي الإجمالي يتجاوز الميزانية المصروفة');
    if (spentQuarter > allocatedQuarter) errors.push('المنصرف الربع سنوي يتجاوز الميزانية الربع سنوية');
    if (allocatedQuarter > allocatedToDate) errors.push('الميزانية الربع سنوية تتجاوز الميزانية الإجمالية');
    if (spentQuarter > spentToDate) errors.push('المنصرف الربع سنوي يتجاوز المنصرف الإجمالي');
    
    const errorsDiv = document.getElementById(`budgetErrors_${id}`);
    if (errors.length > 0) {
        errorsDiv.className = 'alert alert-danger';
        errorsDiv.innerHTML = `<i class="bi bi-exclamation-triangle me-2"></i><strong>أخطاء:</strong><ul class="mb-0 mt-2">${errors.map(e => `<li>${e}</li>`).join('')}</ul>`;
    } else if (allocatedToDate > 0 || spentToDate > 0) {
        errorsDiv.className = 'alert alert-success';
        errorsDiv.innerHTML = '<i class="bi bi-check-circle me-2"></i><strong>البيانات صحيحة</strong>';
    } else {
        errorsDiv.className = 'd-none';
    }
    
    const remaining = allocatedToDate - spentToDate;
    const rate = allocatedToDate > 0 ? ((spentToDate / allocatedToDate) * 100).toFixed(1) : 0;
    
    document.getElementById(`totalAllocated_${id}`).textContent = formatCurrency(allocatedToDate);
    document.getElementById(`totalSpent_${id}`).textContent = formatCurrency(spentToDate);
    document.getElementById(`remaining_${id}`).textContent = formatCurrency(remaining);
    document.getElementById(`spendingRate_${id}`).textContent = rate + '%';
    
    return errors.length === 0;
}

function saveBudget(id) {
    if (!validateBudget(id)) {
        alert('لا يمكن الحفظ: يوجد أخطاء في البيانات');
        return;
    }
    console.log('Saving:', id);
    alert('تم حفظ البيانات المالية بنجاح');
}

function validateInitiativeBudget(id) { return validateBudget(id); }
function saveInitiativeBudget(id, name) { saveBudget(id); }
function resetBudgetForm(id) {
    document.getElementById(`budgetForm_${id}`)?.reset();
    validateBudget(id);
}

// ============= Milestones Tab (UC-17) =============
let milestonesTable, milestonesData = [];

document.addEventListener('DOMContentLoaded', function() {
    const milestonesTab = document.querySelector('[data-bs-target="#milestones-content"]');
    if (milestonesTab) {
        milestonesTab.addEventListener('click', function() {
            setTimeout(() => initializeMilestones(), 100);
        });
    }
});

function initializeMilestones() {
    if (!milestonesTable && $('#milestonesDataTable').length > 0) {
        loadMilestonesData();
        initializeDataTable();
        setupFilters();
        updateSummaryCards();
    }
}

function loadMilestonesData() {
    milestonesData = [
        { code: 'M-001', name: 'إطلاق النسخة التجريبية من المنصة', initiative: '1-18-139-1377', initiativeName: 'تطوير منصة رقمية', plannedDate: '2024-03-15', actualDate: '2024-03-10', progress: 100, status: 'مكتمل', priority: 'عالية' },
        { code: 'M-002', name: 'تدريب 100 موظف على النظام', initiative: '1-18-139-1377', initiativeName: 'تطوير منصة رقمية', plannedDate: '2024-04-30', actualDate: '-', progress: 75, status: 'جاري', priority: 'متوسطة' },
        { code: 'M-003', name: 'إطلاق حملة التوعية الأولى', initiative: '1-18-140-1378', initiativeName: 'التوعية المالية', plannedDate: '2024-04-01', actualDate: '-', progress: 25, status: 'متأخر', priority: 'عالية' },
        { code: 'M-004', name: 'توقيع الشراكة مع الجامعات', initiative: '1-18-140-1378', initiativeName: 'التوعية المالية', plannedDate: '2024-05-15', actualDate: '-', progress: 0, status: 'لم يبدأ', priority: 'منخفضة' },
        { code: 'M-005', name: 'تطوير واجهات المستخدم', initiative: '1-18-139-1377', initiativeName: 'تطوير منصة رقمية', plannedDate: '2024-05-01', actualDate: '-', progress: 60, status: 'جاري', priority: 'عالية' },
        { code: 'M-006', name: 'اختبار الأمان والحماية', initiative: '1-18-139-1377', initiativeName: 'تطوير منصة رقمية', plannedDate: '2024-06-01', actualDate: '-', progress: 0, status: 'لم يبدأ', priority: 'عالية' }
    ];
}

function initializeDataTable() {
    milestonesTable = $('#milestonesDataTable').DataTable({
        data: milestonesData,
        language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/ar.json' },
        columns: [
            { data: 'code' },
            { data: 'name' },
            { data: 'initiativeName' },
            { data: 'plannedDate' },
            { data: 'actualDate' },
            { data: 'progress', render: function(data) {
                const color = data === 100 ? 'bg-success' : data >= 50 ? 'bg-warning' : 'bg-danger';
                return `<div class="progress" style="height: 20px;"><div class="progress-bar ${color}" style="width: ${data}%">${data}%</div></div>`;
            }},
            { data: 'status', render: function(data) {
                const badges = { 'مكتمل': 'bg-success', 'جاري': 'bg-primary', 'متأخر': 'bg-danger', 'لم يبدأ': 'bg-secondary' };
                return `<span class="badge ${badges[data]}">${data}</span>`;
            }},
            { data: 'priority', render: function(data) {
                const colors = { 'عالية': 'text-danger', 'متوسطة': 'text-warning', 'منخفضة': 'text-info' };
                return `<span class="${colors[data]}"><i class="bi bi-circle-fill"></i> ${data}</span>`;
            }},
            { data: null, orderable: false, render: function() {
                return `<button class="btn btn-sm btn-outline-primary" onclick="viewMilestoneDetails(this)"><i class="bi bi-eye"></i></button>`;
            }}
        ],
        pageLength: 10,
        responsive: true
    });
}

function setupFilters() {
    $('#filterInitiative').on('change', function() { milestonesTable.column(2).search(this.value).draw(); updateSummaryCards(); });
    $('#filterStatus').on('change', function() { milestonesTable.column(6).search(this.value).draw(); updateSummaryCards(); });
    $('#filterPriority').on('change', function() { milestonesTable.column(7).search(this.value).draw(); updateSummaryCards(); });
    $('#quickSearch').on('keyup', function() { milestonesTable.search(this.value).draw(); updateSummaryCards(); });
}

function updateSummaryCards() {
    if (!milestonesTable) return;
    const filteredData = milestonesTable.rows({ search: 'applied' }).data().toArray();
    $('#totalMilestones').text(filteredData.length);
    $('#completedMilestones').text(filteredData.filter(m => m.status === 'مكتمل').length);
    $('#inProgressMilestones').text(filteredData.filter(m => m.status === 'جاري').length);
    $('#delayedMilestones').text(filteredData.filter(m => m.status === 'متأخر').length);
}

function loadAllMilestones() {
    $('#filterInitiative, #filterStatus, #filterPriority').val('');
    $('#quickSearch').val('');
    if (milestonesTable) milestonesTable.search('').columns().search('').draw();
    updateSummaryCards();
}

function loadSelectedMilestones() { alert('سيتم تحميل معالم المبادرات المختارة من الجدول الأول'); }
function exportMilestones() { alert('جاري تصدير المعالم إلى ملف Excel...'); }
function viewMilestoneDetails(btn) {
    const data = milestonesTable.row($(btn).parents('tr')).data();
    alert('عرض تفاصيل المعلم: ' + data.name);
}

function toggleTimelineView() {
    const timelineView = document.getElementById('timelineView');
    timelineView.style.display = timelineView.style.display === 'none' ? 'block' : 'none';
    if (timelineView.style.display === 'block') {
        document.querySelector('.timeline-container').innerHTML = '<div class="alert alert-info"><i class="bi bi-calendar3 me-2"></i>العرض الزمني للمعالم - قيد التطوير</div>';
    }
}

function loadInitiativeMilestones() {
    if (!selectedInitiative) return;
    const init = selectedInitiative;
    document.getElementById('milestones-content').innerHTML = `
        <div class="card">
            <div class="card-header bg-light"><h5 class="mb-0"><i class="bi bi-flag-fill me-2"></i>مراحل إنجاز مستوى المبادرة</h5></div>
            <div class="card-body">
                <div class="row g-3 mb-4">
                    <div class="col-md-6">
                        <label class="form-label fw-bold">مرحلة الإنجاز الرئيسية للمبادرة</label>
                        <select class="form-select" id="milestonePhaseFilter_${init.id}">
                            <option value="">جميع المراحل</option>
                            <option value="phase1">المرحلة الأولى</option>
                            <option value="phase2">المرحلة الثانية</option>
                            <option value="phase3">المرحلة الثالثة</option>
                        </select>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-bold">اسم المعلم</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="bi bi-search"></i></span>
                            <input type="text" class="form-control" id="milestoneSearch_${init.id}" placeholder="ابحث عن معلم...">
                        </div>
                    </div>
                </div>
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead class="table-light">
                            <tr><th width="18%">الإجراء</th><th width="12%">رمز مرحلة الإنجاز</th><th width="30%">اسم المعلم</th><th width="15%">تاريخ الانتهاء المستهدف</th><th width="10%">كود المبادرة</th><th width="15%">اسم المبادرة</th></tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><div class="btn-group btn-group-sm" role="group">
                                    <button class="btn btn-outline-primary" onclick="viewMilestone('M-001', '${init.id}')" title="عرض"><i class="bi bi-eye"></i></button>
                                    <button class="btn btn-outline-success" onclick="editMilestone('M-001', '${init.id}')" title="تعديل"><i class="bi bi-pencil"></i></button>
                                    <button class="btn btn-outline-info" onclick="uploadMilestoneDoc('M-001', '${init.id}')" title="تحميل ملف"><i class="bi bi-file-earmark-arrow-up"></i></button>
                                </div></td>
                                <td class="fw-semibold">PH1-M001</td>
                                <td>إطلاق النسخة التجريبية من المنصة</td>
                                <td>2024-06-30</td>
                                <td class="text-muted small">${init.id}</td>
                                <td class="small">${init.name.substring(0, 20)}...</td>
                            </tr>
                            <tr>
                                <td><div class="btn-group btn-group-sm" role="group">
                                    <button class="btn btn-outline-primary" onclick="viewMilestone('M-002', '${init.id}')"><i class="bi bi-eye"></i></button>
                                    <button class="btn btn-outline-success" onclick="editMilestone('M-002', '${init.id}')"><i class="bi bi-pencil"></i></button>
                                    <button class="btn btn-outline-info" onclick="uploadMilestoneDoc('M-002', '${init.id}')"><i class="bi bi-file-earmark-arrow-up"></i></button>
                                </div></td>
                                <td class="fw-semibold">PH2-M002</td>
                                <td>تدريب المستخدمين على النظام</td>
                                <td>2024-08-15</td>
                                <td class="text-muted small">${init.id}</td>
                                <td class="small">${init.name.substring(0, 20)}...</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="text-muted small mt-3"><i class="bi bi-info-circle me-1"></i>${init.milestoneCount} معالم مرتبطة بهذه المبادرة</div>
            </div>
        </div>
    `;
}

function viewMilestone(milestoneId, initiativeId) { alert(`عرض تفاصيل المعلم: ${milestoneId}`); }
function editMilestone(milestoneId, initiativeId) { alert(`تعديل المعلم: ${milestoneId}`); }
function uploadMilestoneDoc(milestoneId, initiativeId) { alert(`رفع مستند للمعلم: ${milestoneId}`); }

// ============= Risks Tab =============
function loadInitiativeRisks() {
    if (!selectedInitiative) return;
    const init = selectedInitiative;
    document.getElementById('risks-content').innerHTML = `
        <div class="row g-3 mb-4">
            <div class="col-md-3"><div class="card border-danger h-100"><div class="card-body text-center"><h3 class="text-danger mb-0">2</h3><small class="text-muted">مخاطر عالية جداً</small></div></div></div>
            <div class="col-md-3"><div class="card border-warning h-100"><div class="card-body text-center"><h3 class="text-warning mb-0">3</h3><small class="text-muted">مخاطر متوسطة</small></div></div></div>
            <div class="col-md-3"><div class="card border-success h-100"><div class="card-body text-center"><h3 class="text-success mb-0">5</h3><small class="text-muted">مخاطر منخفضة</small></div></div></div>
            <div class="col-md-3"><div class="card border-info h-100"><div class="card-body text-center"><h3 class="text-info mb-0">3</h3><small class="text-muted">مخاطر مغلقة</small></div></div></div>
        </div>
        <div class="card">
            <div class="card-header bg-light d-flex justify-content-between align-items-center">
                <h5 class="mb-0"><i class="bi bi-exclamation-triangle me-2"></i>مخاطر المبادرة</h5>
                <button class="btn btn-danger btn-sm" onclick="addInitiativeRisk('${init.id}')"><i class="bi bi-plus-circle me-1"></i>إضافة مخاطرة</button>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead class="table-light">
                            <tr><th>الإجراء</th><th>رمز الخطر</th><th>اسم الخطر</th><th>الاحتمالية</th><th>الأثر</th><th>مستوى الخطر</th><th>حالة الخطر</th><th>إجراءات التخفيف</th></tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><div class="btn-group btn-group-sm" role="group">
                                    <button class="btn btn-outline-primary" onclick="viewRisk('VRP_1_NID_227', '${init.id}')" title="عرض"><i class="bi bi-eye"></i></button>
                                    <button class="btn btn-outline-warning" onclick="editRisk('VRP_1_NID_227', '${init.id}')" title="تعديل"><i class="bi bi-pencil"></i></button>
                                    <button class="btn btn-outline-danger" onclick="closeRisk('VRP_1_NID_227', '${init.id}')" title="إغلاق"><i class="bi bi-x-circle"></i></button>
                                </div></td>
                                <td class="fw-semibold">VRP_1_NID_227</td>
                                <td>تأخر في تنفيذ المشاريع التقنية</td>
                                <td><span class="badge bg-warning text-dark">متوسط</span></td>
                                <td><span class="badge bg-danger">عالي</span></td>
                                <td><span class="badge bg-danger">حرج</span></td>
                                <td><span class="badge bg-info">مفتوح</span></td>
                                <td><div class="progress" style="height: 20px;"><div class="progress-bar bg-success" style="width: 60%">60%</div></div><small class="text-muted">3 من 5 مكتملة</small></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    `;
}

function viewRisk(riskId, initiativeId) { alert(`عرض تفاصيل المخاطرة: ${riskId}`); }
function editRisk(riskId, initiativeId) { alert(`تعديل المخاطرة: ${riskId}`); }
function closeRisk(riskId, initiativeId) { if (confirm('هل أنت متأكد من إغلاق هذه المخاطرة؟')) alert(`تم إغلاق المخاطرة: ${riskId}`); }
function addInitiativeRisk(initiativeId) { alert(`إضافة مخاطرة جديدة للمبادرة: ${initiativeId}`); }

// ============= Initiative Documents Tab =============
function loadInitiativeDocuments() {
    if (!selectedInitiative) return;
    const init = selectedInitiative;
    document.getElementById('docs-content').innerHTML = `
        <div class="card">
            <div class="card-header bg-light d-flex justify-content-between align-items-center">
                <h5 class="mb-0"><i class="bi bi-file-earmark-text me-2"></i>وثائق المبادرة</h5>
                <button class="btn btn-primary btn-sm" onclick="uploadInitiativeDocument('${init.id}')">
                    <i class="bi bi-upload me-1"></i>رفع وثيقة
                </button>
            </div>
            <div class="card-body">
                <div class="alert alert-info">
                    <i class="bi bi-info-circle me-2"></i> قسم الوثائق قيد التطوير
                </div>
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead class="table-light">
                            <tr>
                                <th>اسم الوثيقة</th>
                                <th>النوع</th>
                                <th>الحجم</th>
                                <th>تاريخ الرفع</th>
                                <th>الرفع بواسطة</th>
                                <th>الإجراءات</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td colspan="6" class="text-center text-muted py-4">
                                    <i class="bi bi-inbox fs-1 d-block mb-2"></i>
                                    لا توجد وثائق مرفوعة حالياً
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    `;
}

function uploadInitiativeDocument(initiativeId) {
    alert(`رفع وثيقة جديدة للمبادرة: ${initiativeId}`);
}

// ============= Other Actions =============
function editInitiative(id) { $('#addInitiativeModal').modal('show'); }
function saveInitiative() { alert('تم حفظ بيانات المبادرة بنجاح'); $('#addInitiativeModal').modal('hide'); }
function viewRisks(id) { window.location.href = `risk-escalation.html?initiative=${id}`; }
function generateReport(id) { window.location.href = `report-generation.html?type=initiative&id=${id}`; }
function exportInitiatives() { alert('جاري تصدير بيانات المبادرات...'); }
function filterInitiatives() { console.log('Filtering initiatives...'); }
function clearFilters() {
    document.getElementById('searchInitiatives').value = '';
    document.getElementById('filterProgram').value = '';
    document.getElementById('filterStatus').value = '';
    document.getElementById('filterType').value = '';
}

// Backward compatibility wrappers
function viewInitiativeKPIs(id, name) { selectInitiative(id, name, 'kpis'); }
function viewInitiativeBudget(id, name) { selectInitiative(id, name, 'budget'); }
function viewInitiativeMilestones(id, name) { selectInitiative(id, name, 'milestones'); }
function viewInitiativeRisks(id, name) { selectInitiative(id, name, 'risks'); }


