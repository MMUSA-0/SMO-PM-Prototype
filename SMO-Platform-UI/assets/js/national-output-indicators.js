/**
 * National Output Indicators Module
 * مؤشرات الناتج الوطني
 * Version: 1.0.0
 * 
 * This module manages National Output Indicators for Vision 2030
 * including economic production, productivity, value added, and trade metrics
 */

// ==========================================
// Global Variables and Mock Data
// ==========================================

let currentIndicator = null;
let currentCategory = null;
let noiData = {};
let chartInstances = {};

// ==========================================
// Helper Functions (Must be defined first)
// ==========================================

function generateHistoricalData(startValue, endValue, quarters) {
    const data = [];
    const step = (endValue - startValue) / quarters;
    const startYear = 2020;
    
    for (let i = 0; i <= quarters; i++) {
        const year = startYear + Math.floor(i / 4);
        const quarter = (i % 4) + 1;
        data.push({
            period: `${year}-Q${quarter}`,
            value: startValue + (step * i) + (Math.random() * step * 0.2 - step * 0.1),
            target: startValue + (step * i * 1.1),
            actual: startValue + (step * i)
        });
    }
    return data;
}

function calculateAchievementPercentage(current, baseline, target) {
    if (target === baseline) return 0;
    return Math.round(((current - baseline) / (target - baseline)) * 100);
}

function formatNumber(num) {
    return new Intl.NumberFormat('ar-SA').format(num);
}

function getStatusClass(achievement) {
    if (achievement >= 100) return 'success';
    if (achievement >= 80) return 'primary';
    if (achievement >= 60) return 'warning';
    return 'danger';
}

function getStatusText(achievement) {
    if (achievement >= 100) return 'متقدم';
    if (achievement >= 80) return 'على المسار';
    if (achievement >= 60) return 'يحتاج متابعة';
    return 'حرج';
}

function getStatusBadge(achievement) {
    const status = getStatusClass(achievement);
    const text = getStatusText(achievement);
    return `<span class="badge bg-${status}">${text}</span>`;
}

// ==========================================
// Mock Data Structures
// ==========================================

// Complete NOI System: 60 National Output Indicators
// Distributed across 3 national priorities, 27 sectoral objectives, and 96 detailed objectives

const noiSystemInfo = {
    totalIndicators: 60,
    activeIndicators: 42,
    warningIndicators: 13,
    criticalIndicators: 5,
    
    distribution: {
        'مجتمع حيوي': { count: 18, achieved: 14, atRisk: 3, critical: 1 },
        'اقتصاد مزدهر': { count: 28, achieved: 18, atRisk: 7, critical: 3 },
        'وطن طموح': { count: 14, achieved: 10, atRisk: 3, critical: 1 }
    },
    
    levels: {
        'L1_Priority': 5,     // Top national indicators (GDP, Employment, etc.)
        'L2_Sectoral': 27,    // Sectoral indicators (Tourism, Industry, etc.)
        'Cross_Cutting': 28   // Cross-cutting indicators affecting multiple sectors
    },
    
    linkedStructure: {
        level1_priorities: 3,    // National priorities
        level2_objectives: 27,   // Sectoral objectives  
        level3_objectives: 96,   // Detailed strategic objectives
        vrps: 15,               // Vision Realization Programs
        nationalStrategies: 13,  // National strategies
        sectoralStrategies: 'Multiple'
    }
};

// Sample of the 60 National Output Indicators (showing 10 for detail)
const noiMockData = {
    'NOI.001': {
        code: 'NOI.001',
        nameAr: 'الناتج المحلي الإجمالي',
        nameEn: 'Gross Domestic Product',
        category: 'economic-production',
        subcategory: 'total-output',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.15,
        baseline: 2692000,
        baselineYear: 2020,
        target2030: 4000000,
        currentValue: 3581200,
        lastUpdate: '2024-12-31',
        source: 'الهيئة العامة للإحصاء (GASTAT)',
        owner: 'وزارة الاقتصاد والتخطيط',
        formula: 'C + I + G + (X - M)',
        description: 'إجمالي القيمة السوقية لجميع السلع والخدمات النهائية المنتجة في المملكة',
        historicalData: generateHistoricalData(2692000, 3581200, 20),
        verified: true,
        dataQualityScore: 98
    },
    'NOI.002': {
        code: 'NOI.002',
        nameAr: 'الناتج غير النفطي',
        nameEn: 'Non-Oil GDP',
        category: 'economic-production',
        subcategory: 'non-oil',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.20,
        baseline: 1580000,
        baselineYear: 2020,
        target2030: 2700000,
        currentValue: 2284800,
        lastUpdate: '2024-12-31',
        source: 'الهيئة العامة للإحصاء (GASTAT)',
        owner: 'وزارة الاقتصاد والتخطيط',
        historicalData: generateHistoricalData(1580000, 2284800, 20),
        verified: true,
        dataQualityScore: 97
    },
    'NOI.003': {
        code: 'NOI.003',
        nameAr: 'مساهمة القطاع الخاص',
        nameEn: 'Private Sector Contribution',
        category: 'private-sector',
        subcategory: 'contribution',
        unit: 'نسبة مئوية',
        unitEn: 'Percentage',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.18,
        baseline: 40,
        baselineYear: 2020,
        target2030: 65,
        currentValue: 48.5,
        lastUpdate: '2024-12-31',
        source: 'الهيئة العامة للإحصاء (GASTAT)',
        historicalData: generateHistoricalData(40, 48.5, 20)
    },
    'NOI.004': {
        code: 'NOI.004',
        nameAr: 'الصادرات غير النفطية',
        nameEn: 'Non-Oil Exports',
        category: 'trade-investment',
        subcategory: 'exports',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.12,
        baseline: 185,
        baselineYear: 2020,
        target2030: 535,
        currentValue: 381.4,
        lastUpdate: '2024-12-31',
        source: 'الهيئة العامة للجمارك',
        historicalData: generateHistoricalData(185, 381.4, 20)
    },
    'NOI.005': {
        code: 'NOI.005',
        nameAr: 'نسبة النساء في سوق العمل',
        nameEn: 'Women Labor Force Participation',
        category: 'employment',
        subcategory: 'gender-equality',
        unit: 'نسبة مئوية',
        unitEn: 'Percentage',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.08,
        baseline: 22.3,
        baselineYear: 2020,
        target2030: 30,
        currentValue: 35.6,
        lastUpdate: '2024-12-31',
        source: 'الهيئة العامة للإحصاء (GASTAT)',
        historicalData: generateHistoricalData(22.3, 35.6, 20)
    },
    'NOI.006': {
        code: 'NOI.006',
        nameAr: 'الاستثمار الأجنبي المباشر',
        nameEn: 'Foreign Direct Investment',
        category: 'trade-investment',
        subcategory: 'fdi',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.10,
        baseline: 18.5,
        baselineYear: 2020,
        target2030: 110,
        currentValue: 42.8,
        lastUpdate: '2024-12-31',
        source: 'هيئة الاستثمار (MISA)',
        historicalData: generateHistoricalData(18.5, 42.8, 20)
    },
    'NOI.015': {
        code: 'NOI.015',
        nameAr: 'عدد السياح',
        nameEn: 'Tourist Arrivals',
        category: 'trade-investment',
        subcategory: 'tourism',
        unit: 'مليون',
        unitEn: 'Million',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.12,
        baseline: 15.3,
        baselineYear: 2020,
        target2030: 100,
        currentValue: 27.1,
        lastUpdate: '2024-12-31',
        source: 'وزارة السياحة',
        owner: 'وزارة السياحة',
        historicalData: generateHistoricalData(15.3, 27.1, 20)
    },
    'NOI.023': {
        code: 'NOI.023',
        nameAr: 'الصادرات الصناعية',
        nameEn: 'Industrial Exports',
        category: 'economic-production',
        subcategory: 'industrial',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.10,
        baseline: 95.2,
        baselineYear: 2020,
        target2030: 200,
        currentValue: 156.3,
        lastUpdate: '2024-12-31',
        source: 'وزارة الصناعة',
        owner: 'وزارة الصناعة',
        historicalData: generateHistoricalData(95.2, 156.3, 20)
    },
    'NOI.031': {
        code: 'NOI.031',
        nameAr: 'نسبة التوطين في القطاع الخاص',
        nameEn: 'Private Sector Localization Rate',
        category: 'employment',
        subcategory: 'localization',
        unit: 'نسبة مئوية',
        unitEn: 'Percentage',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.08,
        baseline: 18.5,
        baselineYear: 2020,
        target2030: 30,
        currentValue: 22.8,
        lastUpdate: '2024-12-31',
        source: 'وزارة الموارد البشرية',
        owner: 'وزارة الموارد البشرية',
        historicalData: generateHistoricalData(18.5, 22.8, 20)
    },
    'NOI.042': {
        code: 'NOI.042',
        nameAr: 'مساهمة الاقتصاد الرقمي',
        nameEn: 'Digital Economy Contribution',
        category: 'digital-transformation',
        subcategory: 'digital',
        unit: 'نسبة مئوية',
        unitEn: 'Percentage',
        frequency: 'quarterly',
        polarity: 'increasing',
        weight: 0.09,
        baseline: 2.8,
        baselineYear: 2020,
        target2030: 10,
        currentValue: 5.8,
        lastUpdate: '2024-12-31',
        source: 'وزارة الاتصالات',
        owner: 'وزارة الاتصالات',
        historicalData: generateHistoricalData(2.8, 5.8, 20)
    }
};

