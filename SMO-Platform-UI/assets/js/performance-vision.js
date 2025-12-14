/**
 * Performance Vision - Vision Level Performance Management
 * Implements UC-01 (MEI Management) and UC-02 (Strategic Objectives) per BRD
 * Business Rules: BR-01 through BR-41 and AS-01 through AS-11
 */

// ============= Global State =============
let currentMEI = null;
let currentStrategicObjective = null;
let auditLog = [];

// ============= Mock Data (Replace with API calls) =============
const meiMockData = {
    'MEI.001': {
        code: 'MEI.001',
        nameAr: 'الناتج المحلي الإجمالي',
        nameEn: 'Gross Domestic Product',
        unit: 'مليون ريال',
        frequency: 'سنوي',
        source: 'الهيئة العامة للإحصاء',
        polarity: 'متزايد',
        baseline: 2000000,
        baselineDate: '2016-Q2',
        target2030: 3500000,
        currentValue: 2850000,
        formula: 'مجموع الإنتاج المحلي',
        comparativeCountries: 'الإمارات، قطر، البحرين'
    }
};

// ============= Business Rules Implementation =============

/**
 * BR-03, BR-27: Calculate Performance Indicator
 * Formula: ((Actual - Baseline) / (Target - Baseline)) × 100
 */
function calculatePerformanceIndicator(actual, baseline, target, polarity = 'متزايد') {
    if (target === baseline) return 0;
    
    let result = ((actual - baseline) / (target - baseline)) * 100;
    
    // BR-59, AS-05: Apply polarity (متناقص reverses calculation)
    if (polarity === 'متناقص') {
        result = 100 - result;
    }
    
    return Math.round(result * 100) / 100; // Round to 2 decimals
}

/**
 * BR-04, BR-28, AS-03: Validate positive values
 */
function validatePositiveValue(value, fieldName) {
    const num = parseFloat(value);
    if (isNaN(num) || num < 0) {
        showErrorToast(`${fieldName}: القيمة يجب أن تكون رقم موجب`);
        return false;
    }
    return true;
}

/**
 * BR-05, BR-29: Check if period is closed
 * Closed periods are older than current quarter
 */
function isPeriodClosed(year, quarter) {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentQuarter = Math.floor(currentDate.getMonth() / 3) + 1;
    
    if (year < currentYear) return true;
    if (year === currentYear && quarter < currentQuarter) return true;
    
    return false;
}

/**
 * BR-06, BR-30: Audit logging
 */
function logAudit(action, entity, entityId, oldValue, newValue) {
    const entry = {
        timestamp: new Date().toISOString(),
        user: 'فهد محمد القحطاني', // Get from session
        action: action,
        entity: entity,
        entityId: entityId,
        oldValue: oldValue,
        newValue: newValue
    };
    
    auditLog.push(entry);
    console.log('Audit Log:', entry);
    
    // Send to backend
    // await apiService.post('/audit-log', entry);
}

/**
 * BR-08, BR-32: Generate rows based on frequency
 */
function generatePeriodRows(year, frequency) {
    const rows = [];
    
    switch (frequency) {
        case 'سنوي':
            rows.push({ period: year, quarter: null, month: null });
            break;
        case 'ربع سنوي':
            for (let q = 1; q <= 4; q++) {
                rows.push({ period: `${year} - Q${q}`, quarter: q, month: null });
            }
            break;
        case 'شهري':
            const months = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 
                           'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];
            for (let m = 0; m < 12; m++) {
                const q = Math.floor(m / 3) + 1;
                rows.push({ period: `${year} - Q${q} - ${months[m]}`, quarter: q, month: m + 1 });
            }
            break;
    }
    
    return rows;
}

// ============= MEI Edit Modal Functions (UC-01) =============

/**
 * Open MEI Edit Modal - Wire up to Edit buttons
 */
function editMEI(meiCode) {
    console.log('📝 Opening MEI Edit Modal for:', meiCode);
    
    try {
        showLoadingState();
        
        setTimeout(() => {
            try {
                // Load MEI data (mock for now, replace with API)
                const meiData = meiMockData[meiCode] || {
                    code: meiCode,
                    nameAr: 'مؤشر اقتصادي',
                    nameEn: 'Economic Indicator',
                    unit: '%',
                    frequency: 'سنوي',
                    source: 'مصدر البيانات',
                    polarity: 'متزايد',
                    baseline: 0,
                    baselineDate: '2016',
                    target2030: 100,
                    currentValue: 50,
                    formula: '',
                    comparativeCountries: ''
                };
                
                currentMEI = meiData;
                console.log('📊 MEI Data loaded:', meiData);
                
                // Check if modal exists
                const modalElement = document.getElementById('meiEditModal');
                if (!modalElement) {
                    console.error('❌ MEI Edit Modal element not found!');
                    hideLoadingState();
                    alert('خطأ: لم يتم العثور على نافذة التعديل. يرجى تحديث الصفحة.');
                    return;
                }
                
                // Check if fields exist before populating
                const fields = {
                    'meiInfoName': meiData.nameAr,
                    'meiInfoCode': meiData.code,
                    'meiInfoUnit': meiData.unit,
                    'meiInfoFrequency': meiData.frequency,
                    'meiInfoSource': meiData.source || '-',
                    'meiInfoBaseline': meiData.baseline.toLocaleString('ar-SA'),
                    'meiInfoBaselineDate': meiData.baselineDate,
                    'meiInfoTarget2030': meiData.target2030.toLocaleString('ar-SA'),
                    'meiInfoFormula': meiData.formula || '-',
                    'meiInfoCountries': meiData.comparativeCountries || '-'
                };
                
                // Populate fields safely
                for (const [fieldId, value] of Object.entries(fields)) {
                    const element = document.getElementById(fieldId);
                    if (element) {
                        element.textContent = value;
                        console.log(`✅ Set ${fieldId} = ${value}`);
                    } else {
                        console.warn(`⚠️ Field ${fieldId} not found in modal`);
                    }
                }
                
                hideLoadingState();
                console.log('✅ Loading state hidden');
                
                // Show modal
                try {
                    const modal = new bootstrap.Modal(modalElement);
                    modal.show();
                    console.log('✅ Modal shown successfully');
                    
                    // Load actual vs target data for default year
                    setTimeout(() => {
                        try {
                            loadActualTargetData();
                            loadPerformanceFactors(meiCode);
                        } catch (e) {
                            console.error('❌ Error loading additional data:', e);
                        }
                    }, 100);
                    
                } catch (modalError) {
                    console.error('❌ Error showing modal:', modalError);
                    hideLoadingState();
                    alert('خطأ في عرض النافذة. يرجى التحقق من وجود Bootstrap.');
                }
                
            } catch (innerError) {
                console.error('❌ Error in modal setup:', innerError);
                hideLoadingState();
                alert('حدث خطأ في تحميل البيانات');
            }
        }, 500);
        
    } catch (error) {
        console.error('❌ Error in editMEI:', error);
        hideLoadingState();
        alert('حدث خطأ غير متوقع');
    }
}

