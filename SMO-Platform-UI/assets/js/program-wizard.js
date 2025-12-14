/**
 * Program Wizard JavaScript Module
 * BRD-Compliant Implementation for Performance Management
 * Covers UC-05 to UC-12 including KPIs, Achievements, Risks, and Support Requirements
 */

// Global data stores
let kpiData = [];
let achievementsData = [];
let risksData = [];
let supportRequestsData = [];
let budgetData = {};
let economicIndicatorsData = [];

// ============================================
// KPI Management (UC-06)
// ============================================

/**
 * Initialize KPI data with mock values aligned to BRD requirements
 */
function initializeKPIData() {
    kpiData = [
        {
            id: 'KPI-001',
            code: 'برنامج-2025-01',
            nameAr: 'نسبة التحول الرقمي في القطاع الحكومي',
            nameEn: 'Digital Transformation Rate in Government Sector',
            unit: '%',
            baseline: 45,
            target: 85,
            actual: 72,
            year: 2025,
            quarter: 'Q1',
            frequency: 'quarterly',
            polarity: 'increasing',
            status: 'near',
            verified: false,
            source: 'system',
            formula: '(عدد الخدمات المحولة رقمياً / إجمالي الخدمات) × 100',
            lastUpdate: '2025-01-10',
            contributingFactors: {
                drivers: ['تبني التقنيات الحديثة', 'دعم القيادة العليا'],
                obstacles: ['مقاومة التغيير', 'قيود الميزانية']
            }
        },
        {
            id: 'KPI-002', 
            code: 'برنامج-2025-02',
            nameAr: 'عدد الوظائف المستحدثة في القطاع الخاص',
            nameEn: 'New Jobs Created in Private Sector',
            unit: 'وظيفة',
            baseline: 50000,
            target: 150000,
            actual: 125000,
            year: 2025,
            quarter: 'Q1',
            frequency: 'annual',
            polarity: 'increasing',
            status: 'achieved',
            verified: true,
            source: 'manual',
            lastUpdate: '2025-01-08'
        }
    ];
}

/**
 * Load and display KPI data
 */
function loadKPIData() {
    updateKPIStatistics();
    populateKPITable();
    loadDriversAndObstacles();
}

/**
 * Update KPI statistics cards
 */
function updateKPIStatistics() {
    const total = kpiData.length;
    const achieved = kpiData.filter(kpi => kpi.status === 'achieved').length;
    const near = kpiData.filter(kpi => kpi.status === 'near').length;
    const behind = kpiData.filter(kpi => kpi.status === 'behind').length;
    
    const avgPerformance = kpiData.reduce((sum, kpi) => {
        const perf = calculateKPIPerformance(kpi);
        return sum + perf;
    }, 0) / (total || 1);
    
    // Update DOM
    updateElement('achieved-count', achieved);
    updateElement('near-count', near); 
    updateElement('behind-count', behind);
    updateElement('avg-performance', avgPerformance.toFixed(1) + '%');
    
    // Update progress bar
    const progressBar = document.getElementById('performance-progress');
    if (progressBar) {
        progressBar.style.width = avgPerformance + '%';
        progressBar.className = 'progress-bar ' + getProgressBarClass(avgPerformance);
    }
}

/**
 * Calculate KPI performance percentage (BR-27)
 */
function calculateKPIPerformance(kpi) {
    if (!kpi.target || !kpi.baseline) return 0;
    return ((kpi.actual - kpi.baseline) / (kpi.target - kpi.baseline)) * 100;
}

/**
 * Calculate deviation percentage
 */
function calculateDeviation(kpi) {
    if (!kpi.target) return 0;
    return ((kpi.actual - kpi.target) / kpi.target) * 100;
}

/**
 * Populate KPI table with data
 */