// Categories configuration
const noiCategories = {
    'economic-production': {
        nameAr: 'الإنتاج الاقتصادي',
        nameEn: 'Economic Production',
        icon: 'bi-cash-stack',
        color: '#1F6046',
        indicators: ['NOI.001', 'NOI.002']
    },
    'private-sector': {
        nameAr: 'القطاع الخاص',
        nameEn: 'Private Sector',
        icon: 'bi-briefcase',
        color: '#227758',
        indicators: ['NOI.003']
    },
    'trade-investment': {
        nameAr: 'التجارة والاستثمار',
        nameEn: 'Trade & Investment',
        icon: 'bi-globe2',
        color: '#14553f',
        indicators: ['NOI.004', 'NOI.006']
    },
    'employment': {
        nameAr: 'التوظيف والعمالة',
        nameEn: 'Employment',
        icon: 'bi-people-fill',
        color: '#27A57B',
        indicators: ['NOI.005']
    },
    'diversification': {
        nameAr: 'التنويع الاقتصادي',
        nameEn: 'Economic Diversification',
        icon: 'bi-diagram-3',
        color: '#1F6046',
        indicators: []
    },
    'digital-transformation': {
        nameAr: 'التحول الرقمي',
        nameEn: 'Digital Transformation',
        icon: 'bi-phone',
        color: '#227758',
        indicators: []
    }
};

// Benchmark countries data
const benchmarkCountries = {
    'UAE': { nameAr: 'الإمارات', gdpPerCapita: 53.2, productivity: 175, nonOilShare: 70, privateSector: 60 },
    'QAT': { nameAr: 'قطر', gdpPerCapita: 85.3, productivity: 210, nonOilShare: 45, privateSector: 50 },
    'KWT': { nameAr: 'الكويت', gdpPerCapita: 28.4, productivity: 165, nonOilShare: 35, privateSector: 45 },
    'SGP': { nameAr: 'سنغافورة', gdpPerCapita: 84.5, productivity: 250, nonOilShare: 100, privateSector: 85 },
    'KOR': { nameAr: 'كوريا الجنوبية', gdpPerCapita: 34.8, productivity: 195, nonOilShare: 100, privateSector: 88 },
    'SAU': { nameAr: 'السعودية', gdpPerCapita: 32.9, productivity: 158, nonOilShare: 64, privateSector: 48.5 }
};

// Program contributions
const programContributions = {
    'NOI.001': [
        { programId: 'FSD', programName: 'برنامج تطوير القطاع المالي', contribution: 524, percentage: 22 },
        { programId: 'NIDLP', programName: 'برنامج تطوير الصناعة الوطنية والخدمات اللوجستية', contribution: 429, percentage: 18 },
        { programId: 'NTP', programName: 'برنامج التحول الوطني', contribution: 382, percentage: 16 },
        { programId: 'HSG', programName: 'برنامج الإسكان', contribution: 334, percentage: 14 },
        { programId: 'HCD', programName: 'برنامج تنمية القدرات البشرية', contribution: 286, percentage: 12 },
        { programId: 'OTH', programName: 'برامج أخرى', contribution: 429, percentage: 18 }
    ]
};

// ==========================================
// Helper Functions
// ==========================================

function generateHistoricalData(startValue, endValue, quarters) {
    const data = [];
    const step = (endValue - startValue) / quarters;
    const startYear = 2020;
    
    for (let i = 0; i <= quarters; i++) {
        const year = startYear + Math.floor(i / 4);
        const quarter = (i % 4) + 1;
        data.push({
            period: `${year}-Q${quarter}`,
            value: startValue + (step * i) + (Math.random() * step * 0.2 - step * 0.1),
            target: startValue + (step * i * 1.1),
            actual: startValue + (step * i)
        });
    }
    return data;
}

function calculateAchievementPercentage(current, baseline, target) {
    if (target === baseline) return 0;
    return Math.round(((current - baseline) / (target - baseline)) * 100);
}

function formatNumber(num) {
    return new Intl.NumberFormat('ar-SA').format(num);
}

function getStatusBadge(percentage) {
    if (percentage >= 90) return '<span class="badge bg-success">متقدم</span>';
    if (percentage >= 75) return '<span class="badge bg-success">على المسار</span>';
    if (percentage >= 60) return '<span class="badge bg-warning">يحتاج متابعة</span>';
    return '<span class="badge bg-danger">متأخر</span>';
}

function getStatusClass(percentage) {
    if (percentage >= 90) return 'level-1';
    if (percentage >= 75) return 'level-2';
    if (percentage >= 60) return 'level-3';
    return 'level-4';
}

// ==========================================
// Dashboard Functions
// ==========================================

function initializeDashboard() {
    try {
        // Only initialize charts that are currently visible
        const activeTab = document.querySelector('.tab-pane.active');
        
        if (activeTab) {
            // Initialize charts based on active tab
            if (activeTab.id === 'dashboard' || activeTab.id === 'analysis') {
                if (document.getElementById('trendChart')) {
                    initializeTrendChart();
                }
                if (document.getElementById('decompositionChart')) {
                    initializeDecompositionChart();
                }
            }
            
            if (activeTab.id === 'contributions') {
                if (document.getElementById('contributionChart')) {
                    initializeContributionChart();
                }
            }
        }
        
        // Load initial data
        loadDashboardMetrics();
        
        // Set up event listeners
        setupEventListeners();
    } catch (error) {
        console.error('Error in initializeDashboard:', error);
    }
}

function loadDashboardMetrics() {
    // Update key metric cards - these would normally come from API
    console.log('Loading dashboard metrics...');
}

// ==========================================
// Category Management
// ==========================================

function loadCategory(categoryId) {
    currentCategory = categoryId;
    const category = noiCategories[categoryId];
    
    if (!category) return;
    
    // Show category details card
    const detailsCard = document.getElementById('categoryDetails');
    const titleElement = document.getElementById('categoryTitle');
    const contentElement = document.getElementById('categoryContent');
    
    if (detailsCard && titleElement && contentElement) {
        titleElement.textContent = category.nameAr;
        
        // Build indicators table for this category
        let tableHtml = `
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>المؤشر</th>
                            <th>القيمة الحالية</th>
                            <th>المستهدف</th>
                            <th>الإنجاز</th>
                            <th>الحالة</th>
                        </tr>
                    </thead>
                    <tbody>
        `;
        
        category.indicators.forEach(indicatorId => {
            const indicator = noiMockData[indicatorId];
            if (indicator) {
                const achievement = calculateAchievementPercentage(
                    indicator.currentValue,
                    indicator.baseline,
                    indicator.target2030
                );
                
                tableHtml += `
                    <tr>
                        <td>${indicator.nameAr}</td>
                        <td>${formatNumber(indicator.currentValue)} ${indicator.unit}</td>
                        <td>${formatNumber(indicator.target2030)}</td>
                        <td>${achievement}%</td>
                        <td>${getStatusBadge(achievement)}</td>
                    </tr>
                `;
            }
        });
        
        tableHtml += '</tbody></table></div>';
        contentElement.innerHTML = tableHtml;
        detailsCard.style.display = 'block';
        
        // Highlight selected category
        document.querySelectorAll('.category-card').forEach(card => {
            card.classList.remove('active');
        });
        event.target.closest('.category-card').classList.add('active');
    }
}