/**
 * Load Actual vs Target data based on selected year
 * Implements BR-08, BR-09, BR-33, BR-34, BR-35
 */
function loadActualTargetData() {
    try {
        const yearFilter = document.getElementById('actualTargetYearFilter');
        const tbody = document.getElementById('actualTargetTableBody');
        
        if (!yearFilter) {
            console.error('❌ Year filter element not found');
            return;
        }
        
        if (!tbody) {
            console.error('❌ Table body element not found');
            return;
        }
        
        const year = yearFilter.value || '2024';
        const frequency = currentMEI?.frequency || 'سنوي';
        
        console.log(`📅 Loading data for year: ${year}, frequency: ${frequency}`);
        
        tbody.innerHTML = '';
        
        // Generate rows based on frequency (BR-08, BR-32)
        const periods = generatePeriodRows(year, frequency);
    
    periods.forEach((period, index) => {
        const isFuture = parseInt(year) >= 2026; // BR-12: Future years are readonly
        const isClosed = !isFuture && isPeriodClosed(parseInt(year), period.quarter || 1);
        
        const row = document.createElement('tr');
        row.className = isClosed ? 'table-warning' : '';
        row.innerHTML = `
            <td>${period.period}</td>
            <td class="fw-semibold">-</td>
            <td>
                <input type="number" 
                       class="form-control form-control-sm" 
                       id="actual_${index}" 
                       step="0.01" 
                       min="0"
                       ${isFuture || isClosed ? 'readonly' : ''}
                       placeholder="أدخل القيمة">
                ${isClosed ? '<small class="text-warning">فترة مغلقة</small>' : ''}
            </td>
            <td>
                <input type="text" 
                       class="form-control form-control-sm" 
                       id="desc_${index}"
                       maxlength="2000"
                       ${isFuture ? 'readonly' : ''}
                       placeholder="الوصف">
            </td>
            <td>
                ${!isFuture ? `
                    <button class="btn btn-sm btn-success" 
                            onclick="saveActualTargetRow(${index}, '${period.period}', ${period.quarter}, ${period.month})"
                            ${isClosed ? 'title="فترة مغلقة - تتطلب موافقة" class="btn-sm btn-warning"' : ''}>
                        <i class="bi bi-save"></i> حفظ
                    </button>
                ` : '<span class="text-muted small">للقراءة فقط</span>'}
            </td>
        `;
        
        tbody.appendChild(row);
    });
        
        console.log(`✅ Generated ${periods.length} rows for ${frequency} frequency`);
    } catch (error) {
        console.error('❌ Error in loadActualTargetData:', error);
    }
}

/**
 * Save Actual vs Target row
 * Implements BR-06, BR-10, BR-18
 */
async function saveActualTargetRow(index, period, quarter, month) {
    const actualInput = document.getElementById(`actual_${index}`);
    const descInput = document.getElementById(`desc_${index}`);
    
    const actualValue = actualInput.value;
    const description = descInput.value;
    
    // BR-18: Must have actual value or description
    if (!actualValue && !description) {
        showErrorToast('يجب إدخال القيمة الفعلية أو الوصف');
        return;
    }
    
    // BR-04, BR-10: Validate positive number
    if (actualValue && !validatePositiveValue(actualValue, 'القيمة الفعلية')) {
        return;
    }
    
    // BR-11: Description max 2000 chars (already enforced by maxlength)
    
    const year = document.getElementById('actualTargetYearFilter').value;
    
    // BR-05: Check if closed period
    if (isPeriodClosed(parseInt(year), quarter)) {
        if (!confirm('هذه فترة مغلقة. هل لديك موافقة خاصة للتعديل؟')) {
            return;
        }
    }
    
    // BR-06: Audit log
    logAudit('UPDATE', 'MEI_ACTUAL_VALUE', currentMEI.code, null, actualValue);
    
    // Save to backend
    try {
        showLoadingState();
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500));
        
        // const response = await apiService.post('/mei/actual-values', {
        //     meiCode: currentMEI.code,
        //     year: year,
        //     quarter: quarter,
        //     month: month,
        //     actualValue: actualValue,
        //     description: description
        // });
        
        hideLoadingState();
        showSuccessToast('تم حفظ البيانات بنجاح');
        
        // Update button state
        const btn = event.currentTarget;
        btn.classList.remove('btn-success');
        btn.classList.add('btn-secondary');
        btn.innerHTML = '<i class="bi bi-check"></i> محفوظ';
        
        setTimeout(() => {
            btn.classList.remove('btn-secondary');
            btn.classList.add('btn-success');
            btn.innerHTML = '<i class="bi bi-save"></i> حفظ';
        }, 2000);
        
    } catch (error) {
        hideLoadingState();
        showErrorToast('فشل الحفظ، يرجى المحاولة مرة أخرى');
        console.error('Save error:', error);
    }
}

// ============= Performance Drivers Management (BR-24) =============

/**
 * Add Performance Driver
 * Implements BR-24, AS-08
 */