function populateKPITable() {
    const tableBody = document.getElementById('kpi-table-body');
    if (!tableBody) return;
    
    const filteredKPIs = applyKPIFilters();
    
    tableBody.innerHTML = filteredKPIs.map(kpi => {
        const performance = calculateKPIPerformance(kpi);
        const deviation = calculateDeviation(kpi);
        
        return `
            <tr>
                <td><input type="checkbox" class="kpi-select" value="${kpi.id}"></td>
                <td>${kpi.code}</td>
                <td>
                    <div>${kpi.nameAr}</div>
                    <small class="text-muted">${kpi.nameEn}</small>
                </td>
                <td>${kpi.unit}</td>
                <td>${kpi.baseline.toLocaleString()}</td>
                <td>${kpi.target.toLocaleString()}</td>
                <td class="fw-bold">${kpi.actual.toLocaleString()}</td>
                <td class="${deviation >= 0 ? 'text-success' : 'text-danger'}">
                    ${deviation >= 0 ? '+' : ''}${deviation.toFixed(1)}%
                </td>
                <td>
                    <div class="progress" style="height: 20px;">
                        <div class="progress-bar ${getProgressBarClass(performance)}" 
                             style="width: ${Math.min(performance, 100)}%">
                            ${performance.toFixed(0)}%
                        </div>
                    </div>
                </td>
                <td>${getKPIStatusBadge(kpi.status)}</td>
                <td>${getFrequencyLabel(kpi.frequency)}</td>
                <td>${getVerificationBadge(kpi.verified)}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" onclick="viewKPIDetails('${kpi.id}')">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-outline-warning" onclick="editKPI('${kpi.id}')">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-outline-danger" onclick="deleteKPI('${kpi.id}')">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

/**
 * Load drivers and obstacles
 */
function loadDriversAndObstacles() {
    const driversContainer = document.getElementById('performance-drivers');
    const obstaclesContainer = document.getElementById('performance-obstacles');
    
    if (driversContainer && kpiData[0]?.contributingFactors?.drivers) {
        driversContainer.innerHTML = kpiData[0].contributingFactors.drivers.map((driver, index) => `
            <div class="d-flex align-items-center mb-2">
                <span class="badge bg-success me-2">${index + 1}</span>
                <span class="flex-grow-1">${driver}</span>
                <button class="btn btn-sm btn-outline-danger" onclick="removeDriver(${index})">
                    <i class="bi bi-trash"></i>
                </button>
            </div>
        `).join('');
    }
    
    if (obstaclesContainer && kpiData[0]?.contributingFactors?.obstacles) {
        obstaclesContainer.innerHTML = kpiData[0].contributingFactors.obstacles.map((obstacle, index) => `
            <div class="d-flex align-items-center mb-2">
                <span class="badge bg-danger me-2">${index + 1}</span>
                <span class="flex-grow-1">${obstacle}</span>
                <button class="btn btn-sm btn-outline-danger" onclick="removeObstacle(${index})">
                    <i class="bi bi-trash"></i>
                </button>
            </div>
        `).join('');
    }
}

// ============================================
// Achievements Management (UC-10) 
// ============================================

/**
 * Initialize achievements data with BRD-compliant fields
 */
function initializeAchievementsData() {
    achievementsData = [
        {
            id: 'ACH-001',
            title: 'إطلاق منصة المدفوعات الرقمية الموحدة',
            description: 'تم إطلاق المنصة الوطنية للمدفوعات الرقمية بنجاح مع تحقيق أكثر من 500,000 معاملة في الأسبوع الأول من الإطلاق، مما يعكس الإقبال الكبير من المواطنين والمقيمين على استخدام الخدمات الرقمية',
            category: 'kpi', // kpi | initiative | impact (BR-84, BR-85)
            impact: 'high', // high | medium | low
            year: 2025,
            quarter: 'Q1',
            isTop5: true, // BR-81: Max 5 can be marked as top
            // Strategic objective - 3 levels (BR-83)
            strategicObjectiveL1: 'تحسين جودة الحياة',
            strategicObjectiveL2: 'تطوير الخدمات الحكومية',
            strategicObjectiveL3: 'رقمنة الخدمات الحكومية بنسبة 100%',
            // Related entities
            relatedKPI: 'KPI-001', // BR-84: Link to specific KPI for category 'kpi'
            relatedInitiative: '1-18-139-1377', // BR-85: Link to initiative
            // Contributing factors
            contributingActivities: [
                'تطوير البنية التحتية التقنية',
                'التنسيق مع البنوك والمؤسسات المالية',
                'حملة تسويقية شاملة للتوعية'
            ],
            governmentEntities: [
                'وزارة المالية',
                'البنك المركزي السعودي',
                'هيئة الحكومة الرقمية'
            ],
            // Metadata
            createdBy: 'محمد أحمد',
            createdDate: '2025-01-05',
            lastModified: '2025-01-10',
            status: 'active', // active | archived | deleted (BR-86)
            verificationStatus: 'verified',
            targetAudience: 'جميع المواطنين والمقيمين',
            beneficiariesCount: '2.5 مليون مستفيد',
            completionPercentage: 100
        },
        {
            id: 'ACH-002',
            title: 'تحقيق 85% من مستهدف التوظيف في القطاع الخاص',
            description: 'تم تحقيق نسبة 85% من المستهدف السنوي لتوظيف السعوديين في القطاع الخاص خلال الربع الأول',
            category: 'initiative',
            impact: 'high',
            year: 2025,
            quarter: 'Q1',
            isTop5: true,
            strategicObjectiveL1: 'اقتصاد مزدهر',
            strategicObjectiveL2: 'فرص العمل للجميع',
            strategicObjectiveL3: 'خفض معدل البطالة إلى 7%',
            relatedInitiative: '1-18-140-1378',
            contributingActivities: [
                'برامج التدريب المهني',
                'حوافز للقطاع الخاص',
                'معارض التوظيف'
            ],
            governmentEntities: [
                'وزارة الموارد البشرية',
                'صندوق تنمية الموارد البشرية'
            ],
            createdBy: 'سارة محمد',
            createdDate: '2025-01-08',
            status: 'active',
            verificationStatus: 'pending',
            beneficiariesCount: '125,000 موظف'
        }
    ];
}

/**
 * Load and display achievements
 */
function loadAchievements() {
    updateAchievementStatistics();
    displayAchievementsList();
    updateTop5Count();
}

/**
 * Update achievement statistics
 */
function updateAchievementStatistics() {
    const activeAchievements = achievementsData.filter(a => a.status === 'active');
    
    const highImpact = activeAchievements.filter(a => a.impact === 'high').length;
    const mediumImpact = activeAchievements.filter(a => a.impact === 'medium').length;
    const lowImpact = activeAchievements.filter(a => a.impact === 'low').length;
    const total = activeAchievements.length;
    
    updateElement('high-impact-count', highImpact);
    updateElement('medium-impact-count', mediumImpact);
    updateElement('low-impact-count', lowImpact);
    updateElement('total-achievements', total);
    
    // Update top 5 count
    const top5Count = activeAchievements.filter(a => a.isTop5).length;
    updateElement('top5-count', `${top5Count}/5`);
}

/**
 * Display achievements list with all BRD fields
 */
function displayAchievementsList() {
    const container = document.getElementById('achievements-list');
    if (!container) return;
    
    const filteredAchievements = applyAchievementFilters();
    const top5Count = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
    
    container.innerHTML = filteredAchievements.map((achievement, index) => `
        <div class="card mb-3 border-${getImpactColor(achievement.impact)} ${achievement.isTop5 ? 'achievement-top5' : ''}" 
             id="achievement-${achievement.id}">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-8">
                        <div class="d-flex align-items-start mb-2">
                            <div class="flex-grow-1">
                                <h5 class="card-title mb-1">
                                    ${achievement.isTop5 ? `
                                        <span class="badge bg-warning text-dark me-2">
                                            <i class="bi bi-star-fill"></i> TOP ${achievementsData.filter(a => a.isTop5 && a.status === 'active').indexOf(achievement) + 1}
                                        </span>
                                    ` : ''}
                                    ${achievement.title}
                                </h5>
                                <div class="mb-2">
                                    ${getCategoryBadge(achievement.category)}
                                    ${getImpactBadge(achievement.impact)}
                                    ${getVerificationStatusBadge(achievement.verificationStatus)}
                                </div>
                            </div>
                            <div class="form-check form-switch ms-3">
                                <input class="form-check-input top5-toggle" type="checkbox" 
                                       id="top5-toggle-${achievement.id}" 
                                       ${achievement.isTop5 ? 'checked' : ''} 
                                       ${!achievement.isTop5 && top5Count >= 5 ? 'disabled' : ''}
                                       onchange="toggleTop5FromList('${achievement.id}', this)">
                                <label class="form-check-label" for="top5-toggle-${achievement.id}">
                                    <small class="text-muted">أفضل 5</small>
                                </label>
                            </div>
                        </div>
                        
                        <p class="card-text">${achievement.description}</p>
                        
                        <div class="row g-2 mb-3">
                            <div class="col-md-6">
                                <small class="text-muted">الهدف الاستراتيجي:</small>
                                <div class="ms-2">
                                    <div>${achievement.strategicObjectiveL1}</div>
                                    <div class="ms-2">← ${achievement.strategicObjectiveL2}</div>
                                    <div class="ms-3">→ ${achievement.strategicObjectiveL3}</div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <small class="text-muted">الجهات المشاركة:</small>
                                <div class="ms-2">
                                    ${achievement.governmentEntities.map(e => 
                                        `<span class="badge bg-light text-dark me-1">${e}</span>`
                                    ).join('')}
                                </div>
                            </div>
                        </div>
                        
                        <div class="row g-2">
                            <div class="col-md-6">
                                <small class="text-muted">الأنشطة المساهمة:</small>
                                <ul class="mb-0 small">
                                    ${achievement.contributingActivities.map(a => `<li>${a}</li>`).join('')}
                                </ul>
                            </div>
                            <div class="col-md-6">
                                ${achievement.beneficiariesCount ? `
                                    <small class="text-muted d-block">عدد المستفيدين:</small>
                                    <strong>${achievement.beneficiariesCount}</strong>
                                ` : ''}
                                ${achievement.relatedKPI ? `
                                    <small class="text-muted d-block mt-2">المؤشر المرتبط:</small>
                                    <a href="#" onclick="viewKPIDetails('${achievement.relatedKPI}')">${achievement.relatedKPI}</a>
                                ` : ''}
                                ${achievement.relatedInitiative ? `
                                    <small class="text-muted d-block mt-2">المبادرة المرتبطة:</small>
                                    <a href="#" onclick="viewInitiative('${achievement.relatedInitiative}')">${achievement.relatedInitiative}</a>
                                ` : ''}
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-4 text-end">
                        <div class="mb-3">
                            <small class="text-muted d-block">الفترة:</small>
                            <strong>${achievement.year} - ${achievement.quarter}</strong>
                        </div>
                        
                        <div class="btn-group" role="group">
                            <button class="btn btn-sm btn-outline-primary" onclick="editAchievement('${achievement.id}')">
                                <i class="bi bi-pencil"></i> تعديل
                            </button>
                            <button class="btn btn-sm btn-outline-danger" onclick="archiveAchievement('${achievement.id}')">
                                <i class="bi bi-archive"></i> أرشفة
                            </button>
                        </div>
                        
                        <div class="mt-3 small text-muted">
                            <div>أنشأ بواسطة: ${achievement.createdBy}</div>
                            <div>التاريخ: ${achievement.createdDate}</div>
                            ${achievement.lastModified ? `<div>آخر تعديل: ${achievement.lastModified}</div>` : ''}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
    
    // Update toggles state
    updateTop5ToggleStates();
}

/**
 * Add new achievement with full BRD compliance
 */
function addNewAchievement() {
    const form = document.getElementById('achievement-form');
    if (!form) return;
    
    const formData = new FormData(form);
    
    // Validate top 5 limit (BR-81)
    const currentTop5 = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
    const isTop5 = formData.get('isTop5') === 'true';
    
    if (isTop5 && currentTop5 >= 5) {
        showNotification('لا يمكن تحديد أكثر من 5 إنجازات كأفضل خمسة', 'warning');
        return;
    }
    
    const newAchievement = {
        id: 'ACH-' + Date.now(),
        title: formData.get('title'),
        description: formData.get('description'),
        category: formData.get('category'),
        impact: formData.get('impact'),
        year: parseInt(formData.get('year')),
        quarter: formData.get('quarter'),
        isTop5: isTop5,
        strategicObjectiveL1: formData.get('objectiveL1'),
        strategicObjectiveL2: formData.get('objectiveL2'),
        strategicObjectiveL3: formData.get('objectiveL3'),
        relatedKPI: formData.get('relatedKPI'),
        relatedInitiative: formData.get('relatedInitiative'),
        contributingActivities: formData.get('activities').split('،').map(a => a.trim()).filter(a => a),
        governmentEntities: formData.get('entities').split('،').map(e => e.trim()).filter(e => e),
        targetAudience: formData.get('targetAudience'),
        beneficiariesCount: formData.get('beneficiariesCount'),
        createdBy: 'المستخدم الحالي',
        createdDate: new Date().toISOString().split('T')[0],
        status: 'active',
        verificationStatus: 'pending'
    };
    
    achievementsData.push(newAchievement);
    loadAchievements();
    form.reset();
    showNotification('تم إضافة الإنجاز بنجاح', 'success');
}

/**
 * Archive achievement (BR-86: soft delete with archived flag)
 */
function archiveAchievement(achievementId) {
    if (!confirm('هل تريد أرشفة هذا الإنجاز؟')) return;
    
    const achievement = achievementsData.find(a => a.id === achievementId);
    if (achievement) {
        achievement.status = 'archived';
        achievement.archiveDate = new Date().toISOString();
        achievement.archivedBy = 'المستخدم الحالي';
        loadAchievements();
        showNotification('تم أرشفة الإنجاز', 'info');
    }
}

/**
 * Toggle top 5 status (BR-81)
 */
function toggleTop5(achievementId) {
    const achievement = achievementsData.find(a => a.id === achievementId);
    if (!achievement) return;
    
    if (!achievement.isTop5) {
        const currentTop5 = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
        if (currentTop5 >= 5) {
            showNotification('لا يمكن تحديد أكثر من 5 إنجازات كأفضل خمسة', 'warning');
            return;
        }
    }
    
    achievement.isTop5 = !achievement.isTop5;
    loadAchievements();
    showNotification(achievement.isTop5 ? 'تم إضافة الإنجاز لأفضل خمسة' : 'تم إزالة الإنجاز من أفضل خمسة', 'success');
}

/**
 * Toggle top 5 from list view
 */
function toggleTop5FromList(achievementId, checkbox) {
    const achievement = achievementsData.find(a => a.id === achievementId);
    if (!achievement) return;
    
    const wantToSetTop5 = checkbox.checked;
    
    if (wantToSetTop5) {
        const currentTop5 = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
        if (currentTop5 >= 5) {
            checkbox.checked = false;
            showNotification('لا يمكن تحديد أكثر من 5 إنجازات كأفضل خمسة', 'warning');
            return;
        }
    }
    
    achievement.isTop5 = wantToSetTop5;
    loadAchievements();
    showNotification(wantToSetTop5 ? 'تم إضافة الإنجاز لأفضل خمسة' : 'تم إزالة الإنجاز من أفضل خمسة', 'success');
}

/**
 * Update top 5 toggle states
 */
function updateTop5ToggleStates() {
    const currentTop5 = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
    const isAtLimit = currentTop5 >= 5;
    
    // Update all toggle switches
    document.querySelectorAll('.top5-toggle').forEach(toggle => {
        const achievementId = toggle.id.replace('top5-toggle-', '');
        const achievement = achievementsData.find(a => a.id === achievementId);
        
        if (achievement && !achievement.isTop5 && isAtLimit) {
            toggle.disabled = true;
            toggle.title = 'الحد الأقصى 5 إنجازات في أفضل خمسة';
        } else {
            toggle.disabled = false;
            toggle.title = '';
        }
    });
}

/**
 * Edit achievement
 */
function editAchievement(achievementId) {
    const achievement = achievementsData.find(a => a.id === achievementId);
    if (!achievement) return;
    
    // Populate form with achievement data
    const form = document.getElementById('achievement-form');
    if (form) {
        form.elements['title'].value = achievement.title;
        form.elements['description'].value = achievement.description;
        form.elements['category'].value = achievement.category;
        form.elements['impact'].value = achievement.impact;
        form.elements['year'].value = achievement.year;
        form.elements['quarter'].value = achievement.quarter;
        form.elements['isTop5'].checked = achievement.isTop5;
        form.elements['objectiveL1'].value = achievement.strategicObjectiveL1;
        form.elements['objectiveL2'].value = achievement.strategicObjectiveL2;
        form.elements['objectiveL3'].value = achievement.strategicObjectiveL3;
        form.elements['relatedKPI'].value = achievement.relatedKPI || '';
        form.elements['relatedInitiative'].value = achievement.relatedInitiative || '';
        form.elements['activities'].value = achievement.contributingActivities.join('، ');
        form.elements['entities'].value = achievement.governmentEntities.join('، ');
        form.elements['targetAudience'].value = achievement.targetAudience || '';
        form.elements['beneficiariesCount'].value = achievement.beneficiariesCount || '';
        
        // Scroll to form
        form.scrollIntoView({ behavior: 'smooth' });
    }
}

// ============================================
// Risk Management (UC-11, UC-12)
// ============================================

/**
 * Initialize risks data with BRD-compliant structure from governor
 */
function initializeRisksData() {
    risksData = [
        {
            id: 'RISK-001',
            code: 'VRP_1_SEC_001',
            title: 'خطر أمني على منصة المدفوعات الرقمية',
            titleEn: 'Security Risk on Digital Payments Platform',
            description: 'احتمالية تعرض منصة المدفوعات الرقمية لاختراق أمني قد يؤدي إلى تسريب بيانات المستخدمين المالية',
            category: 'security', // technical, financial, operational, strategic, compliance
            probability: 'medium', // verylow, low, medium, high, veryhigh
            probabilityScore: 2,
            impact: 'high', // verylow, low, medium, high, veryhigh
            impactScore: 3,
            riskScore: 6, // Probability × Impact
            isTop5: true, // BR-equivalent: Max 5 critical risks
            status: 'open', // open, mitigating, closed, escalated
            
            // BRD Fields
            riskLevelType: 'برنامج', // رؤية/برنامج/مبادرة
            submittingEntity: 'هيئة الحكومة الرقمية',
            responsibleEntity: 'البنك المركزي السعودي',
            
            // Mitigation
            mitigationStatus: 'جاري التنفيذ',
            mitigationActionsCount: 4,
            mitigationCompletionPercentage: 75,
            mitigationPlan: 'تطبيق بروتوكولات أمنية متقدمة وإجراء اختبارات اختراق دورية',
            mitigationActions: [
                'تطبيق التشفير من طرف إلى طرف',
                'إضافة المصادقة الثنائية',
                'مراجعة أمنية شهرية',
                'تدريب الموظفين على الأمن السيبراني'
            ],
            contingencyPlan: 'تفعيل خطة الطوارئ وإيقاف المنصة مؤقتاً في حالة الاختراق',
            
            // Time tracking
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-05',
            targetResolutionDate: '2025-03-31',
            
            // Ownership
            owner: 'أحمد محمد - مدير الأمن السيبراني',
            escalatedTo: null,
            
            // Related entities
            affectedComponents: ['منصة المدفوعات', 'بوابة المستخدمين', 'قاعدة البيانات'],
            relatedInitiatives: ['1-18-139-1377'],
            
            createdBy: 'محمد الأحمد',
            createdDate: '2025-01-05'
        },
        {
            id: 'RISK-002',
            code: 'VRP_1_OPS_002',
            title: 'تأخر في تنفيذ المبادرات الرئيسية',
            titleEn: 'Delay in Key Initiatives Implementation',
            description: 'خطر تأخر تنفيذ المبادرات الرئيسية بسبب نقص الموارد البشرية المؤهلة',
            category: 'operational',
            probability: 'high',
            probabilityScore: 3,
            impact: 'medium',
            impactScore: 2,
            riskScore: 6,
            isTop5: true,
            status: 'mitigating',
            
            riskLevelType: 'برنامج',
            submittingEntity: 'مكتب تحقيق الرؤية',
            responsibleEntity: 'وزارة الموارد البشرية',
            
            mitigationStatus: 'جاري التنفيذ',
            mitigationActionsCount: 3,
            mitigationCompletionPercentage: 50,
            mitigationPlan: 'استقطاب الكفاءات وبرامج التدريب المكثفة',
            mitigationActions: [
                'حملة توظيف للمتخصصين',
                'برامج تدريب مكثفة',
                'الاستعانة بخبراء استشاريين'
            ],
            contingencyPlan: 'إعادة جدولة المبادرات حسب الأولوية',
            
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-08',
            targetResolutionDate: '2025-06-30',
            
            owner: 'سارة أحمد - مدير الموارد البشرية',
            escalatedTo: 'لجنة البرنامج',
            
            affectedComponents: ['جميع المبادرات'],
            relatedInitiatives: ['1-18-140-1378', '1-18-141-1379'],
            
            createdBy: 'فاطمة العلي',
            createdDate: '2025-01-08'
        },
        {
            id: 'RISK-003',
            code: 'VRP_1_FIN_003',
            title: 'تجاوز الميزانية المخصصة',
            titleEn: 'Budget Overrun Risk',
            description: 'احتمالية تجاوز التكاليف الفعلية للميزانية المعتمدة بسبب التضخم وارتفاع الأسعار',
            category: 'financial',
            probability: 'medium',
            probabilityScore: 2,
            impact: 'high',
            impactScore: 3,
            riskScore: 6,
            isTop5: true,
            status: 'open',
            
            riskLevelType: 'برنامج',
            submittingEntity: 'الإدارة المالية',
            responsibleEntity: 'وزارة المالية',
            
            mitigationStatus: 'قيد الدراسة',
            mitigationActionsCount: 5,
            mitigationCompletionPercentage: 20,
            mitigationPlan: 'مراجعة الميزانيات وإعادة تخصيص الموارد',
            mitigationActions: [
                'مراجعة ربع سنوية للميزانيات',
                'التفاوض مع الموردين',
                'تحسين كفاءة الإنفاق',
                'البحث عن مصادر تمويل بديلة',
                'تأجيل المشاريع غير الحرجة'
            ],
            contingencyPlan: 'طلب ميزانية إضافية أو إعادة ترتيب الأولويات',
            
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-10',
            targetResolutionDate: '2025-04-30',
            
            owner: 'خالد السالم - المدير المالي',
            escalatedTo: null,
            
            affectedComponents: ['جميع المبادرات'],
            
            createdBy: 'عبدالله الراشد',
            createdDate: '2025-01-10'
        },
        {
            id: 'RISK-004',
            code: 'VRP_1_TECH_004',
            title: 'عدم توافق الأنظمة التقنية',
            titleEn: 'Technical Systems Incompatibility',
            description: 'مخاطر عدم التوافق بين الأنظمة الجديدة والبنية التحتية الحالية',
            category: 'technical',
            probability: 'low',
            probabilityScore: 1,
            impact: 'medium',
            impactScore: 2,
            riskScore: 2,
            isTop5: false,
            status: 'mitigating',
            
            riskLevelType: 'مبادرة',
            submittingEntity: 'إدارة تقنية المعلومات',
            responsibleEntity: 'هيئة الحكومة الرقمية',
            
            mitigationStatus: 'جاري التنفيذ',
            mitigationActionsCount: 2,
            mitigationCompletionPercentage: 80,
            mitigationPlan: 'تطوير طبقة تكامل موحدة',
            mitigationActions: [
                'بناء APIs موحدة',
                'اختبار التوافق المستمر'
            ],
            
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-12',
            targetResolutionDate: '2025-02-28',
            
            owner: 'يوسف العتيبي - مدير التقنية',
            
            createdBy: 'نورة الشمري',
            createdDate: '2025-01-12'
        },
        {
            id: 'RISK-005',
            code: 'VRP_1_COMP_005',
            title: 'عدم الامتثال للوائح التنظيمية',
            titleEn: 'Regulatory Compliance Risk',
            description: 'مخاطر عدم الامتثال للوائح والأنظمة الجديدة للبيانات والخصوصية',
            category: 'compliance',
            probability: 'medium',
            probabilityScore: 2,
            impact: 'veryhigh',
            impactScore: 4,
            riskScore: 8,
            isTop5: true,
            status: 'open',
            
            riskLevelType: 'برنامج',
            submittingEntity: 'إدارة الامتثال',
            responsibleEntity: 'الإدارة القانونية',
            
            mitigationStatus: 'قيد البدء',
            mitigationActionsCount: 6,
            mitigationCompletionPercentage: 15,
            mitigationPlan: 'مراجعة شاملة للوائح وتطبيق إطار امتثال متكامل',
            mitigationActions: [
                'مراجعة قانونية شاملة',
                'تدريب الموظفين على اللوائح',
                'تطوير سياسات الخصوصية',
                'تنفيذ ضوابط الامتثال',
                'إجراء تدقيق دوري',
                'توثيق جميع العمليات'
            ],
            contingencyPlan: 'التعاقد مع مستشار قانوني متخصص',
            
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-15',
            targetResolutionDate: '2025-05-31',
            
            owner: 'منال القحطاني - مدير الامتثال',
            escalatedTo: 'المدير التنفيذي',
            
            affectedComponents: ['جميع الأنظمة التي تعالج البيانات الشخصية'],
            
            createdBy: 'عمر الغامدي',
            createdDate: '2025-01-15'
        },
        {
            id: 'RISK-006',
            code: 'VRP_1_STRAT_006',
            title: 'تغير الأولويات الاستراتيجية',
            titleEn: 'Strategic Priorities Shift',
            description: 'احتمالية تغير الأولويات الوطنية مما قد يؤثر على دعم البرنامج',
            category: 'strategic',
            probability: 'low',
            probabilityScore: 1,
            impact: 'high',
            impactScore: 3,
            riskScore: 3,
            isTop5: true,
            status: 'open',
            
            riskLevelType: 'رؤية',
            submittingEntity: 'مكتب الإدارة الاستراتيجية',
            responsibleEntity: 'مجلس الشؤون الاقتصادية والتنمية',
            
            mitigationStatus: 'مراقبة مستمرة',
            mitigationActionsCount: 2,
            mitigationCompletionPercentage: 100,
            mitigationPlan: 'المواءمة المستمرة مع الأولويات الوطنية',
            mitigationActions: [
                'مراجعة دورية للمواءمة الاستراتيجية',
                'تواصل مستمر مع أصحاب المصلحة'
            ],
            
            year: 2025,
            quarter: 'Q1',
            identifiedDate: '2025-01-01',
            targetResolutionDate: null,
            
            owner: 'ريم الفيصل - مدير الاستراتيجية',
            
            createdBy: 'سلطان الدوسري',
            createdDate: '2025-01-01'
        }
    ];
}

/**
 * Load and display risks
 */
function loadRisks() {
    console.log('=== loadRisks called ===');
    console.log('Risks data count:', risksData.length);
    console.log('First risk:', risksData[0] ? risksData[0].title : 'No risks');
    
    updateRiskStatistics();
    displayRisksList();
    updateTop5RisksCount();
    
    console.log('=== loadRisks completed ===');
}

/**
 * Update risk statistics
 */
function updateRiskStatistics() {
    const activeRisks = risksData.filter(r => r.status !== 'closed');
    
    const critical = activeRisks.filter(r => r.riskScore >= 8).length;
    const high = activeRisks.filter(r => r.riskScore >= 6 && r.riskScore < 8).length;
    const medium = activeRisks.filter(r => r.riskScore >= 4 && r.riskScore < 6).length;
    const low = activeRisks.filter(r => r.riskScore < 4).length;
    
    updateElement('critical-risks-count', critical);
    updateElement('high-risks-count', high);
    updateElement('medium-risks-count', medium);
    updateElement('low-risks-count', low);
    
    // Update overall risk score
    const overallScore = calculateOverallRiskScore();
    updateElement('overall-risk-score', overallScore + '/100');
}

/**
 * Calculate overall risk score
 */
function calculateOverallRiskScore() {
    let score = 100;
    risksData.forEach(risk => {
        if (risk.status !== 'closed') {
            if (risk.riskScore >= 9) score -= 20;
            else if (risk.riskScore >= 6) score -= 10;
            else if (risk.riskScore >= 4) score -= 5;
            else score -= 2;
        }
    });
    return Math.max(0, score);
}

/**
 * Display risks list
 */
function displayRisksList() {
    const container = document.getElementById('risks-list');
    if (!container) {
        console.error('Risks list container not found!');
        return;
    }
    
    const filteredRisks = applyRiskFilters();
    console.log('Displaying', filteredRisks.length, 'risks');
    
    if (filteredRisks.length === 0) {
        container.innerHTML = `
            <div class="alert alert-info">
                <i class="bi bi-info-circle me-2"></i>
                لا توجد مخاطر تطابق معايير البحث أو الفلتر الحالية.
            </div>
        `;
        return;
    }
    
    container.innerHTML = filteredRisks.map(risk => {
        const riskColor = getRiskColor(risk.riskScore);
        return `
            <div class="card mb-3 border-${riskColor} ${risk.isTop5 ? 'risk-top5' : ''}">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-8">
                            <div class="d-flex align-items-start mb-2">
                                <div class="flex-grow-1">
                                    <h5 class="card-title mb-1">
                                        ${risk.isTop5 ? '<span class="badge bg-danger me-2"><i class="bi bi-exclamation-triangle"></i> TOP 5</span>' : ''}
                                        ${risk.title}
                                    </h5>
                                    <div class="mb-2">
                                        <span class="badge bg-secondary">${risk.code}</span>
                                        ${getRiskCategoryBadge(risk.category)}
                                        ${getRiskScoreBadge(risk.riskScore)}
                                        ${getStatusBadge(risk.status)}
                                    </div>
                                </div>
                                <div class="form-check form-switch ms-3">
                                    <input class="form-check-input risk-top5-toggle" type="checkbox"
                                           id="risk-top5-${risk.id}"
                                           ${risk.isTop5 ? 'checked' : ''}
                                           onchange="toggleRiskTop5('${risk.id}', this)">
                                    <label class="form-check-label" for="risk-top5-${risk.id}">
                                        <small class="text-muted">أفضل 5</small>
                                    </label>
                                </div>
                            </div>
                            
                            <p class="card-text">${risk.description}</p>
                            
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <div class="risk-matrix-mini p-2 bg-light rounded">
                                        <small class="text-muted d-block mb-1">تقييم المخاطر:</small>
                                        <div class="d-flex justify-content-between">
                                            <span>الاحتمالية: <strong>${getProbabilityLabel(risk.probability)}</strong></span>
                                            <span>التأثير: <strong>${getImpactLabel(risk.impact)}</strong></span>
                                            <span>النتيجة: <strong class="text-${riskColor}">${risk.riskScore}</strong></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <small class="text-muted d-block">خطة التخفيف:</small>
                                    <div class="progress mb-1" style="height: 20px;">
                                        <div class="progress-bar bg-success" style="width: ${risk.mitigationCompletionPercentage}%">
                                            ${risk.mitigationCompletionPercentage}%
                                        </div>
                                    </div>
                                    <small>${risk.mitigationActionsCount} إجراءات - ${risk.mitigationStatus}</small>
                                </div>
                            </div>
                            
                            ${risk.mitigationActions && risk.mitigationActions.length > 0 ? `
                                <div class="mt-3">
                                    <small class="text-muted">إجراءات التخفيف:</small>
                                    <ul class="small mb-0">
                                        ${risk.mitigationActions.map(action => `<li>${action}</li>`).join('')}
                                    </ul>
                                </div>
                            ` : ''}
                        </div>
                        
                        <div class="col-md-4">
                            <div class="text-end">
                                <small class="text-muted d-block">المسؤول:</small>
                                <strong>${risk.owner}</strong>
                                
                                ${risk.escalatedTo ? `
                                    <small class="text-danger d-block mt-2">
                                        <i class="bi bi-arrow-up-circle"></i> تصعيد إلى: ${risk.escalatedTo}
                                    </small>
                                ` : ''}
                                
                                <div class="mt-3">
                                    <small class="text-muted d-block">التواريخ:</small>
                                    <small>تاريخ التحديد: ${risk.identifiedDate}</small><br>
                                    ${risk.targetResolutionDate ? `<small>الموعد المستهدف: ${risk.targetResolutionDate}</small>` : ''}
                                </div>
                                
                                <div class="btn-group mt-3" role="group">
                                    <button class="btn btn-sm btn-outline-primary" onclick="viewRiskDetails('${risk.id}')">
                                        <i class="bi bi-eye"></i>
                                    </button>
                                    <button class="btn btn-sm btn-outline-warning" onclick="editRisk('${risk.id}')">
                                        <i class="bi bi-pencil"></i>
                                    </button>
                                    <button class="btn btn-sm btn-outline-success" onclick="updateMitigation('${risk.id}')">
                                        <i class="bi bi-shield-check"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }).join('');
}

/**
 * Get risk color based on score
 */
function getRiskColor(score) {
    if (score >= 9) return 'danger';
    if (score >= 6) return 'warning';
    if (score >= 4) return 'info';
    return 'success';
}

/**
 * Get risk score badge
 */
function getRiskScoreBadge(score) {
    const color = getRiskColor(score);
    let label = 'منخفض';
    if (score >= 9) label = 'حرج';
    else if (score >= 6) label = 'عالي';
    else if (score >= 4) label = 'متوسط';
    
    return `<span class="badge bg-${color}">${label} (${score})</span>`;
}

/**
 * Get probability label
 */
function getProbabilityLabel(probability) {
    const labels = {
        'verylow': 'منخفض جداً',
        'low': 'منخفض',
        'medium': 'متوسط',
        'high': 'عالي',
        'veryhigh': 'عالي جداً'
    };
    return labels[probability] || probability;
}

/**
 * Get impact label
 */
function getImpactLabel(impact) {
    const labels = {
        'verylow': 'منخفض جداً',
        'low': 'منخفض',
        'medium': 'متوسط',
        'high': 'عالي',
        'veryhigh': 'عالي جداً'
    };
    return labels[impact] || impact;
}

/**
 * Get status badge
 */
function getStatusBadge(status) {
    const badges = {
        'open': '<span class="badge bg-danger">مفتوح</span>',
        'mitigating': '<span class="badge bg-warning">جاري التخفيف</span>',
        'closed': '<span class="badge bg-success">مغلق</span>',
        'escalated': '<span class="badge bg-danger">تم التصعيد</span>'
    };
    return badges[status] || '';
}

/**
 * Apply risk filters
 */
function applyRiskFilters() {
    let filtered = [...risksData];
    
    // Check if top 5 only filter is active
    const showTop5Only = document.getElementById('top5RisksOnly')?.checked;
    if (showTop5Only) {
        filtered = filtered.filter(r => r.isTop5);
    }
    
    // Category filter
    const categoryFilter = document.getElementById('risk-category-filter')?.value;
    if (categoryFilter) {
        filtered = filtered.filter(r => r.category === categoryFilter);
    }
    
    // Status filter
    const statusFilter = document.getElementById('risk-status-filter')?.value;
    if (statusFilter) {
        filtered = filtered.filter(r => r.status === statusFilter);
    }
    
    // Search filter
    const searchTerm = document.getElementById('risk-search')?.value?.toLowerCase();
    if (searchTerm) {
        filtered = filtered.filter(r => 
            r.title.toLowerCase().includes(searchTerm) ||
            r.titleEn?.toLowerCase().includes(searchTerm) ||
            r.code.toLowerCase().includes(searchTerm) ||
            r.description.toLowerCase().includes(searchTerm)
        );
    }
    
    // Sort by risk score (highest first)
    filtered.sort((a, b) => {
        if (a.isTop5 && !b.isTop5) return -1;
        if (!a.isTop5 && b.isTop5) return 1;
        return b.riskScore - a.riskScore;
    });
    
    return filtered;
}

/**
 * Update top 5 risks count
 */
function updateTop5RisksCount() {
    const top5Count = risksData.filter(r => r.isTop5 && r.status !== 'closed').length;
    const badge = document.getElementById('top5-risks-badge');
    if (badge) {
        badge.textContent = `${top5Count}/5`;
        badge.className = `badge ${top5Count >= 5 ? 'bg-danger' : 'bg-warning'} ms-2`;
    }
}

/**
 * Toggle risk top 5 status
 */
function toggleRiskTop5(riskId, checkbox) {
    const risk = risksData.find(r => r.id === riskId);
    if (!risk) return;
    
    const wantToSetTop5 = checkbox.checked;
    
    if (wantToSetTop5) {
        const currentTop5 = risksData.filter(r => r.isTop5 && r.status !== 'closed').length;
        if (currentTop5 >= 5) {
            checkbox.checked = false;
            showNotification('لا يمكن تحديد أكثر من 5 مخاطر كأعلى خمسة', 'warning');
            return;
        }
    }
    
    risk.isTop5 = wantToSetTop5;
    loadRisks();
    showNotification(wantToSetTop5 ? 'تم إضافة المخاطرة لأعلى خمسة' : 'تم إزالة المخاطرة من أعلى خمسة', 'success');
}

// Risk action stubs
function viewRiskDetails(riskId) {
    console.log('Viewing risk:', riskId);
}

function editRisk(riskId) {
    console.log('Editing risk:', riskId);
}

function updateMitigation(riskId) {
    console.log('Updating mitigation for risk:', riskId);
}

// ============================================
// Utility Functions
// ============================================

function updateElement(id, value) {
    const element = document.getElementById(id);
    if (element) element.textContent = value;
}

function getProgressBarClass(performance) {
    if (performance >= 90) return 'bg-success';
    if (performance >= 70) return 'bg-warning';
    return 'bg-danger';
}

function getKPIStatusBadge(status) {
    const badges = {
        'achieved': '<span class="badge bg-success">محقق</span>',
        'near': '<span class="badge bg-warning">قريب</span>',
        'behind': '<span class="badge bg-danger">متأخر</span>'
    };
    return badges[status] || '';
}

function getVerificationBadge(verified) {
    return verified ? 
        '<span class="badge bg-success"><i class="bi bi-check-circle"></i> تحقق</span>' :
        '<span class="badge bg-secondary"><i class="bi bi-clock"></i> قيد المراجعة</span>';
}

function getFrequencyLabel(frequency) {
    const labels = {
        'annual': 'سنوي',
        'quarterly': 'ربع سنوي',
        'monthly': 'شهري'
    };
    return labels[frequency] || frequency;
}

function getImpactColor(impact) {
    const colors = {
        'high': 'success',
        'medium': 'warning',
        'low': 'info'
    };
    return colors[impact] || 'secondary';
}

function getImpactBadge(impact) {
    const badges = {
        'high': '<span class="badge bg-success">أثر عالي</span>',
        'medium': '<span class="badge bg-warning">أثر متوسط</span>',
        'low': '<span class="badge bg-info">أثر منخفض</span>'
    };
    return badges[impact] || '';
}

function getCategoryBadge(category) {
    const badges = {
        'kpi': '<span class="badge bg-primary">مؤشر أداء</span>',
        'initiative': '<span class="badge bg-info">مبادرة</span>',
        'impact': '<span class="badge bg-success">أثر إيجابي</span>'
    };
    return badges[category] || '';
}

function getRiskCategoryBadge(category) {
    const badges = {
        'security': '<span class="badge bg-danger">أمنية</span>',
        'technical': '<span class="badge bg-info">تقنية</span>',
        'financial': '<span class="badge bg-success">مالية</span>',
        'operational': '<span class="badge bg-warning text-dark">تشغيلية</span>',
        'strategic': '<span class="badge bg-primary">استراتيجية</span>',
        'compliance': '<span class="badge bg-secondary">امتثال</span>'
    };
    return badges[category] || '';
}

function getVerificationStatusBadge(status) {
    const badges = {
        'verified': '<span class="badge bg-success"><i class="bi bi-patch-check"></i> موثق</span>',
        'pending': '<span class="badge bg-warning"><i class="bi bi-hourglass"></i> قيد التوثيق</span>',
        'rejected': '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> مرفوض</span>'
    };
    return badges[status] || '';
}

function showNotification(message, type = 'info') {
    // Bootstrap toast or alert implementation
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 end-0 m-3`;
    alertDiv.style.zIndex = '9999';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
        alertDiv.remove();
    }, 5000);
}

// Filter functions
function applyKPIFilters() {
    let filtered = [...kpiData];
    
    const searchTerm = document.getElementById('kpi-search')?.value?.toLowerCase();
    const statusFilter = document.getElementById('kpi-status-filter')?.value;
    const frequencyFilter = document.getElementById('kpi-frequency-filter')?.value;
    const sourceFilter = document.getElementById('kpi-source-filter')?.value;
    const verifiedFilter = document.getElementById('kpi-verified-filter')?.value;
    const yearFilter = document.getElementById('kpi-year-filter')?.value;
    
    if (searchTerm) {
        filtered = filtered.filter(kpi => 
            kpi.nameAr.toLowerCase().includes(searchTerm) ||
            kpi.nameEn.toLowerCase().includes(searchTerm) ||
            kpi.code.toLowerCase().includes(searchTerm)
        );
    }
    
    if (statusFilter) {
        filtered = filtered.filter(kpi => kpi.status === statusFilter);
    }
    
    if (frequencyFilter) {
        filtered = filtered.filter(kpi => kpi.frequency === frequencyFilter);
    }
    
    if (sourceFilter) {
        filtered = filtered.filter(kpi => kpi.source === sourceFilter);
    }
    
    if (verifiedFilter) {
        filtered = filtered.filter(kpi => kpi.verified === (verifiedFilter === 'true'));
    }
    
    if (yearFilter) {
        filtered = filtered.filter(kpi => kpi.year === parseInt(yearFilter));
    }
    
    return filtered;
}

function applyAchievementFilters() {
    let filtered = achievementsData.filter(a => a.status === 'active');
    
    const showTop5 = document.getElementById('top5Only')?.checked;
    const categoryFilter = document.getElementById('achievement-category-filter')?.value;
    const impactFilter = document.getElementById('achievement-impact-filter')?.value;
    const yearFilter = document.getElementById('achievement-year-filter')?.value;
    const quarterFilter = document.getElementById('achievement-quarter-filter')?.value;
    
    if (showTop5) {
        filtered = filtered.filter(a => a.isTop5);
    }
    
    if (categoryFilter) {
        filtered = filtered.filter(a => a.category === categoryFilter);
    }
    
    if (impactFilter) {
        filtered = filtered.filter(a => a.impact === impactFilter);
    }
    
    if (yearFilter) {
        filtered = filtered.filter(a => a.year === parseInt(yearFilter));
    }
    
    if (quarterFilter) {
        filtered = filtered.filter(a => a.quarter === quarterFilter);
    }
    
    // Sort by impact and top 5 status
    filtered.sort((a, b) => {
        if (a.isTop5 && !b.isTop5) return -1;
        if (!a.isTop5 && b.isTop5) return 1;
        const impactOrder = { high: 3, medium: 2, low: 1 };
        return impactOrder[b.impact] - impactOrder[a.impact];
    });
    
    return filtered;
}

function updateTop5Count() {
    const top5Count = achievementsData.filter(a => a.isTop5 && a.status === 'active').length;
    const badge = document.getElementById('top5-badge');
    if (badge) {
        badge.textContent = `${top5Count}/5`;
        badge.className = `badge ${top5Count >= 5 ? 'bg-danger' : 'bg-warning'} ms-2`;
    }
}

// Achievement form functions
function toggleCategoryFields(category) {
    const kpiField = document.getElementById('kpi-field');
    
    if (kpiField) {
        if (category === 'kpi') {
            kpiField.style.display = 'block';
        } else {
            kpiField.style.display = 'none';
        }
    }
}

function updateLevel2Options(level1) {
    const level2Select = document.querySelector('select[name="objectiveL2"]');
    if (!level2Select) return;
    
    level2Select.innerHTML = '<option value="">اختر...</option>';
    
    const level2Options = {
        'اقتصاد مزدهر': [
            'فرص العمل للجميع',
            'استثمار طويل الأمد',
            'موقع جغرافي مميز'
        ],
        'مجتمع حيوي': [
            'قيمه راسخة',
            'بيئته عامرة',
            'بنيانه متين'
        ],
        'وطن طموح': [
            'حكومته فعالة',
            'مواطنه مسؤول'
        ]
    };
    
    if (level2Options[level1]) {
        level2Options[level1].forEach(option => {
            const opt = document.createElement('option');
            opt.value = option;
            opt.textContent = option;
            level2Select.appendChild(opt);
        });
    }
}

function updateLevel3Options(level2) {
    const level3Select = document.querySelector('select[name="objectiveL3"]');
    if (!level3Select) return;
    
    level3Select.innerHTML = '<option value="">اختر...</option>';
    
    const level3Options = {
        'فرص العمل للجميع': [
            'خفض معدل البطالة إلى 7%',
            'رفع مشاركة المرأة في سوق العمل',
            'تمكين اندماج ذوي الإعاقة في سوق العمل'
        ],
        'استثمار طويل الأمد': [
            'الوصول بالاستثمار الأجنبي المباشر إلى 5.7%',
            'رفع نسبة المحتوى المحلي في القطاعات غير النفطية'
        ]
        // Add more as needed
    };
    
    if (level3Options[level2]) {
        level3Options[level2].forEach(option => {
            const opt = document.createElement('option');
            opt.value = option;
            opt.textContent = option;
            level3Select.appendChild(opt);
        });
    }
}

// Stub functions to be implemented
function viewKPIDetails(kpiId) {
    console.log('Viewing KPI:', kpiId);
    // Implement modal or navigation to details page
}

function editKPI(kpiId) {
    console.log('Editing KPI:', kpiId);
    // Implement edit functionality
}

function deleteKPI(kpiId) {
    if (confirm('هل تريد حذف هذا المؤشر؟')) {
        console.log('Deleting KPI:', kpiId);
        // Implement delete functionality
    }
}

function viewInitiative(initiativeId) {
    console.log('Viewing initiative:', initiativeId);
    // Navigate to initiative page
}

function removeDriver(index) {
    if (kpiData[0]?.contributingFactors?.drivers) {
        kpiData[0].contributingFactors.drivers.splice(index, 1);
        loadDriversAndObstacles();
    }
}

function removeObstacle(index) {
    if (kpiData[0]?.contributingFactors?.obstacles) {
        kpiData[0].contributingFactors.obstacles.splice(index, 1);
        loadDriversAndObstacles();
    }
}

// ============================================
// Initialize on page load
// ============================================

document.addEventListener('DOMContentLoaded', function() {
    console.log('Program Wizard JS loaded');
    
    // Initialize data
    initializeKPIData();
    initializeAchievementsData();
    initializeRisksData();
    
    console.log('Initialized data - Risks count:', risksData.length);
    
    // Initialize tooltips for wizard tabs
    const wizardTabTooltips = document.querySelectorAll('.wizard-tab-tooltip');
    wizardTabTooltips.forEach(function(element) {
        new bootstrap.Tooltip(element, {
            placement: 'top',
            trigger: 'hover'
        });
    });
    
    // Setup tab event listeners
    const kpiTab = document.getElementById('step2-tab');
    if (kpiTab) {
        kpiTab.addEventListener('shown.bs.tab', function() {
            console.log('KPI tab shown');
            loadKPIData();
        });
    }
    
    const achievementsTab = document.getElementById('step6-tab');
    if (achievementsTab) {
        achievementsTab.addEventListener('shown.bs.tab', function() {
            console.log('Achievements tab shown');
            loadAchievements();
        });
    }
    
    const risksTab = document.getElementById('step7-tab');
    if (risksTab) {
        console.log('Risks tab found, adding listener');
        risksTab.addEventListener('shown.bs.tab', function() {
            console.log('Risks tab shown - loading risks');
            loadRisks();
        });
    } else {
        console.error('Risks tab not found!');
    }
    
    // Test: Try to load risks after a delay to see if container exists
    setTimeout(function() {
        const testContainer = document.getElementById('risks-list');
        console.log('Test: Risks container exists?', !!testContainer);
        if (testContainer) {
            console.log('Test: Manually loading risks...');
            loadRisks();
        }
    }, 1000);
    
    // Setup form handlers
    const achievementForm = document.getElementById('achievement-form');
    if (achievementForm) {
        achievementForm.addEventListener('submit', function(e) {
            e.preventDefault();
            addNewAchievement();
        });
    }
    
    // Setup filter handlers
    const filterInputs = document.querySelectorAll('[id$="-filter"], #top5Only, #top5RisksOnly');
    filterInputs.forEach(input => {
        input.addEventListener('change', function() {
            if (this.closest('#step2')) {
                populateKPITable();
            } else if (this.closest('#step6')) {
                displayAchievementsList();
            } else if (this.closest('#step7')) {
                displayRisksList();
            }
        });
    });
    
    // Setup search handlers
    const searchInputs = document.querySelectorAll('[id$="-search"]');
    searchInputs.forEach(input => {
        input.addEventListener('input', function() {
            if (this.id === 'kpi-search') {
                populateKPITable();
            } else if (this.id === 'risk-search') {
                displayRisksList();
            }
        });
    });
});

// Export functions for global use
window.loadKPIData = loadKPIData;
window.loadAchievements = loadAchievements;
window.loadRisks = loadRisks;
window.viewKPIDetails = viewKPIDetails;
window.editKPI = editKPI;
window.deleteKPI = deleteKPI;
window.editAchievement = editAchievement;
window.archiveAchievement = archiveAchievement;
window.toggleTop5 = toggleTop5;
window.toggleTop5FromList = toggleTop5FromList;
window.viewInitiative = viewInitiative;
window.removeDriver = removeDriver;
window.removeObstacle = removeObstacle;
window.viewRiskDetails = viewRiskDetails;
window.editRisk = editRisk;
window.updateMitigation = updateMitigation;
window.toggleRiskTop5 = toggleRiskTop5;
window.toggleCategoryFields = toggleCategoryFields;
window.updateLevel2Options = updateLevel2Options;
window.updateLevel3Options = updateLevel3Options;

// ============= Budget Modal Functions =============
function validateBudgetAmounts() {
    const approved = parseFloat(document.getElementById('budgetApprovedAmount').value) || 0;
    const spent = parseFloat(document.getElementById('budgetSpentAmount').value) || 0;
    
    if (approved > 0) {
        const utilization = ((spent / approved) * 100).toFixed(1);
        const remaining = approved - spent;
        
        document.getElementById('budgetUtilizationPreview').style.display = 'block';
        document.getElementById('previewUtilization').textContent = utilization + '%';
        document.getElementById('previewRemaining').textContent = formatCurrency(remaining);
        
        const progressBar = document.getElementById('previewProgressBar');
        progressBar.style.width = utilization + '%';
        progressBar.textContent = utilization + '%';
        
        progressBar.className = 'progress-bar';
        if (utilization > 100) {
            progressBar.classList.add('bg-danger');
            showNotification('تحذير: المنصرف يتجاوز المعتمد', 'warning');
        } else if (utilization > 90) {
            progressBar.classList.add('bg-warning');
        } else if (utilization > 75) {
            progressBar.classList.add('bg-info');
        } else {
            progressBar.classList.add('bg-success');
        }
    } else {
        document.getElementById('budgetUtilizationPreview').style.display = 'none';
    }
}

// Reset modal when closed
document.addEventListener('DOMContentLoaded', function() {
    const budgetModal = document.getElementById('addBudgetModal');
    if (budgetModal) {
        budgetModal.addEventListener('hidden.bs.modal', function () {
            document.getElementById('budgetForm').reset();
            document.getElementById('budgetRecordId').value = '';
            document.getElementById('budgetUtilizationPreview').style.display = 'none';
        });
    }
});

window.validateBudgetAmounts = validateBudgetAmounts;