// ==========================================
// Indicator Management
// ==========================================

function viewDetails(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) {
        alert('المؤشر غير موجود في البيانات');
        return;
    }
    
    const achievement = calculateAchievementPercentage(
        indicator.currentValue, 
        indicator.baseline, 
        indicator.target2030
    );
    
    // Create modal HTML
    let modalHtml = `
        <div class="modal fade" id="detailsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-info-circle"></i> تفاصيل المؤشر: ${indicator.nameAr}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <h6 class="text-muted">القيمة الحالية</h6>
                                        <h2 class="mb-0">${formatNumber(indicator.currentValue)}</h2>
                                        <small>${indicator.unit}</small>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <h6 class="text-muted">المستهدف 2030</h6>
                                        <h2 class="mb-0">${formatNumber(indicator.target2030)}</h2>
                                        <small>${indicator.unit}</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="card mb-3">
                            <div class="card-body">
                                <h6>نسبة الإنجاز</h6>
                                <div class="progress" style="height: 30px;">
                                    <div class="progress-bar ${getStatusClass(achievement)}" style="width: ${achievement}%;">
                                        ${achievement}%
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <span class="badge bg-${getStatusClass(achievement)}">${getStatusText(achievement)}</span>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-6">
                                <p><strong>الكود:</strong> ${indicator.code}</p>
                                <p><strong>الفئة:</strong> ${indicator.category}</p>
                                <p><strong>التكرار:</strong> ${indicator.frequency}</p>
                            </div>
                            <div class="col-md-6">
                                <p><strong>المصدر:</strong> ${indicator.source}</p>
                                <p><strong>آخر تحديث:</strong> ${indicator.lastUpdate}</p>
                                <p><strong>الوزن:</strong> ${indicator.weight}</p>
                            </div>
                        </div>
                        
                        ${indicator.description ? `
                        <div class="alert alert-info">
                            <strong>الوصف:</strong> ${indicator.description}
                        </div>
                        ` : ''}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="editIndicator('${indicatorId}')">
                            <i class="bi bi-pencil"></i> تعديل
                        </button>
                        <button type="button" class="btn btn-success" onclick="exportData('${indicatorId}')">
                            <i class="bi bi-download"></i> تصدير
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('detailsModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('detailsModal'));
    modal.show();
}

function editIndicator(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) {
        alert('المؤشر غير موجود في البيانات');
        return;
    }
    
    currentIndicator = indicator;
    
    // Close details modal if open
    const detailsModal = document.getElementById('detailsModal');
    if (detailsModal) {
        const bsModal = bootstrap.Modal.getInstance(detailsModal);
        if (bsModal) bsModal.hide();
    }
    
    // Switch to data entry tab
    const dataEntryTab = document.getElementById('data-entry-tab');
    if (dataEntryTab) {
        dataEntryTab.click();
        
        // Fill form after a short delay to ensure tab is loaded
        setTimeout(() => {
            const select = document.getElementById('indicatorSelect');
            if (select) {
                select.value = indicatorId;
                loadIndicatorFields();
            }
        }, 100);
    } else {
        alert(`تم تحديد المؤشر: ${indicator.nameAr}\nللتعديل، انتقل إلى تبويب "إدخال البيانات"`);
    }
}

function viewTrend(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) {
        alert('المؤشر غير موجود في البيانات');
        return;
    }
    
    // Switch to analysis tab and show trend
    const analysisTab = document.getElementById('analysis-tab');
    if (analysisTab) {
        analysisTab.click();
        setTimeout(() => {
            updateTrendChart(indicator);
        }, 100);
    } else {
        alert(`عرض الاتجاه للمؤشر: ${indicator.nameAr}`);
    }
}

function exportData(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) {
        alert('المؤشر غير موجود في البيانات');
        return;
    }
    
    // Create CSV data
    const csvData = [
        ['الكود', 'الاسم', 'القيمة الحالية', 'الوحدة', 'المستهدف 2030', 'نسبة الإنجاز', 'المصدر', 'آخر تحديث'],
        [
            indicator.code,
            indicator.nameAr,
            indicator.currentValue,
            indicator.unit,
            indicator.target2030,
            calculateAchievementPercentage(indicator.currentValue, indicator.baseline, indicator.target2030) + '%',
            indicator.source,
            indicator.lastUpdate
        ]
    ];
    
    // Convert to CSV string
    const csvString = csvData.map(row => row.join(',')).join('\n');
    
    // Create download link
    const blob = new Blob(['\ufeff' + csvString], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${indicator.code}_${indicator.nameAr}.csv`;
    link.click();
    
    // Show success message
    setTimeout(() => {
        alert(`تم تصدير البيانات: ${indicator.nameAr}\nالملف: ${indicator.code}_${indicator.nameAr}.csv`);
    }, 100);
}

// ==========================================
// Data Entry Functions
// ==========================================

function loadIndicatorFields() {
    const select = document.getElementById('indicatorSelect');
    const unitLabel = document.getElementById('unitLabel');
    
    if (select && select.value && unitLabel) {
        const indicator = noiMockData[select.value];
        if (indicator) {
            unitLabel.textContent = indicator.unit;
        }
    }
}

function clearForm() {
    document.getElementById('dataEntryForm').reset();
    document.getElementById('unitLabel').textContent = 'مليار ريال';
}

// ==========================================
// Chart Functions
// ==========================================

// Utility function to safely destroy a chart
function destroyChart(chartName) {
    if (chartInstances[chartName]) {
        try {
            chartInstances[chartName].destroy();
            chartInstances[chartName] = null;
            delete chartInstances[chartName];
        } catch (error) {
            console.error(`Error destroying chart ${chartName}:`, error);
        }
    }
}

// Utility function to destroy all charts
function destroyAllCharts() {
    Object.keys(chartInstances).forEach(chartName => {
        destroyChart(chartName);
    });
}

function initializeTrendChart() {
    const ctx = document.getElementById('trendChart');
    if (!ctx) return;
    
    // Safely destroy existing chart if it exists
    destroyChart('trendChart');
    
    const indicator = noiMockData['NOI.001']; // Default to GDP
    
    // Limit data points to prevent overflow
    const maxDataPoints = 20;
    const dataPoints = indicator.historicalData.slice(-maxDataPoints);
    
    chartInstances.trendChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: dataPoints.map(d => d.period),
            datasets: [{
                label: 'القيمة الفعلية',
                data: dataPoints.map(d => d.actual),
                borderColor: 'rgb(31, 96, 70)',
                backgroundColor: 'rgba(31, 96, 70, 0.1)',
                tension: 0.4,
                pointRadius: 3,
                pointHoverRadius: 5
            }, {
                label: 'المستهدف',
                data: dataPoints.map(d => d.target),
                borderColor: 'rgb(39, 165, 123)',
                backgroundColor: 'rgba(39, 165, 123, 0.1)',
                borderDash: [5, 5],
                tension: 0.4,
                pointRadius: 3,
                pointHoverRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        padding: 10,
                        font: {
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'اتجاه المؤشر عبر الزمن',
                    font: {
                        size: 14
                    }
                }
            },
            scales: {
                x: {
                    ticks: {
                        autoSkip: true,
                        maxTicksLimit: 10,
                        font: {
                            size: 11
                        }
                    }
                },
                y: {
                    beginAtZero: false,
                    ticks: {
                        font: {
                            size: 11
                        }
                    }
                }
            },
            layout: {
                padding: 10
            }
        }
    });
}

function updateTrendChart(indicator) {
    if (!chartInstances.trendChart) {
        initializeTrendChart();
        return;
    }
    
    chartInstances.trendChart.data.labels = indicator.historicalData.map(d => d.period);
    chartInstances.trendChart.data.datasets[0].data = indicator.historicalData.map(d => d.actual);
    chartInstances.trendChart.data.datasets[1].data = indicator.historicalData.map(d => d.target);
    chartInstances.trendChart.options.plugins.title.text = `اتجاه ${indicator.nameAr} عبر الزمن`;
    chartInstances.trendChart.update();
}