function addDriver() {
    const input = document.getElementById('newDriver');
    const driverText = input.value.trim();
    
    if (!driverText) {
        showWarningToast('يرجى إدخال نص الدافع');
        return;
    }
    
    if (driverText.length > 500) {
        showErrorToast('الحد الأقصى 500 حرف');
        return;
    }
    
    const driversList = document.getElementById('driversList');
    const div = document.createElement('div');
    div.className = 'list-group-item d-flex justify-content-between align-items-center';
    div.innerHTML = `
        <span>${driverText}</span>
        <div>
            <button class="btn btn-sm btn-outline-secondary me-1" onclick="editDriver(this)" title="تعديل">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" onclick="deleteDriver(this)" title="حذف">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    driversList.appendChild(div);
    input.value = '';
    
    // Log audit
    logAudit('CREATE', 'DRIVER', currentMEI.code, null, driverText);
    
    showSuccessToast('تم إضافة الدافع بنجاح');
}

function editDriver(btn) {
    const item = btn.closest('.list-group-item');
    const span = item.querySelector('span');
    const currentText = span.textContent;
    const newText = prompt('تعديل دافع الأداء:', currentText);
    
    if (newText && newText.trim()) {
        if (newText.length > 500) {
            showErrorToast('الحد الأقصى 500 حرف');
            return;
        }
        logAudit('UPDATE', 'DRIVER', currentMEI.code, currentText, newText);
        span.textContent = newText.trim();
        showSuccessToast('تم تعديل الدافع');
    }
}

function deleteDriver(btn) {
    if (confirm('هل أنت متأكد من حذف هذا الدافع؟')) {
        const item = btn.closest('.list-group-item');
        const text = item.querySelector('span').textContent;
        logAudit('DELETE', 'DRIVER', currentMEI.code, text, null);
        item.remove();
        showSuccessToast('تم حذف الدافع');
    }
}

// ============= Performance Obstacles Management (BR-24) =============

function addObstacle() {
    const input = document.getElementById('newObstacle');
    const obstacleText = input.value.trim();
    
    if (!obstacleText) {
        showWarningToast('يرجى إدخال نص المعوق');
        return;
    }
    
    if (obstacleText.length > 500) {
        showErrorToast('الحد الأقصى 500 حرف');
        return;
    }
    
    const obstaclesList = document.getElementById('obstaclesList');
    const div = document.createElement('div');
    div.className = 'list-group-item d-flex justify-content-between align-items-center';
    div.innerHTML = `
        <span>${obstacleText}</span>
        <div>
            <button class="btn btn-sm btn-outline-secondary me-1" onclick="editObstacle(this)" title="تعديل">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" onclick="deleteObstacle(this)" title="حذف">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    obstaclesList.appendChild(div);
    input.value = '';
    
    logAudit('CREATE', 'OBSTACLE', currentMEI.code, null, obstacleText);
    showSuccessToast('تم إضافة المعوق بنجاح');
}

function editObstacle(btn) {
    const item = btn.closest('.list-group-item');
    const span = item.querySelector('span');
    const currentText = span.textContent;
    const newText = prompt('تعديل معوق الأداء:', currentText);
    
    if (newText && newText.trim()) {
        if (newText.length > 500) {
            showErrorToast('الحد الأقصى 500 حرف');
            return;
        }
        logAudit('UPDATE', 'OBSTACLE', currentMEI.code, currentText, newText);
        span.textContent = newText.trim();
        showSuccessToast('تم تعديل المعوق');
    }
}

function deleteObstacle(btn) {
    if (confirm('هل أنت متأكد من حذف هذا المعوق؟')) {
        const item = btn.closest('.list-group-item');
        const text = item.querySelector('span').textContent;
        logAudit('DELETE', 'OBSTACLE', currentMEI.code, text, null);
        item.remove();
        showSuccessToast('تم حذف المعوق');
    }
}

// ============= Brief Explanation Management (BR-23, BR-62, AS-07) =============

function openBriefExplanationPage() {
    // AS-07: Navigate to separate page for brief explanation
    const modal = new bootstrap.Modal(document.getElementById('briefExplanationModal'));
    modal.show();
}

function addExplanation() {
    const modal = new bootstrap.Modal(document.getElementById('addExplanationModal'));
    modal.show();
}

function saveExplanation() {
    const year = document.getElementById('explanationYear').value;
    const quarter = document.getElementById('explanationQuarter').value;
    const text = document.getElementById('explanationText').value.trim();
    
    if (!text) {
        showErrorToast('يرجى إدخال نص الشرح');
        return;
    }
    
    // Check if duplicate exists
    const table = document.getElementById('explanationsTable');
    const existing = Array.from(table.querySelectorAll('tr')).find(row => {
        const cells = row.querySelectorAll('td');
        return cells[0]?.textContent === year && cells[1]?.textContent === quarter;
    });
    
    if (existing) {
        showErrorToast('يوجد شرح موجود لهذه الفترة. يرجى التعديل بدلاً من الإضافة');
        return;
    }
    
    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>${year}</td>
        <td>${quarter}</td>
        <td>${text}</td>
        <td>
            <button class="btn btn-sm btn-outline-secondary" onclick="editExplanation(this)" title="تعديل">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" onclick="deleteExplanation(this)" title="حذف">
                <i class="bi bi-trash"></i>
            </button>
        </td>
    `;
    table.appendChild(tr);
    
    logAudit('CREATE', 'BRIEF_EXPLANATION', currentMEI.code, null, `${year}-${quarter}: ${text}`);
    
    bootstrap.Modal.getInstance(document.getElementById('addExplanationModal')).hide();
    document.getElementById('explanationText').value = '';
    
    showSuccessToast('تم إضافة الشرح بنجاح');
}

function editExplanation(btn) {
    const row = btn.closest('tr');
    const cells = row.querySelectorAll('td');
    const currentText = cells[2].textContent;
    
    const newText = prompt('تعديل الشرح الموجز:', currentText);
    
    if (newText && newText.trim()) {
        logAudit('UPDATE', 'BRIEF_EXPLANATION', currentMEI.code, currentText, newText);
        cells[2].textContent = newText.trim();
        showSuccessToast('تم تعديل الشرح');
    }
}

function deleteExplanation(btn) {
    if (confirm('هل أنت متأكد من حذف هذا الشرح؟')) {
        const row = btn.closest('tr');
        const cells = row.querySelectorAll('td');
        const text = `${cells[0].textContent}-${cells[1].textContent}: ${cells[2].textContent}`;
        logAudit('DELETE', 'BRIEF_EXPLANATION', currentMEI.code, text, null);
        row.remove();
        showSuccessToast('تم حذف الشرح');
    }
}

// ============= Calculation Formula Modal (Screen 04, BR-20) =============

function openFormulaCalculation(meiCode) {
    const modal = new bootstrap.Modal(document.getElementById('calculationModal'));
    modal.show();
    
    // Load formula parameters (mock data - replace with API)
    const tbody = document.getElementById('calculationTableBody');
    tbody.innerHTML = `
        <tr>
            <td><input type="text" class="form-control" readonly value="الناتج المحلي"></td>
            <td><input type="number" class="form-control" id="calc_param_1" step="0.01" value="2850000"></td>
            <td><input type="text" class="form-control" readonly value="مليون ريال"></td>
            <td><input type="text" class="form-control" readonly value="الهيئة العامة للإحصاء"></td>
        </tr>
        <tr>
            <td><input type="text" class="form-control" readonly value="معدل النمو"></td>
            <td><input type="number" class="form-control" id="calc_param_2" step="0.01" value="3.5"></td>
            <td><input type="text" class="form-control" readonly value="%"></td>
            <td><input type="text" class="form-control" readonly value="البنك المركزي"></td>
        </tr>
    `;
}

function executeCalculation() {
    // Get all parameter values from calculation table
    const param1 = parseFloat(document.getElementById('calc_param_1')?.value || 0);
    const param2 = parseFloat(document.getElementById('calc_param_2')?.value || 0);
    
    // Execute formula (simplified - real formula varies per indicator)
    const calculatedValue = param1 * (1 + param2 / 100);
    
    showSuccessToast(`القيمة المحسوبة: ${calculatedValue.toLocaleString('ar-SA')}`);
    
    // Close calculation modal
    bootstrap.Modal.getInstance(document.getElementById('calculationModal')).hide();
    
    // Could auto-populate the actual value field here
}

// ============= Strategic Objectives Edit (UC-02) =============

function editStrategicObjective(objId, level) {
    showLoadingState();
    
    setTimeout(() => {
        // Load data (mock)
        const objData = {
            code: 'SO' + level + '.' + objId,
            nameAr: 'هدف استراتيجي',
            unit: '%',
            frequency: 'ربع سنوي',
            source: 'مركز معلومات الرؤية',
            baseline: 0,
            baselineDate: '2015-Q1',
            targetValue: 100
        };
        
        currentStrategicObjective = objData;
        
        // Populate modal
        document.getElementById('soInfoName').textContent = objData.nameAr;
        document.getElementById('soInfoCode').textContent = objData.code;
        document.getElementById('soInfoUnit').textContent = objData.unit;
        document.getElementById('soInfoFrequency').textContent = objData.frequency;
        document.getElementById('soInfoSource').textContent = objData.source;
        document.getElementById('soInfoBaseline').textContent = objData.baseline;
        document.getElementById('soInfoBaselineDate').textContent = objData.baselineDate;
        document.getElementById('soInfoTargetValue').textContent = objData.targetValue;
        
        hideLoadingState();
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('strategicObjectiveEditModal'));
        modal.show();
        
        // Load actual vs target data
        loadStrategicActualTargetData();
        
        // Load performance factors
        loadStrategicPerformanceFactors(objData.code);
    }, 500);
}

function loadStrategicActualTargetData() {
    const year = document.getElementById('soActualTargetYearFilter').value;
    const frequency = currentStrategicObjective?.frequency || 'ربع سنوي';
    
    const tbody = document.getElementById('soActualTargetTableBody');
    tbody.innerHTML = '';
    
    const periods = generatePeriodRows(year, frequency);
    
    periods.forEach((period, index) => {
        const isFuture = parseInt(year) >= 2026;
        const isClosed = !isFuture && isPeriodClosed(parseInt(year), period.quarter || 1);
        
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${period.period}</td>
            <td class="fw-semibold">-</td>
            <td>
                <input type="number" class="form-control form-control-sm" id="so_actual_${index}" step="0.01" min="0" ${isFuture || isClosed ? 'readonly' : ''}>
            </td>
            <td>
                <input type="text" class="form-control form-control-sm" id="so_desc_${index}" maxlength="2000" ${isFuture ? 'readonly' : ''}>
            </td>
            <td>
                ${!isFuture ? `
                    <button class="btn btn-sm btn-success" onclick="saveStrategicActualTargetRow(${index}, '${period.period}', ${period.quarter}, ${period.month})">
                        <i class="bi bi-save"></i> حفظ
                    </button>
                ` : '<span class="text-muted small">للقراءة فقط</span>'}
            </td>
        `;
        
        tbody.appendChild(row);
    });
}

async function saveStrategicActualTargetRow(index, period, quarter, month) {
    const actualInput = document.getElementById(`so_actual_${index}`);
    const descInput = document.getElementById(`so_desc_${index}`);
    
    const actualValue = actualInput.value;
    const description = descInput.value;
    
    if (!actualValue && !description) {
        showErrorToast('يجب إدخال القيمة الفعلية أو الوصف');
        return;
    }
    
    if (actualValue && !validatePositiveValue(actualValue, 'القيمة الفعلية')) {
        return;
    }
    
    const year = document.getElementById('soActualTargetYearFilter').value;
    
    if (isPeriodClosed(parseInt(year), quarter)) {
        if (!confirm('هذه فترة مغلقة. هل لديك موافقة خاصة للتعديل؟')) {
            return;
        }
    }
    
    logAudit('UPDATE', 'SO_ACTUAL_VALUE', currentStrategicObjective.code, null, actualValue);
    
    try {
        showLoadingState();
        await new Promise(resolve => setTimeout(resolve, 500));
        hideLoadingState();
        
        showSuccessToast('تم حفظ البيانات بنجاح');
        
        const btn = event.currentTarget;
        btn.classList.remove('btn-success');
        btn.classList.add('btn-secondary');
        btn.innerHTML = '<i class="bi bi-check"></i> محفوظ';
        
        setTimeout(() => {
            btn.classList.remove('btn-secondary');
            btn.classList.add('btn-success');
            btn.innerHTML = '<i class="bi bi-save"></i> حفظ';
        }, 2000);
        
    } catch (error) {
        hideLoadingState();
        showErrorToast('فشل الحفظ، يرجى المحاولة مرة أخرى');
    }
}