function initializeDecompositionChart() {
    const ctx = document.getElementById('decompositionChart');
    if (!ctx) return;
    
    // Safely destroy existing chart if it exists
    destroyChart('decompositionChart');
    
    chartInstances.decompositionChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['القطاع النفطي', 'الصناعة', 'الخدمات', 'الزراعة', 'البناء'],
            datasets: [{
                data: [36, 18, 38, 2, 6],
                backgroundColor: [
                    'rgba(31, 96, 70, 0.8)',
                    'rgba(34, 119, 88, 0.8)',
                    'rgba(39, 165, 123, 0.8)',
                    'rgba(20, 85, 63, 0.8)',
                    'rgba(17, 75, 51, 0.8)'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: 'توزيع الناتج المحلي حسب القطاعات'
                }
            }
        }
    });
}

function initializeContributionChart() {
    const ctx = document.getElementById('contributionChart');
    if (!ctx) return;
    
    // Safely destroy existing chart if it exists
    destroyChart('contributionChart');
    
    const contributions = programContributions['NOI.001'] || [];
    
    chartInstances.contributionChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: contributions.map(c => c.programName),
            datasets: [{
                data: contributions.map(c => c.contribution),
                backgroundColor: [
                    'rgba(31, 96, 70, 0.8)',
                    'rgba(39, 165, 123, 0.8)',
                    'rgba(34, 119, 88, 0.8)',
                    'rgba(20, 85, 63, 0.8)',
                    'rgba(17, 75, 51, 0.8)',
                    'rgba(93, 93, 93, 0.6)'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: 'مساهمات البرامج في الناتج المحلي'
                }
            }
        }
    });
}

// ==========================================
// Event Listeners
// ==========================================

function setupEventListeners() {
    try {
        // Data entry form submission
        const dataEntryForm = document.getElementById('dataEntryForm');
        if (dataEntryForm) {
            dataEntryForm.addEventListener('submit', function(e) {
                e.preventDefault();
                saveIndicatorData();
            });
        }
        
        // Character counter for notes
        const notesField = document.getElementById('notes');
        if (notesField) {
            notesField.addEventListener('input', function() {
                const charCount = document.querySelector('.char-count');
                if (charCount) {
                    charCount.textContent = `${this.value.length} / 500`;
                }
            });
        }
    } catch (error) {
        console.error('Error setting up event listeners:', error);
    }
}

function saveIndicatorData() {
    // Collect form data
    const formData = {
        indicator: document.getElementById('indicatorSelect').value,
        year: document.getElementById('yearSelect').value,
        quarter: document.getElementById('quarterSelect').value,
        actualValue: document.getElementById('actualValue').value,
        source: document.getElementById('sourceSelect').value,
        notes: document.getElementById('notes').value,
        verified: document.getElementById('verifiedCheck').checked
    };
    
    console.log('Saving indicator data:', formData);
    
    // Show success message (would normally save to backend)
    alert('تم حفظ البيانات بنجاح');
    clearForm();
    
    // Add to recent entries
    addToRecentEntries(formData);
}

function addToRecentEntries(data) {
    // This would update the recent entries timeline
    console.log('Adding to recent entries:', data);
}

// ==========================================
// API Integration Functions (Placeholder)
// ==========================================

async function fetchIndicatorData(indicatorId) {
    // This would fetch real data from the API
    return noiMockData[indicatorId];
}

async function saveIndicatorValue(indicatorId, period, value) {
    // This would save to the backend
    console.log(`Saving ${indicatorId} for ${period}: ${value}`);
    return { success: true };
}

async function fetchBenchmarkData(countries, indicator) {
    // This would fetch benchmark data from API
    return benchmarkCountries;
}

// ==========================================
// NOI Complexity Management Functions
// ==========================================

/**
 * Get indicators by level and category
 * Manages the complexity of 60 indicators across multiple dimensions
 */
function getIndicatorsByFilter(level = null, priority = null, status = null, ministry = null) {
    // In reality, this would filter through all 60 indicators
    let filtered = Object.values(noiMockData);
    
    if (level) {
        // Filter by L1, L2, or Cross-cutting level
        filtered = filtered.filter(ind => ind.level === level);
    }
    
    if (priority) {
        // Filter by national priority (مجتمع حيوي, اقتصاد مزدهر, وطن طموح)
        filtered = filtered.filter(ind => ind.priority === priority);
    }
    
    if (status) {
        // Filter by achievement status
        if (status === 'critical') {
            filtered = filtered.filter(ind => ind.achievement < 50);
        } else if (status === 'warning') {
            filtered = filtered.filter(ind => ind.achievement >= 50 && ind.achievement < 75);
        } else if (status === 'ontrack') {
            filtered = filtered.filter(ind => ind.achievement >= 75);
        }
    }
    
    if (ministry) {
        // Filter by responsible ministry
        filtered = filtered.filter(ind => ind.owner === ministry);
    }
    
    return filtered;
}

/**
 * Calculate NOI cascade impact
 * Shows how a single NOI impacts multiple levels and objectives
 */
function calculateNOICascade(indicatorId) {
    // Each NOI typically impacts:
    // - 3-5 Level 3 objectives (from 96 total)
    // - 2-4 Vision Realization Programs (from 15 total)  
    // - 1-3 National Strategies (from 13 total)
    // - Multiple sectoral strategies
    
    const cascade = {
        indicator: indicatorId,
        impactedObjectives: {
            level3: Math.floor(Math.random() * 3) + 3,  // 3-5 objectives
            programs: Math.floor(Math.random() * 3) + 2, // 2-4 programs
            strategies: Math.floor(Math.random() * 3) + 1 // 1-3 strategies
        },
        totalImpactedEntities: 0,
        complexityScore: 0
    };
    
    cascade.totalImpactedEntities = 
        cascade.impactedObjectives.level3 + 
        cascade.impactedObjectives.programs + 
        cascade.impactedObjectives.strategies;
    
    // Higher complexity = more entities impacted
    cascade.complexityScore = Math.min(100, cascade.totalImpactedEntities * 10);
    
    return cascade;
}

/**
 * Get NOI accountability chain
 * Shows the full accountability structure for each indicator
 */
function getNOIAccountability(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) return null;
    
    return {
        indicator: indicatorId,
        ultimateOwner: 'Crown Prince',
        minister: indicator.owner || 'وزير الاقتصاد',
        programOwners: [
            'مدير برنامج تطوير الصناعة',
            'مدير برنامج الصادرات',
            'مدير برنامج الاستثمار'
        ],
        entityHeads: [
            'رئيس هيئة الاستثمار',
            'رئيس هيئة الصناعة',
            'رئيس هيئة الصادرات'
        ],
        escalationPath: [
            { level: 1, responsible: 'Entity Head', timeframe: '24 hours' },
            { level: 2, responsible: 'Program Owner', timeframe: '3 days' },
            { level: 3, responsible: 'Minister', timeframe: '1 week' },
            { level: 4, responsible: 'Cabinet', timeframe: 'Immediate' }
        ]
    };
}

// ==========================================
// Error Handling
// ==========================================

// Global error handler to prevent page crashes
window.addEventListener('error', function(event) {
    console.error('Global error caught:', event.error);
    // Prevent the default error handling
    event.preventDefault();
});

// Handle unhandled promise rejections
window.addEventListener('unhandledrejection', function(event) {
    console.error('Unhandled promise rejection:', event.reason);
    // Prevent the default handling
    event.preventDefault();
});

// ==========================================
// Unified Service Button Handlers
// ==========================================

function initializeUnifiedServiceButtons() {
    // Handle service button clicks
    document.querySelectorAll('.service-btn[data-bs-toggle="pill"]').forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all service buttons
            document.querySelectorAll('.service-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            
            // Add active class to clicked button
            this.classList.add('active');
        });
    });
    
    // Handle tab changes triggered by Bootstrap
    const tabButtons = document.querySelectorAll('[data-bs-toggle="pill"]');
    tabButtons.forEach(button => {
        button.addEventListener('shown.bs.tab', function() {
            // Sync service button active states
            const targetId = this.getAttribute('data-bs-target');
            const serviceBtn = document.querySelector(`.service-btn[data-bs-target="${targetId}"]`);
            if (serviceBtn && !serviceBtn.classList.contains('active')) {
                document.querySelectorAll('.service-btn').forEach(btn => {
                    btn.classList.remove('active');
                });
                serviceBtn.classList.add('active');
            }
        });
    });
}

// ==========================================
// Initialization
// ==========================================