// ============= Load Performance Factors =============

function loadPerformanceFactors(entityCode) {
    // Load drivers, obstacles, and explanations from backend
    // Mock implementation for now
    const driversList = document.getElementById('driversList');
    const obstaclesList = document.getElementById('obstaclesList');
    
    // Clear existing
    driversList.innerHTML = '<div class="text-muted small">لا توجد دوافع مضافة</div>';
    obstaclesList.innerHTML = '<div class="text-muted small">لا توجد معوقات مضافة</div>';
}

function loadStrategicPerformanceFactors(entityCode) {
    // Same as above but for strategic objectives
    const driversList = document.getElementById('soDriversList');
    const obstaclesList = document.getElementById('soObstaclesList');
    
    if (driversList) driversList.innerHTML = '<div class="text-muted small">لا توجد دوافع مضافة</div>';
    if (obstaclesList) obstaclesList.innerHTML = '<div class="text-muted small">لا توجد معوقات مضافة</div>';
}

// ============= Strategic Objective Drivers/Obstacles (UC-02) =============

function addSODriver() {
    const input = document.getElementById('newSODriver');
    const driverText = input.value.trim();
    
    if (!driverText) {
        showWarningToast('يرجى إدخال نص الدافع');
        return;
    }
    
    if (driverText.length > 500) {
        showErrorToast('الحد الأقصى 500 حرف');
        return;
    }
    
    const driversList = document.getElementById('soDriversList');
    
    // Remove "no data" message if exists
    if (driversList.querySelector('.text-muted')) {
        driversList.innerHTML = '';
    }
    
    const div = document.createElement('div');
    div.className = 'list-group-item d-flex justify-content-between align-items-center';
    div.innerHTML = `
        <span>${driverText}</span>
        <div>
            <button class="btn btn-sm btn-outline-secondary me-1" onclick="editSODriver(this)">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" onclick="deleteSODriver(this)">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    driversList.appendChild(div);
    input.value = '';
    
    logAudit('CREATE', 'SO_DRIVER', currentStrategicObjective.code, null, driverText);
    showSuccessToast('تم إضافة الدافع بنجاح');
}

function editSODriver(btn) {
    const item = btn.closest('.list-group-item');
    const span = item.querySelector('span');
    const currentText = span.textContent;
    const newText = prompt('تعديل دافع الأداء:', currentText);
    
    if (newText && newText.trim()) {
        if (newText.length > 500) {
            showErrorToast('الحد الأقصى 500 حرف');
            return;
        }
        logAudit('UPDATE', 'SO_DRIVER', currentStrategicObjective.code, currentText, newText);
        span.textContent = newText.trim();
        showSuccessToast('تم تعديل الدافع');
    }
}

function deleteSODriver(btn) {
    if (confirm('هل أنت متأكد من حذف هذا الدافع؟')) {
        const item = btn.closest('.list-group-item');
        const text = item.querySelector('span').textContent;
        logAudit('DELETE', 'SO_DRIVER', currentStrategicObjective.code, text, null);
        item.remove();
        
        const list = document.getElementById('soDriversList');
        if (list.children.length === 0) {
            list.innerHTML = '<div class="text-muted small">لا توجد دوافع مضافة</div>';
        }
        
        showSuccessToast('تم حذف الدافع');
    }
}

function addSOObstacle() {
    const input = document.getElementById('newSOObstacle');
    const obstacleText = input.value.trim();
    
    if (!obstacleText) {
        showWarningToast('يرجى إدخال نص المعوق');
        return;
    }
    
    if (obstacleText.length > 500) {
        showErrorToast('الحد الأقصى 500 حرف');
        return;
    }
    
    const obstaclesList = document.getElementById('soObstaclesList');
    
    // Remove "no data" message if exists
    if (obstaclesList.querySelector('.text-muted')) {
        obstaclesList.innerHTML = '';
    }
    
    const div = document.createElement('div');
    div.className = 'list-group-item d-flex justify-content-between align-items-center';
    div.innerHTML = `
        <span>${obstacleText}</span>
        <div>
            <button class="btn btn-sm btn-outline-secondary me-1" onclick="editSOObstacle(this)">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" onclick="deleteSOObstacle(this)">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    obstaclesList.appendChild(div);
    input.value = '';
    
    logAudit('CREATE', 'SO_OBSTACLE', currentStrategicObjective.code, null, obstacleText);
    showSuccessToast('تم إضافة المعوق بنجاح');
}

function editSOObstacle(btn) {
    const item = btn.closest('.list-group-item');
    const span = item.querySelector('span');
    const currentText = span.textContent;
    const newText = prompt('تعديل معوق الأداء:', currentText);
    
    if (newText && newText.trim()) {
        if (newText.length > 500) {
            showErrorToast('الحد الأقصى 500 حرف');
            return;
        }
        logAudit('UPDATE', 'SO_OBSTACLE', currentStrategicObjective.code, currentText, newText);
        span.textContent = newText.trim();
        showSuccessToast('تم تعديل المعوق');
    }
}

function deleteSOObstacle(btn) {
    if (confirm('هل أنت متأكد من حذف هذا المعوق؟')) {
        const item = btn.closest('.list-group-item');
        const text = item.querySelector('span').textContent;
        logAudit('DELETE', 'SO_OBSTACLE', currentStrategicObjective.code, text, null);
        item.remove();
        
        const list = document.getElementById('soObstaclesList');
        if (list.children.length === 0) {
            list.innerHTML = '<div class="text-muted small">لا توجد معوقات مضافة</div>';
        }
        
        showSuccessToast('تم حذف المعوق');
    }
}

// ============= Toast Notifications =============

function showToast(message, type = 'info') {
    const toastContainer = document.getElementById('toastContainer') || createToastContainer();
    const toastId = 'toast_' + Date.now();
    
    const bgClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';
    
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toast = new bootstrap.Toast(document.getElementById(toastId));
    toast.show();
    
    setTimeout(() => document.getElementById(toastId)?.remove(), 5000);
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
    return container;
}

function showSuccessToast(message) {
    showToast(message, 'success');
}

function showErrorToast(message) {
    showToast(message, 'error');
}

function showWarningToast(message) {
    showToast(message, 'warning');
}

function showInfoToast(message) {
    showToast(message, 'info');
}

// ============= Loading States =============

function showLoadingState() {
    console.log('🔄 Showing loading state...');
    
    // Clear any existing loader first
    hideLoadingState();
    
    let loader = document.getElementById('globalLoader');
    if (!loader) {
        loader = document.createElement('div');
        loader.id = 'globalLoader';
        loader.className = 'loading-overlay';
        loader.innerHTML = `
            <div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 9999; display: flex; align-items: center; justify-content: center;">
                <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status">
                    <span class="visually-hidden">جاري التحميل...</span>
                </div>
            </div>
        `;
        document.body.appendChild(loader);
    }
    loader.style.display = 'block';
    
    // Failsafe: Auto-hide after 5 seconds to prevent getting stuck
    setTimeout(() => {
        const stillExists = document.getElementById('globalLoader');
        if (stillExists) {
            console.warn('⚠️ Loading state timeout - auto-hiding after 5 seconds');
            hideLoadingState();
        }
    }, 5000);
}

function hideLoadingState() {
    console.log('🔄 Hiding loading state...');
    // Try multiple ways to ensure loading is removed
    
    // Method 1: Get by ID and remove
    const loader = document.getElementById('globalLoader');
    if (loader) {
        loader.style.display = 'none';
        loader.remove();
    }
    
    // Method 2: Query selector for any loading overlays
    const allLoaders = document.querySelectorAll('#globalLoader, .loading-overlay, .spinner-border');
    allLoaders.forEach(loader => {
        if (loader.id === 'globalLoader' || loader.classList.contains('loading-overlay')) {
            loader.remove();
        }
    });
    
    // Method 3: Remove any fixed overlays that might be stuck
    const overlays = document.querySelectorAll('div[style*="position: fixed"][style*="z-index: 9999"]');
    overlays.forEach(overlay => {
        if (overlay.querySelector('.spinner-border')) {
            overlay.remove();
        }
    });
    
    // Enable body scroll in case it was disabled
    document.body.style.overflow = '';
    document.body.style.pointerEvents = '';
    
    console.log('✅ Loading state cleared');
}

// ============= Quarterly Explanation Check (BR-23, BR-62) =============

function checkQuarterlyExplanations() {
    const currentYear = new Date().getFullYear();
    const currentQuarter = Math.floor(new Date().getMonth() / 3) + 1;
    
    // Check if explanation exists for current quarter
    // In real app, check via API
    const hasExplanation = false; // Simplified
    
    if (!hasExplanation) {
        setTimeout(() => {
            showWarningToast(`تنبيه: يجب إدخال شرح موجز للأداء للربع ${currentQuarter} من ${currentYear}`);
        }, 3000);
    }
}

// ============= Utility Functions =============

function viewMEI(meiCode) {
    editMEI(meiCode); // Reuse edit modal for view
}

// Diagnostic function to check what's missing
function diagnosePerformanceVision() {
    console.log('🔍 Diagnosing Performance Vision Issues...\n');
    
    // Check 1: Bootstrap
    if (typeof bootstrap === 'undefined') {
        console.error('❌ CRITICAL: Bootstrap is not loaded!');
        return false;
    } else {
        console.log('✅ Bootstrap is loaded');
    }
    
    // Check 2: Main modal container
    const modalElement = document.getElementById('meiEditModal');
    if (!modalElement) {
        console.error('❌ CRITICAL: MEI Edit Modal (#meiEditModal) not found in DOM!');
        console.log('   This means the modal HTML is missing from the page.');
        console.log('   The modal should be added before the closing </body> tag.');
        return false;
    } else {
        console.log('✅ MEI Edit Modal container found');
    }
    
    // Check 3: Modal fields
    const requiredFields = [
        'meiInfoName', 'meiInfoCode', 'meiInfoUnit', 'meiInfoFrequency',
        'meiInfoSource', 'meiInfoBaseline', 'meiInfoBaselineDate',
        'meiInfoTarget2030', 'actualTargetYearFilter', 'actualTargetTableBody'
    ];
    
    let missingFields = [];
    for (const fieldId of requiredFields) {
        if (!document.getElementById(fieldId)) {
            missingFields.push(fieldId);
        }
    }
    
    if (missingFields.length > 0) {
        console.error('❌ Missing modal fields:', missingFields);
        console.log('   These fields are required inside the modal but were not found.');
    } else {
        console.log('✅ All required modal fields found');
    }
    
    // Check 4: Edit buttons
    const editButtons = document.querySelectorAll('#mei-content .btn-outline-primary[title="تعديل"]');
    console.log(`📊 Found ${editButtons.length} MEI edit buttons`);
    
    // Check 5: Try to create modal instance
    if (modalElement) {
        try {
            const testModal = new bootstrap.Modal(modalElement);
            console.log('✅ Can create Bootstrap modal instance');
            
            // Try to show and immediately hide
            testModal.show();
            setTimeout(() => {
                testModal.hide();
                console.log('✅ Modal show/hide test successful');
            }, 500);
        } catch (e) {
            console.error('❌ Error creating modal instance:', e);
        }
    }
    
    console.log('\n🔍 Diagnosis complete. Check above for issues.');
    return true;
}