document.addEventListener('DOMContentLoaded', function() {
    console.log('National Output Indicators module loaded');
    
    // Initialize dashboard on page load
    initializeDashboard();
    
    // Apply role-based permissions if available
    if (typeof applyRolePermissions === 'function') {
        applyRolePermissions();
    }
    
    // Initialize unified service buttons
    initializeUnifiedServiceButtons();
    
    // Diagnostic: List all available tabs
    const availableTabs = document.querySelectorAll('.tab-pane');
    console.log('Available tabs:', Array.from(availableTabs).map(tab => tab.id));
    
    // Diagnostic: List all service buttons
    const serviceButtons = document.querySelectorAll('.service-btn[data-bs-toggle="pill"]');
    console.log('Service buttons:', Array.from(serviceButtons).map(btn => btn.getAttribute('data-bs-target')));
    
    // Handle tab switching with safer event delegation
    try {
        const tabContainer = document.getElementById('noiTabs');
        if (tabContainer) {
            tabContainer.addEventListener('click', function(event) {
                const button = event.target.closest('[data-bs-toggle="pill"]');
                if (button) {
                    const targetId = button.getAttribute('data-bs-target');
                    
                    // Handle chart initialization for specific tabs
                    setTimeout(() => {
                        if (targetId === '#analysis') {
                            // Destroy and reinitialize charts for proper sizing
                            if (chartInstances.trendChart) {
                                chartInstances.trendChart.destroy();
                                chartInstances.trendChart = null;
                            }
                            if (chartInstances.decompositionChart) {
                                chartInstances.decompositionChart.destroy();
                                chartInstances.decompositionChart = null;
                            }
                            
                            initializeTrendChart();
                            initializeDecompositionChart();
                        } else if (targetId === '#contributions') {
                            if (chartInstances.contributionChart) {
                                chartInstances.contributionChart.destroy();
                                chartInstances.contributionChart = null;
                            }
                            initializeContributionChart();
                        }
                    }, 300);
                }
            });
        }
    } catch (error) {
        console.error('Error setting up tab handlers:', error);
    }
});

// ==========================================
// Integration Functions
// ==========================================