// Test function - Can be called from browser console
function testPerformanceVision() {
    console.log('📋 Testing Performance Vision Module...');
    
    // Test 1: Check if modals exist
    const modals = {
        'MEI Edit Modal': document.getElementById('meiEditModal'),
        'Strategic Objective Modal': document.getElementById('strategicObjectiveEditModal'),
        'Calculation Modal': document.getElementById('calculationModal'),
        'Add Explanation Modal': document.getElementById('addExplanationModal')
    };
    
    let allModalsExist = true;
    for (const [name, modal] of Object.entries(modals)) {
        if (modal) {
            console.log(`✅ ${name} exists`);
        } else {
            console.error(`❌ ${name} NOT FOUND`);
            allModalsExist = false;
        }
    }
    
    // Test 2: Check if edit buttons exist
    const meiButtons = document.querySelectorAll('#mei-content .btn-outline-primary[title="تعديل"]');
    console.log(`📊 Found ${meiButtons.length} MEI edit buttons`);
    
    const soL1Buttons = document.querySelectorAll('#strategic-1-content .btn-outline-primary[title="تعديل"]');
    console.log(`📊 Found ${soL1Buttons.length} Strategic L1 edit buttons`);
    
    const soL2Buttons = document.querySelectorAll('#strategic-2-content .btn-outline-primary[title="تعديل"]');
    console.log(`📊 Found ${soL2Buttons.length} Strategic L2 edit buttons`);
    
    // Test 3: Try opening MEI modal
    if (allModalsExist) {
        console.log('🔧 Testing MEI modal opening...');
        try {
            editMEI('MEI.TEST');
            console.log('✅ MEI modal opened successfully');
            setTimeout(() => {
                const modal = bootstrap.Modal.getInstance(document.getElementById('meiEditModal'));
                if (modal) modal.hide();
            }, 2000);
        } catch (e) {
            console.error('❌ Error opening MEI modal:', e);
        }
    }
    
    console.log('✅ Test complete! Check above for any errors.');
    return allModalsExist;
}

// Emergency reset function if page gets stuck
function resetPage() {
    console.log('🔧 Running emergency reset...');
    
    // Hide all modals
    const allModals = document.querySelectorAll('.modal');
    allModals.forEach(modal => {
        const modalInstance = bootstrap.Modal.getInstance(modal);
        if (modalInstance) {
            modalInstance.hide();
        }
    });
    
    // Remove all backdrops
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => backdrop.remove());
    
    // Clear loading state
    hideLoadingState();
    
    // Reset body styles
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
    document.body.style.pointerEvents = '';
    
    console.log('✅ Page reset complete');
}

// Make test functions globally available
window.testPerformanceVision = testPerformanceVision;
window.diagnosePerformanceVision = diagnosePerformanceVision;
window.editMEI = editMEI; // Make available for direct testing
window.hideLoadingState = hideLoadingState; // In case loading gets stuck
window.resetPage = resetPage; // Emergency reset function

// Global error handler to prevent stuck states
window.addEventListener('error', function(e) {
    console.error('❌ Global error caught:', e.error);
    hideLoadingState();
});

window.addEventListener('unhandledrejection', function(e) {
    console.error('❌ Unhandled promise rejection:', e.reason);
    hideLoadingState();
});

// ============= Additional Dropdown Menu Functions =============

/**
 * View trends for an indicator
 */
function viewTrends(code) {
    console.log(`📈 Viewing trends for ${code}`);
    showInfoToast(`عرض الاتجاهات للمؤشر ${code}`);
}

/**
 * Compare with benchmark
 */
function compareBenchmark(code) {
    console.log(`📊 Comparing benchmark for ${code}`);
    showInfoToast(`المقارنة المعيارية للمؤشر ${code}`);
}

/**
 * View brief explanation
 */
function viewBriefExplanation(code) {
    console.log(`💬 Viewing brief explanation for ${code}`);
    showInfoToast(`عرض الشرح الموجز للمؤشر ${code}`);
}

/**
 * View MEI history
 */
function viewMEIHistory(code) {
    console.log(`📜 Viewing history for ${code}`);
    showInfoToast(`عرض السجل التاريخي للمؤشر ${code}`);
}

/**
 * View change log
 */
function viewChangeLog(code) {
    console.log(`📋 Viewing change log for ${code}`);
    showInfoToast(`عرض سجل التغييرات للمؤشر ${code}`);
}

/**
 * Generate performance report
 */
function generateReport(code) {
    console.log(`📄 Generating report for ${code}`);
    showInfoToast(`توليد تقرير الأداء للمؤشر ${code}`);
}

/**
 * Export data in various formats
 */
function exportData(code, format) {
    console.log(`💾 Exporting ${code} as ${format}`);
    showInfoToast(`جاري تصدير البيانات بصيغة ${format}`);
}

/**
 * Delete MEI indicator
 */
function deleteMEI(code) {
    console.log(`🗑️ Delete request for ${code}`);
    if (confirm(`هل أنت متأكد من حذف المؤشر ${code}؟`)) {
        showSuccessToast(`تم حذف المؤشر ${code}`);
    }
}

/**
 * View performance drivers
 */
function viewPerformanceDrivers(code) {
    console.log(`⬆️ Viewing performance drivers for ${code}`);
    showInfoToast(`عرض دوافع الأداء للمؤشر ${code}`);
}

/**
 * View performance obstacles
 */
function viewObstacles(code) {
    console.log(`⚠️ Viewing obstacles for ${code}`);
    showInfoToast(`عرض معوقات الأداء للمؤشر ${code}`);
}

/**
 * View actual vs target data
 */
function viewActualTarget(code) {
    console.log(`📊 Viewing actual vs target for ${code}`);
    showInfoToast(`عرض الفعلي مقابل المستهدف للمؤشر ${code}`);
}

// Strategic Objectives specific functions
function viewStrategicObjective(id, level) {
    console.log(`👁️ Viewing strategic objective ${id} at level ${level}`);
    showInfoToast(`عرض الهدف الاستراتيجي ${id}`);
}

function viewObjectivePerformance(id, level) {
    console.log(`📈 Viewing objective performance ${id} at level ${level}`);
    showInfoToast(`عرض أداء الهدف ${id}`);
}

function viewObjectiveDrivers(id, level) {
    console.log(`⬆️ Viewing objective drivers ${id} at level ${level}`);
    showInfoToast(`عرض دوافع الأداء للهدف ${id}`);
}