function showContributions(indicatorId) {
    const indicator = noiMockData[indicatorId];
    if (!indicator) return;
    
    // Create modal showing program contributions
    const contributions = programContributions[indicatorId] || programContributions['NOI.001'];
    
    let modalHtml = `
        <div class="modal fade" id="contributionsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="bi bi-diagram-2"></i> مساهمات البرامج في ${indicator.nameAr}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-info">
                            <strong>القيمة الحالية:</strong> ${formatNumber(indicator.currentValue)} ${indicator.unit}
                            <br>
                            <strong>المستهدف 2030:</strong> ${formatNumber(indicator.target2030)} ${indicator.unit}
                        </div>
                        <h6>البرامج المساهمة:</h6>
                        <div class="list-group">`;
    
    contributions.forEach(contrib => {
        modalHtml += `
            <div class="list-group-item">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${contrib.programName}</h6>
                        <small class="text-muted">رمز البرنامج: ${contrib.programId}</small>
                    </div>
                    <div class="text-end">
                        <div class="badge bg-primary">${contrib.percentage}%</div>
                        <div class="small text-muted">+${contrib.contribution} مليار ريال</div>
                    </div>
                </div>
                <div class="progress mt-2" style="height: 10px;">
                    <div class="progress-bar" style="width: ${contrib.percentage * 2}%; background-color: #1F6046;"></div>
                </div>
            </div>`;
    });
    
    modalHtml += `
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="drillDownToPrograms('${indicatorId}')">
                            <i class="bi bi-arrow-down"></i> عرض تفاصيل البرامج
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('contributionsModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('contributionsModal'));
    modal.show();
}

function showAccountability(indicatorId) {
    // Show accountability chain for this NOI
    const indicator = noiMockData[indicatorId];
    if (!indicator) {
        alert('المؤشر غير موجود');
        return;
    }
    
    const accountability = getNOIAccountability(indicatorId);
    
    let modalHtml = `
        <div class="modal fade" id="accountabilityModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-person-badge"></i> سلسلة المساءلة: ${indicator.nameAr}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-warning">
                            <strong>المسؤول النهائي:</strong> ${accountability.ultimateOwner}
                        </div>
                        
                        <h6>سلسلة المسؤولية:</h6>
                        <div class="list-group mb-3">
                            <div class="list-group-item list-group-item-danger">
                                <strong>الوزير المسؤول:</strong> ${accountability.minister}
                            </div>`;
    
    accountability.programOwners.forEach((owner, idx) => {
        modalHtml += `
            <div class="list-group-item">
                <strong>مدير برنامج ${idx + 1}:</strong> ${owner}
            </div>`;
    });
    
    modalHtml += `
                        </div>
                        
                        <h6>مصفوفة التصعيد:</h6>
                        <table class="table table-bordered table-sm">
                            <thead>
                                <tr>
                                    <th>المستوى</th>
                                    <th>المسؤول</th>
                                    <th>الإطار الزمني</th>
                                </tr>
                            </thead>
                            <tbody>`;
    
    accountability.escalationPath.forEach(step => {
        modalHtml += `
                                <tr>
                                    <td>المستوى ${step.level}</td>
                                    <td>${step.responsible}</td>
                                    <td>${step.timeframe}</td>
                                </tr>`;
    });
    
    modalHtml += `
                            </tbody>
                        </table>
                        
                        <div class="alert alert-info mt-3">
                            <strong>الإجراء المطلوب:</strong> تقديم خطة تصحيحية خلال 7 أيام
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger">
                            <i class="bi bi-bell"></i> تصعيد للوزير
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('accountabilityModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('accountabilityModal'));
    modal.show();
}

function viewFullCascade() {
    // Navigate to cascade view showing full hierarchy
    alert('عرض التسلسل الكامل من المؤشرات التشغيلية إلى مؤشرات الناتج الوطني');
    // In production, this would navigate to a detailed cascade view page
}

function drillDownToPrograms(indicatorId) {
    // Navigate to programs page filtered by this NOI
    window.location.href = `performance-programs.html?noi=${indicatorId}`;
}

// ==========================================
// Role-Based Action Functions (ADAA Analyst)
// ==========================================

function updateNOI() {
    // Open modal to update NOI values
    const modalHtml = `
        <div class="modal fade" id="updateNOIModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-pencil"></i> تحديث مؤشرات الناتج الوطني
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-info">
                            <i class="bi bi-info-circle"></i> اختر المؤشر المراد تحديثه
                        </div>
                        <div class="row">
                            ${Object.entries(noiMockData).slice(0, 6).map(([key, indicator]) => `
                                <div class="col-md-6 mb-3">
                                    <div class="card">
                                        <div class="card-body">
                                            <h6>${indicator.nameAr}</h6>
                                            <p class="mb-1">القيمة الحالية: ${formatNumber(indicator.currentValue)} ${indicator.unit}</p>
                                            <button class="btn btn-sm btn-primary" onclick="editIndicator('${key}')">
                                                <i class="bi bi-pencil"></i> تحديث
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            `).join('')}
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('updateNOIModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body and show
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('updateNOIModal'));
    modal.show();
}

function calculateContributions() {
    // Calculate and display program contributions to NOI
    const modalHtml = `
        <div class="modal fade" id="calcContributionsModal" tabindex="-1">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-calculator"></i> حساب مساهمات البرامج في المؤشرات
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-success">
                            <i class="bi bi-check-circle"></i> تم حساب المساهمات بنجاح
                        </div>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>المؤشر</th>
                                    <th>القيمة الحالية</th>
                                    <th>عدد البرامج المساهمة</th>
                                    <th>أكبر مساهم</th>
                                    <th>نسبة المساهمة</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${Object.entries(noiMockData).slice(0, 6).map(([key, indicator]) => {
                                    const contributions = programContributions[key] || [];
                                    const topContributor = contributions[0] || { programName: 'غير محدد', percentage: 0 };
                                    return `
                                        <tr>
                                            <td>${indicator.nameAr}</td>
                                            <td>${formatNumber(indicator.currentValue)} ${indicator.unit}</td>
                                            <td><span class="badge bg-primary">${contributions.length || 6}</span></td>
                                            <td>${topContributor.programName}</td>
                                            <td>
                                                <div class="progress" style="height: 20px;">
                                                    <div class="progress-bar" style="width: ${topContributor.percentage || 22}%">
                                                        ${topContributor.percentage || 22}%
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    `;
                                }).join('')}
                            </tbody>
                        </table>
                        <div class="alert alert-info mt-3">
                            <strong>الخلاصة:</strong> إجمالي 15 برنامج يساهم في 60 مؤشر ناتج وطني
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" onclick="exportCalculations()">
                            <i class="bi bi-download"></i> تصدير النتائج
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('calcContributionsModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body and show
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('calcContributionsModal'));
    modal.show();
}

function linkPrograms() {
    // Link programs to NOI
    const modalHtml = `
        <div class="modal fade" id="linkProgramsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-info text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-link-45deg"></i> ربط البرامج بمؤشرات الناتج الوطني
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h6>اختر المؤشر:</h6>
                                <select class="form-select mb-3" id="linkIndicatorSelect">
                                    ${Object.entries(noiMockData).map(([key, indicator]) => 
                                        `<option value="${key}">${indicator.nameAr}</option>`
                                    ).join('')}
                                </select>
                            </div>
                            <div class="col-md-6">
                                <h6>البرامج المتاحة:</h6>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" value="FSD" checked>
                                    <label class="form-check-label">برنامج تطوير القطاع المالي</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" value="NIDLP" checked>
                                    <label class="form-check-label">برنامج تطوير الصناعة الوطنية</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" value="NTP">
                                    <label class="form-check-label">برنامج التحول الوطني</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" value="HSG">
                                    <label class="form-check-label">برنامج الإسكان</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" value="HCD">
                                    <label class="form-check-label">برنامج تنمية القدرات البشرية</label>
                                </div>
                            </div>
                        </div>
                        <hr>
                        <div class="alert alert-warning">
                            <i class="bi bi-diagram-3"></i> البرامج المرتبطة حالياً: 3 من 15
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="saveProgramLinks()">
                            <i class="bi bi-save"></i> حفظ الروابط
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('linkProgramsModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body and show
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('linkProgramsModal'));
    modal.show();
}

function generateQuarterlyReport() {
    // Generate quarterly NOI report
    const quarter = 'Q4 2024';
    const modalHtml = `
        <div class="modal fade" id="quarterlyReportModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-dark text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-file-earmark-pdf"></i> التقرير الربعي - ${quarter}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="card mb-3">
                            <div class="card-body">
                                <h6>ملخص الأداء:</h6>
                                <div class="row text-center">
                                    <div class="col-md-3">
                                        <h4 class="text-success">42</h4>
                                        <small>على المسار</small>
                                    </div>
                                    <div class="col-md-3">
                                        <h4 class="text-warning">13</h4>
                                        <small>يحتاج متابعة</small>
                                    </div>
                                    <div class="col-md-3">
                                        <h4 class="text-danger">5</h4>
                                        <small>حرج</small>
                                    </div>
                                    <div class="col-md-3">
                                        <h4 class="text-primary">72.3%</h4>
                                        <small>متوسط الإنجاز</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="card">
                            <div class="card-body">
                                <h6>أبرز الإنجازات:</h6>
                                <ul>
                                    <li>نسبة النساء في سوق العمل تجاوزت المستهدف (119%)</li>
                                    <li>الناتج المحلي الإجمالي حقق 82% من المستهدف</li>
                                    <li>الصادرات غير النفطية نمت بنسبة 12% هذا الربع</li>
                                </ul>
                                
                                <h6>التحديات:</h6>
                                <ul>
                                    <li>الاستثمار الأجنبي المباشر عند 39% فقط</li>
                                    <li>عدد السياح أقل من المتوقع (27%)</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" onclick="downloadPDFReport()">
                            <i class="bi bi-file-earmark-pdf"></i> تحميل PDF
                        </button>
                        <button type="button" class="btn btn-success" onclick="downloadExcelReport()">
                            <i class="bi bi-file-earmark-excel"></i> تحميل Excel
                        </button>
                        <button type="button" class="btn btn-primary" onclick="sendReportToCouncil()">
                            <i class="bi bi-send"></i> إرسال للمجلس
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('quarterlyReportModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body and show
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('quarterlyReportModal'));
    modal.show();
}

function createAlert() {
    // Create alert for NOI thresholds
    const modalHtml = `
        <div class="modal fade" id="createAlertModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-warning">
                        <h5 class="modal-title">
                            <i class="bi bi-bell"></i> إنشاء تنبيه جديد
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">المؤشر:</label>
                            <select class="form-select">
                                ${Object.entries(noiMockData).map(([key, indicator]) => 
                                    `<option value="${key}">${indicator.nameAr}</option>`
                                ).join('')}
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">نوع التنبيه:</label>
                            <select class="form-select">
                                <option>انخفاض عن الحد الأدنى</option>
                                <option>تجاوز الحد الأعلى</option>
                                <option>تغيير مفاجئ</option>
                                <option>عدم تحديث البيانات</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">القيمة الحدية:</label>
                            <input type="number" class="form-control" value="70">
                        </div>
                        <div class="mb-3">
                            <label class="form-label">المستلمون:</label>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" checked>
                                <label class="form-check-label">الوزير المسؤول</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" checked>
                                <label class="form-check-label">مدير البرنامج</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox">
                                <label class="form-check-label">فريق ADAA</label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-warning" onclick="saveAlert()">
                            <i class="bi bi-bell"></i> إنشاء التنبيه
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
                    </div>
                </div>
            </div>
        </div>`;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('createAlertModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to body and show
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('createAlertModal'));
    modal.show();
}

// Helper functions for role-based actions
function exportCalculations() {
    alert('تم تصدير حسابات المساهمات إلى ملف Excel');
}

function saveProgramLinks() {
    alert('تم حفظ روابط البرامج بنجاح');
    const modal = bootstrap.Modal.getInstance(document.getElementById('linkProgramsModal'));
    if (modal) modal.hide();
}

function downloadPDFReport() {
    alert('جاري تحميل التقرير بصيغة PDF...');
}

function downloadExcelReport() {
    alert('جاري تحميل التقرير بصيغة Excel...');
}

function sendReportToCouncil() {
    alert('تم إرسال التقرير إلى مجلس الوزراء بنجاح');
}

function saveAlert() {
    alert('تم إنشاء التنبيه بنجاح وسيتم إرسال الإشعارات عند تحقق الشرط');
    const modal = bootstrap.Modal.getInstance(document.getElementById('createAlertModal'));
    if (modal) modal.hide();
}

// ==========================================
// Export Functions for External Use
// ==========================================

// Export to window.NOI namespace
window.NOI = {
    viewDetails,
    editIndicator,
    viewTrend,
    exportData,
    loadCategory,
    loadIndicatorFields,
    clearForm,
    saveIndicatorData,
    showContributions,
    showAccountability,
    viewFullCascade,
    drillDownToPrograms,
    getIndicatorsByFilter,
    calculateNOICascade,
    getNOIAccountability,
    // Role-based actions
    updateNOI,
    calculateContributions,
    linkPrograms,
    generateQuarterlyReport,
    createAlert
};

// Also export functions globally for onclick handlers
window.viewDetails = viewDetails;
window.editIndicator = editIndicator;
window.viewTrend = viewTrend;
window.exportData = exportData;
window.loadCategory = loadCategory;
window.loadIndicatorFields = loadIndicatorFields;
window.clearForm = clearForm;
window.saveIndicatorData = saveIndicatorData;
window.showContributions = showContributions;
window.showAccountability = showAccountability;
window.viewFullCascade = viewFullCascade;
window.drillDownToPrograms = drillDownToPrograms;

// Role-based action functions
window.updateNOI = updateNOI;
window.calculateContributions = calculateContributions;
window.linkPrograms = linkPrograms;
window.generateQuarterlyReport = generateQuarterlyReport;
window.createAlert = createAlert;

// New enhanced UI functions
window.showIndicatorDetails = showIndicatorDetails;
window.showAllIndicators = showAllIndicators;
window.exportIndicatorReport = exportIndicatorReport;
window.getCategoryName = getCategoryName;
window.loadMoreIndicators = loadMoreIndicators;
window.switchToTab = switchToTab;

// ==========================================
// New Enhanced UI Functions
// ==========================================

function showIndicatorDetails(indicatorCode) {
    const indicator = noiMockData[indicatorCode];
    if (!indicator) return;
    
    const achievement = calculateAchievementPercentage(
        indicator.currentValue, 
        indicator.baseline, 
        indicator.target2030
    );
    
    const modalContent = `
        <div class="modal fade" id="indicatorDetailsModal" tabindex="-1">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="bi bi-info-circle"></i> تفاصيل المؤشر: ${indicator.nameAr}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <!-- Indicator Overview -->
                        <div class="row mb-4">
                            <div class="col-md-3">
                                <div class="card bg-light">
                                    <div class="card-body text-center">
                                        <h6>القيمة الحالية</h6>
                                        <h3 class="text-primary">${formatNumber(indicator.currentValue)}</h3>
                                        <small>${indicator.unit}</small>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-light">
                                    <div class="card-body text-center">
                                        <h6>المستهدف 2030</h6>
                                        <h3 class="text-success">${formatNumber(indicator.target2030)}</h3>
                                        <small>${indicator.unit}</small>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-light">
                                    <div class="card-body text-center">
                                        <h6>نسبة الإنجاز</h6>
                                        <h3 class="${achievement >= 80 ? 'text-success' : achievement >= 60 ? 'text-warning' : 'text-danger'}">${achievement}%</h3>
                                        <small>${getStatusText(achievement)}</small>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-light">
                                    <div class="card-body text-center">
                                        <h6>آخر تحديث</h6>
                                        <h5>${indicator.lastUpdate}</h5>
                                        <small>${indicator.frequency === 'quarterly' ? 'ربع سنوي' : 'سنوي'}</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Tabs for Different Views -->
                        <ul class="nav nav-tabs" role="tablist">
                            <li class="nav-item">
                                <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#overview-tab-content">
                                    <i class="bi bi-info-circle"></i> نظرة عامة
                                </button>
                            </li>
                            <li class="nav-item">
                                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#accountability-tab-content">
                                    <i class="bi bi-person-badge"></i> المسؤولية
                                </button>
                            </li>
                            <li class="nav-item">
                                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#contributions-tab-content">
                                    <i class="bi bi-diagram-2"></i> المساهمات
                                </button>
                            </li>
                            <li class="nav-item">
                                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#trends-tab-content">
                                    <i class="bi bi-graph-up"></i> الاتجاهات
                                </button>
                            </li>
                        </ul>
                        
                        <div class="tab-content mt-3">
                            <!-- Overview Tab -->
                            <div class="tab-pane fade show active" id="overview-tab-content">
                                <div class="row">
                                    <div class="col-md-6">
                                        <h6>معلومات المؤشر</h6>
                                        <table class="table table-sm">
                                            <tr>
                                                <td width="40%">الكود:</td>
                                                <td><span class="badge bg-primary">${indicator.code}</span></td>
                                            </tr>
                                            <tr>
                                                <td>الاسم بالإنجليزية:</td>
                                                <td>${indicator.nameEn}</td>
                                            </tr>
                                            <tr>
                                                <td>الفئة:</td>
                                                <td>${getCategoryName(indicator.category)}</td>
                                            </tr>
                                            <tr>
                                                <td>الوزن النسبي:</td>
                                                <td>${(indicator.weight * 100).toFixed(1)}%</td>
                                            </tr>
                                            <tr>
                                                <td>قيمة الأساس (${indicator.baselineYear}):</td>
                                                <td>${formatNumber(indicator.baseline)} ${indicator.unit}</td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="col-md-6">
                                        <h6>الوصف والمنهجية</h6>
                                        <p>${indicator.description || 'وصف تفصيلي للمؤشر وأهميته في تحقيق رؤية 2030'}</p>
                                        ${indicator.formula ? `<p><strong>المعادلة:</strong> <code>${indicator.formula}</code></p>` : ''}
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Accountability Tab -->
                            <div class="tab-pane fade" id="accountability-tab-content">
                                <div class="row">
                                    <div class="col-md-6">
                                        <h6>سلسلة المسؤولية</h6>
                                        <div class="timeline">
                                            <div class="timeline-item">
                                                <div class="timeline-marker bg-success"></div>
                                                <div class="timeline-content">
                                                    <strong>المسؤول الرئيسي</strong>
                                                    <p>${indicator.owner}</p>
                                                </div>
                                            </div>
                                            <div class="timeline-item">
                                                <div class="timeline-marker bg-primary"></div>
                                                <div class="timeline-content">
                                                    <strong>مصدر البيانات</strong>
                                                    <p>${indicator.source}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <h6>جودة البيانات</h6>
                                        <div class="progress mb-2" style="height: 25px;">
                                            <div class="progress-bar bg-success" style="width: ${indicator.dataQualityScore || 95}%">
                                                جودة البيانات: ${indicator.dataQualityScore || 95}%
                                            </div>
                                        </div>
                                        <small class="text-muted">
                                            ${indicator.verified ? '<i class="bi bi-check-circle-fill text-success"></i> البيانات مُحققة ومُعتمدة' : '<i class="bi bi-exclamation-triangle text-warning"></i> في انتظار التحقق'}
                                        </small>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Contributions Tab -->
                            <div class="tab-pane fade" id="contributions-tab-content">
                                <h6>البرامج والجهات المساهمة</h6>
                                <div class="alert alert-info">
                                    <i class="bi bi-info-circle"></i> 
                                    هذا المؤشر يتأثر بأداء ${Math.floor(Math.random() * 10) + 5} برامج و ${Math.floor(Math.random() * 8) + 3} جهات حكومية
                                </div>
                                <!-- Add contribution details here -->
                            </div>
                            
                            <!-- Trends Tab -->
                            <div class="tab-pane fade" id="trends-tab-content">
                                <canvas id="trendChart" height="100"></canvas>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="exportIndicatorReport('${indicator.code}')">
                            <i class="bi bi-download"></i> تصدير التقرير
                        </button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('indicatorDetailsModal');
    if (existingModal) {
        // Properly dispose of existing Bootstrap modal instance
        const existingModalInstance = bootstrap.Modal.getInstance(existingModal);
        if (existingModalInstance) {
            existingModalInstance.dispose();
        }
        existingModal.remove();
    }
    
    // Remove any lingering modal backdrops
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => backdrop.remove());
    
    // Remove modal-open class from body
    document.body.classList.remove('modal-open');
    document.body.style.removeProperty('overflow');
    document.body.style.removeProperty('padding-right');
    
    // Add modal to body
    document.body.insertAdjacentHTML('beforeend', modalContent);
    
    // Show modal with proper initialization
    const modalElement = document.getElementById('indicatorDetailsModal');
    const modal = new bootstrap.Modal(modalElement, {
        keyboard: true,
        backdrop: true,
        focus: true
    });
    
    // Clean up on modal hide
    modalElement.addEventListener('hidden.bs.modal', function () {
        // Destroy any charts in the modal
        if (chartInstances['trendChart']) {
            chartInstances['trendChart'].destroy();
            delete chartInstances['trendChart'];
        }
        
        // Dispose of the modal instance
        modal.dispose();
        
        // Remove the modal element
        modalElement.remove();
        
        // Clean up any remaining backdrops
        const backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(backdrop => backdrop.remove());
        
        // Reset body styles
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('overflow');
        document.body.style.removeProperty('padding-right');
    });
    
    modal.show();
    
    // Initialize chart after modal is shown
    modalElement.addEventListener('shown.bs.modal', function () {
        setTimeout(() => {
            if (document.getElementById('trendChart')) {
                createTrendChart('trendChart', indicator);
            }
        }, 100);
    }, { once: true });
}

function showAllIndicators() {
    // Switch to the matrix tab to show all 60 indicators
    switchToTab('matrix');
}

// Track how many indicators are currently displayed
let displayedIndicatorsCount = 10;
const totalIndicators = 60;

function loadMoreIndicators() {
    try {
        // Find the table body
        const tableBody = document.querySelector('#dashboard table tbody');
        if (!tableBody) {
            console.warn('Table body not found');
            return;
        }
        
        // Find the load more row
        const loadMoreRow = tableBody.querySelector('tr:last-child');
        if (!loadMoreRow) {
            console.warn('Load more row not found');
            return;
        }
        
        // Calculate how many more to load (load 10 more each time)
        const indicatorsToLoad = Math.min(10, totalIndicators - displayedIndicatorsCount);
        
        if (indicatorsToLoad <= 0) {
            // All indicators are loaded, hide the button
            loadMoreRow.style.display = 'none';
            return;
        }
        
        // Generate additional indicator rows
        // This is a simplified version - in production, you'd fetch from API
        const startIndex = displayedIndicatorsCount + 1;
        const endIndex = Math.min(startIndex + indicatorsToLoad - 1, totalIndicators);
        
        // Create rows for additional indicators
        let newRows = '';
        for (let i = startIndex; i <= endIndex; i++) {
            const indicatorCode = `NOI.${String(i).padStart(3, '0')}`;
            const indicator = noiMockData[indicatorCode] || {
                code: indicatorCode,
                nameAr: `مؤشر ${i}`,
                currentValue: Math.random() * 1000,
                target2030: 1000,
                baseline: 500,
                unit: 'مليار ريال',
                owner: 'وزارة',
                category: 'economic-production'
            };
            
            const achievement = calculateAchievementPercentage(
                indicator.currentValue,
                indicator.baseline,
                indicator.target2030
            );
            
            const statusClass = achievement >= 80 ? 'success' : achievement >= 60 ? 'warning' : 'danger';
            const statusText = achievement >= 80 ? 'متقدم' : achievement >= 60 ? 'على المسار' : 'يحتاج متابعة';
            
            newRows += `
                <tr>
                    <td>${i}</td>
                    <td><span class="badge bg-secondary">${indicatorCode}</span></td>
                    <td><strong>${indicator.nameAr}</strong></td>
                    <td><span class="badge bg-info">L2</span></td>
                    <td>${indicator.owner}</td>
                    <td class="fw-bold">${formatNumber(indicator.currentValue)}</td>
                    <td>${formatNumber(indicator.target2030)}</td>
                    <td>
                        <div class="progress" style="height: 20px;">
                            <div class="progress-bar bg-${statusClass}" style="width: ${achievement}%;">${achievement}%</div>
                        </div>
                    </td>
                    <td>
                        <button class="btn btn-sm btn-light" onclick="viewDetails('${indicatorCode}')">
                            <i class="bi bi-three-dots"></i>
                        </button>
                    </td>
                </tr>
            `;
        }
        
        // Insert new rows before the load more row
        loadMoreRow.insertAdjacentHTML('beforebegin', newRows);
        
        // Update displayed count
        displayedIndicatorsCount = endIndex;
        
        // Update the button text and count display
        const remaining = totalIndicators - displayedIndicatorsCount;
        const button = loadMoreRow.querySelector('button');
        const countDisplay = document.getElementById('indicatorsCount');
        
        if (button) {
            if (remaining > 0) {
                button.innerHTML = `<i class="bi bi-arrow-down-circle"></i> عرض المزيد من المؤشرات (${remaining} متبقي)`;
            } else {
                button.innerHTML = '<i class="bi bi-check-circle"></i> تم عرض جميع المؤشرات';
                button.disabled = true;
                button.classList.remove('btn-outline-primary');
                button.classList.add('btn-success');
            }
        }
        
        if (countDisplay) {
            countDisplay.textContent = `عرض ${displayedIndicatorsCount} من ${totalIndicators} مؤشر ناتج وطني`;
        }
        
        console.log(`Loaded ${indicatorsToLoad} more indicators. Total displayed: ${displayedIndicatorsCount}`);
        
    } catch (error) {
        console.error('Error loading more indicators:', error);
    }
}

// Custom tab switching function to avoid Bootstrap issues
function switchToTab(tabName) {
    try {
        // Hide all tab panes
        const allTabPanes = document.querySelectorAll('.tab-pane');
        allTabPanes.forEach(pane => {
            pane.classList.remove('show', 'active');
        });
        
        // Remove active class from all nav links and buttons
        const allNavLinks = document.querySelectorAll('[data-bs-toggle="pill"], .nav-link');
        allNavLinks.forEach(link => {
            link.classList.remove('active');
        });
        
        // Also remove active from service buttons
        const serviceButtons = document.querySelectorAll('.service-btn');
        serviceButtons.forEach(btn => {
            btn.classList.remove('active');
        });
        
        // Also remove active from secondary buttons
        const secondaryButtons = document.querySelectorAll('.btn-outline-secondary');
        secondaryButtons.forEach(btn => {
            btn.classList.remove('active');
        });
        
        // Show the selected tab
        const targetTab = document.getElementById(tabName);
        if (targetTab) {
            targetTab.classList.add('show', 'active');
            
            // Find and activate the corresponding nav button if it exists
            const navButton = document.querySelector(`[data-bs-target="#${tabName}"]`);
            if (navButton) {
                navButton.classList.add('active');
            }
            
            // Also activate service button if it exists
            const serviceButton = document.querySelector(`.service-btn[data-bs-target="#${tabName}"]`);
            if (serviceButton) {
                serviceButton.classList.add('active');
            }
            
            // Scroll to top of the tab content
            targetTab.scrollIntoView({ behavior: 'smooth', block: 'start' });
            
            // Initialize charts if switching to specific tabs
            if (tabName === 'analysis') {
                // Destroy existing charts first
                if (chartInstances.trendChart) {
                    chartInstances.trendChart.destroy();
                    chartInstances.trendChart = null;
                }
                if (chartInstances.decompositionChart) {
                    chartInstances.decompositionChart.destroy();
                    chartInstances.decompositionChart = null;
                }
                
                // Reinitialize after a short delay
                setTimeout(() => {
                    if (document.getElementById('trendChart')) {
                        initializeTrendChart();
                    }
                    if (document.getElementById('decompositionChart')) {
                        initializeDecompositionChart();
                    }
                }, 200);
            } else if (tabName === 'contributions') {
                if (chartInstances.contributionChart) {
                    chartInstances.contributionChart.destroy();
                    chartInstances.contributionChart = null;
                }
                
                setTimeout(() => {
                    if (document.getElementById('contributionChart')) {
                        initializeContributionChart();
                    }
                }, 200);
            }
            
            // Log successful tab switch
            console.log(`Switched to tab: ${tabName}`);
        } else {
            console.warn(`Tab not found: ${tabName}`);
        }
    } catch (error) {
        console.error('Error switching tabs:', error);
    }
}

function getCategoryName(category) {
    const categories = {
        'economic-production': 'الإنتاج الاقتصادي',
        'private-sector': 'القطاع الخاص',
        'trade-investment': 'التجارة والاستثمار',
        'employment': 'التوظيف',
        'innovation': 'الابتكار',
        'tourism': 'السياحة',
        'sustainability': 'الاستدامة'
    };
    return categories[category] || category;
}

function exportIndicatorReport(indicatorCode) {
    const indicator = noiMockData[indicatorCode];
    if (!indicator) return;
    
    // Show success message
    const alert = `
        <div class="alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" style="z-index: 9999;">
            <i class="bi bi-check-circle"></i> جاري إعداد التقرير...
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    document.body.insertAdjacentHTML('beforeend', alert);
    
    // Auto-dismiss after 3 seconds
    setTimeout(() => {
        const alertEl = document.querySelector('.alert');
        if (alertEl) alertEl.remove();
    }, 3000);
}

function createTrendChart(canvasId, indicator) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    
    // Destroy existing chart if it exists to prevent canvas reuse error
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        chartInstances[canvasId] = null;
    }
    
    // Prepare data
    const labels = indicator.historicalData ? indicator.historicalData.slice(-12).map(d => d.period) : [];
    const actualData = indicator.historicalData ? indicator.historicalData.slice(-12).map(d => d.actual) : [];
    const targetData = indicator.historicalData ? indicator.historicalData.slice(-12).map(d => d.target) : [];
    
    chartInstances[canvasId] = new Chart(ctx.getContext('2d'), {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'القيمة الفعلية',
                data: actualData,
                borderColor: '#1F6046',
                backgroundColor: 'rgba(31, 96, 70, 0.1)',
                tension: 0.4
            }, {
                label: 'المستهدف',
                data: targetData,
                borderColor: '#FFC107',
                backgroundColor: 'rgba(255, 193, 7, 0.1)',
                borderDash: [5, 5],
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        font: {
                            family: 'Cairo'
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: false,
                    ticks: {
                        font: {
                            family: 'Cairo'
                        }
                    }
                }
            }
        }
    });
}