function deleteStrategicObjective(id, level) {
    console.log(`🗑️ Delete request for strategic objective ${id} at level ${level}`);
    if (confirm(`هل أنت متأكد من حذف الهدف الاستراتيجي ${id}؟`)) {
        showSuccessToast(`تم حذف الهدف الاستراتيجي ${id}`);
    }
}

// Helper function to show info toast
function showInfoToast(message) {
    const toast = document.createElement('div');
    toast.className = 'toast-notification toast-info';
    toast.innerHTML = `<i class="bi bi-info-circle"></i> ${message}`;
    toast.style.cssText = `
        position: fixed;
        bottom: 20px;
        right: 20px;
        background: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        display: flex;
        align-items: center;
        gap: 0.75rem;
        z-index: 2000;
        border-left: 4px solid #0dcaf0;
    `;
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ============= Initialize on Page Load =============

document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 Performance Vision JS Starting...');
    
    // Check if Bootstrap is loaded
    if (typeof bootstrap === 'undefined') {
        console.error('❌ Bootstrap not loaded! Modals will not work.');
        return;
    } else {
        console.log('✅ Bootstrap loaded');
    }
    
    // Initialize tooltips
    try {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl);
        });
        console.log('✅ Tooltips initialized');
    } catch (e) {
        console.error('❌ Tooltip initialization error:', e);
    }
    
    // Check quarterly explanations (BR-23, BR-62)
    try {
        checkQuarterlyExplanations();
        console.log('✅ Quarterly explanations checked');
    } catch (e) {
        console.error('❌ Quarterly check error:', e);
    }
    
    // Wire up edit buttons in existing tables
    try {
        wireUpEditButtons();
    } catch (e) {
        console.error('❌ Button wiring error:', e);
    }
    
    // Setup modal close handlers to prevent stuck loading states
    try {
        setupModalCloseHandlers();
        console.log('✅ Modal close handlers setup');
    } catch (e) {
        console.error('❌ Modal handlers error:', e);
    }
    
    console.log('✅ Performance Vision JS fully loaded and initialized');
});

// ============= Setup Modal Close Handlers =============

function setupModalCloseHandlers() {
    // List of all modal IDs in the application
    const modalIds = [
        'meiEditModal',
        'strategicObjectiveEditModal',
        'addExplanationModal', 
        'calculationModal'
    ];
    
    modalIds.forEach(modalId => {
        const modalElement = document.getElementById(modalId);
        if (modalElement) {
            // Handle when modal is closed by any means (X button, backdrop click, ESC key)
            modalElement.addEventListener('hidden.bs.modal', function () {
                console.log(`🔄 Modal ${modalId} closed - clearing loading state`);
                hideLoadingState();
                // Fix Bootstrap aria-hidden focus issue
                document.activeElement.blur();
                document.body.focus();
            });
            
            // Handle when modal hide starts (before animation)
            modalElement.addEventListener('hide.bs.modal', function () {
                console.log(`🔄 Modal ${modalId} hiding - clearing loading state`);
                hideLoadingState();
                // Remove focus from modal elements to prevent aria-hidden warning
                const activeElement = document.activeElement;
                if (activeElement && this.contains(activeElement)) {
                    activeElement.blur();
                }
            });
            
            // Find all close buttons in the modal and add explicit handlers
            const closeButtons = modalElement.querySelectorAll('[data-bs-dismiss="modal"], .btn-close');
            closeButtons.forEach(btn => {
                btn.addEventListener('click', function() {
                    console.log(`🔄 Close button clicked in ${modalId}`);
                    hideLoadingState();
                });
            });
        }
    });
    
    // Global escape key handler to clear loading state
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' || e.keyCode === 27) {
            console.log('🔄 ESC key pressed - clearing loading state');
            hideLoadingState();
        }
    });
    
    // Failsafe: Clear loading state if user clicks on backdrop
    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('modal')) {
            console.log('🔄 Modal backdrop clicked - clearing loading state');
            setTimeout(hideLoadingState, 100);
        }
    });
}

// ============= Wire Up Edit Buttons =============

function wireUpEditButtons() {
    // MEI table edit buttons - The MEI code is in the 6th column
    document.querySelectorAll('#mei-content .btn-outline-primary').forEach(btn => {
        if (btn.title === 'تعديل') {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const row = this.closest('tr');
                // Find the badge with MEI code - it's in the row
                const codeCell = row.querySelector('.badge');
                const meiCode = codeCell?.textContent.trim() || 'MEI.001';
                console.log('Edit MEI clicked:', meiCode); // Debug log
                editMEI(meiCode);
            });
        }
    });
    
    // View buttons for MEI
    document.querySelectorAll('#mei-content .btn-outline-info').forEach(btn => {
        if (btn.title === 'عرض') {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const row = this.closest('tr');
                const codeCell = row.querySelector('.badge');
                const meiCode = codeCell?.textContent.trim() || 'MEI.001';
                console.log('View MEI clicked:', meiCode); // Debug log
                viewMEI(meiCode);
            });
        }
    });
    
    // Strategic Objectives L1 edit buttons
    document.querySelectorAll('#strategic-1-content .btn-outline-primary').forEach(btn => {
        if (btn.title === 'تعديل') {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const row = this.closest('tr');
                const codeCell = row.querySelector('.badge');
                const objCode = codeCell?.textContent.trim() || 'SO1.001';
                console.log('Edit SO L1 clicked:', objCode); // Debug log
                editStrategicObjective(objCode.split('.')[1], 1);
            });
        }
    });
    
    // Strategic Objectives L2 edit buttons
    document.querySelectorAll('#strategic-2-content .btn-outline-primary').forEach(btn => {
        if (btn.title === 'تعديل') {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const row = this.closest('tr');
                const codeCell = row.querySelector('.badge');
                const objCode = codeCell?.textContent.trim() || 'SO2.001';
                console.log('Edit SO L2 clicked:', objCode); // Debug log
                editStrategicObjective(objCode.split('.')[1], 2);
            });
        }
    });
    
    console.log('✅ Edit buttons wired up successfully');
}

