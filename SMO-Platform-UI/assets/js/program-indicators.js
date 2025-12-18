/**
 * Program Indicators (Level 3) Management System
 * This module manages the relationship between Program KPIs and NOIs
 */

// ==========================================
// Program KPIs Data Structure (Level 3)
// ==========================================

const programKPIs = {
    // National Transformation Program
    'NTP': {
        code: '1-18',
        nameAr: 'برنامج التحول الوطني',
        nameEn: 'National Transformation Program',
        kpis: [
            {
                id: 'NTP.KPI.001',
                nameAr: 'نسبة الخدمات الحكومية المحولة رقمياً',
                nameEn: 'Digital Government Services Rate',
                unit: '%',
                baseline: 45,
                target2030: 95,
                currentValue: 78,
                weight: 0.35,
                frequency: 'quarterly'
            },
            {
                id: 'NTP.KPI.002',
                nameAr: 'مؤشر كفاءة الإنفاق الحكومي',
                nameEn: 'Government Efficiency Index',
                unit: 'نقطة',
                baseline: 60,
                target2030: 90,
                currentValue: 75,
                weight: 0.30,
                frequency: 'annual'
            },
            {
                id: 'NTP.KPI.003',
                nameAr: 'معدل رضا المستفيدين',
                nameEn: 'Beneficiary Satisfaction Rate',
                unit: '%',
                baseline: 65,
                target2030: 90,
                currentValue: 82,
                weight: 0.35,
                frequency: 'quarterly'
            }
        ],
        overallAchievement: null, // Will be calculated
        contributes_to: [
            { noi: 'NOI.051', weight: 0.16 }, // Real GDP
            { noi: 'NOI.055', weight: 0.10 }, // Investments
            { noi: 'NOI.041', weight: 0.25 }  // Digital Economy
        ]
    },
    
    // Financial Sector Development Program
    'FSD': {
        code: '1-19',
        nameAr: 'برنامج تطوير القطاع المالي',
        nameEn: 'Financial Sector Development Program',
        kpis: [
            {
                id: 'FSD.KPI.001',
                nameAr: 'حجم الأصول المصرفية',
                nameEn: 'Banking Assets Volume',
                unit: 'تريليون ريال',
                baseline: 2.5,
                target2030: 5.0,
                currentValue: 3.8,
                weight: 0.40,
                frequency: 'quarterly'
            },
            {
                id: 'FSD.KPI.002',
                nameAr: 'نسبة الشمول المالي',
                nameEn: 'Financial Inclusion Rate',
                unit: '%',
                baseline: 70,
                target2030: 95,
                currentValue: 86,
                weight: 0.30,
                frequency: 'annual'
            },
            {
                id: 'FSD.KPI.003',
                nameAr: 'حجم سوق رأس المال',
                nameEn: 'Capital Market Size',
                unit: 'تريليون ريال',
                baseline: 8.0,
                target2030: 15.0,
                currentValue: 11.2,
                weight: 0.30,
                frequency: 'quarterly'
            }
        ],
        overallAchievement: null,
        contributes_to: [
            { noi: 'NOI.001', weight: 0.22 }, // GDP
            { noi: 'NOI.003', weight: 0.30 }, // Private Sector
            { noi: 'NOI.006', weight: 0.25 }  // FDI
        ]
    },
    
    // Housing Program
    'HSG': {
        code: '1-20',
        nameAr: 'برنامج الإسكان',
        nameEn: 'Housing Program',
        kpis: [
            {
                id: 'HSG.KPI.001',
                nameAr: 'نسبة التملك السكني',
                nameEn: 'Home Ownership Rate',
                unit: '%',
                baseline: 47,
                target2030: 70,
                currentValue: 62,
                weight: 0.50,
                frequency: 'annual'
            },
            {
                id: 'HSG.KPI.002',
                nameAr: 'عدد الوحدات السكنية المنجزة',
                nameEn: 'Completed Housing Units',
                unit: 'ألف وحدة',
                baseline: 50,
                target2030: 300,
                currentValue: 185,
                weight: 0.30,
                frequency: 'quarterly'
            },
            {
                id: 'HSG.KPI.003',
                nameAr: 'معدل القدرة على تحمل التكاليف',
                nameEn: 'Affordability Index',
                unit: 'نقطة',
                baseline: 45,
                target2030: 75,
                currentValue: 58,
                weight: 0.20,
                frequency: 'annual'
            }
        ],
        overallAchievement: null,
        contributes_to: [
            { noi: 'NOI.001', weight: 0.14 }, // GDP
            { noi: 'NOI.007', weight: 0.35 }, // Quality of Life
            { noi: 'NOI.003', weight: 0.15 }  // Private Sector
        ]
    },
    
    // Human Capability Development Program
    'HCD': {
        code: '1-21',
        nameAr: 'برنامج تنمية القدرات البشرية',
        nameEn: 'Human Capability Development Program',
        kpis: [
            {
                id: 'HCD.KPI.001',
                nameAr: 'معدل محو الأمية',
                nameEn: 'Literacy Rate',
                unit: '%',
                baseline: 94.5,
                target2030: 99,
                currentValue: 97.2,
                weight: 0.25,
                frequency: 'annual'
            },
            {
                id: 'HCD.KPI.002',
                nameAr: 'نسبة المهارات المتقدمة',
                nameEn: 'Advanced Skills Rate',
                unit: '%',
                baseline: 35,
                target2030: 65,
                currentValue: 48,
                weight: 0.40,
                frequency: 'annual'
            },
            {
                id: 'HCD.KPI.003',
                nameAr: 'معدل التوظيف للخريجين',
                nameEn: 'Graduate Employment Rate',
                unit: '%',
                baseline: 60,
                target2030: 85,
                currentValue: 72,
                weight: 0.35,
                frequency: 'quarterly'
            }
        ],
        overallAchievement: null,
        contributes_to: [
            { noi: 'NOI.001', weight: 0.12 }, // GDP
            { noi: 'NOI.005', weight: 0.40 }, // Women Employment
            { noi: 'NOI.008', weight: 0.30 }  // Unemployment Rate
        ]
    },
    
    // National Industrial Development Program
    'NIDLP': {
        code: '1-22',
        nameAr: 'برنامج تطوير الصناعة الوطنية والخدمات اللوجستية',
        nameEn: 'National Industrial Development & Logistics Program',
        kpis: [
            {
                id: 'NIDLP.KPI.001',
                nameAr: 'حجم الصادرات الصناعية',
                nameEn: 'Industrial Exports Volume',
                unit: 'مليار ريال',
                baseline: 180,
                target2030: 450,
                currentValue: 285,
                weight: 0.35,
                frequency: 'quarterly'
            },
            {
                id: 'NIDLP.KPI.002',
                nameAr: 'مؤشر الأداء اللوجستي',
                nameEn: 'Logistics Performance Index',
                unit: 'نقطة',
                baseline: 3.2,
                target2030: 4.5,
                currentValue: 3.8,
                weight: 0.30,
                frequency: 'annual'
            },
            {
                id: 'NIDLP.KPI.003',
                nameAr: 'نسبة التوطين الصناعي',
                nameEn: 'Industrial Localization Rate',
                unit: '%',
                baseline: 45,
                target2030: 75,
                currentValue: 58,
                weight: 0.35,
                frequency: 'annual'
            }
        ],
        overallAchievement: null,
        contributes_to: [
            { noi: 'NOI.001', weight: 0.18 }, // GDP
            { noi: 'NOI.004', weight: 0.45 }, // Non-oil Exports
            { noi: 'NOI.002', weight: 0.25 }  // Non-oil GDP
        ]
    }
};

// ==========================================
// Contribution Mapping System
// ==========================================

const contributionMatrix = {
    // Maps NOIs to their contributing programs with weights - Updated for 60 NOI system
    'NOI.051': { // Real GDP (Total)
        nameAr: 'الناتج المحلي الإجمالي الحقيقي',
        contributors: [
            { programId: 'FSD', weight: 0.22 },
            { programId: 'NIDLP', weight: 0.18 },
            { programId: 'NTP', weight: 0.16 },
            { programId: 'HSG', weight: 0.14 },
            { programId: 'HCD', weight: 0.12 },
            { programId: 'OTHER', weight: 0.18 }
        ]
    },
    'NOI.052': { // Real GDP (Non-oil)
        nameAr: 'الناتج المحلي الإجمالي الحقيقي (غير النفطي)',
        contributors: [
            { programId: 'NIDLP', weight: 0.25 },
            { programId: 'FSD', weight: 0.20 },
            { programId: 'NTP', weight: 0.15 },
            { programId: 'HSG', weight: 0.10 },
            { programId: 'HCD', weight: 0.10 },
            { programId: 'OTHER', weight: 0.20 }
        ]
    },
    'NOI.003': { // Private Sector Contribution
        nameAr: 'مساهمة القطاع الخاص',
        contributors: [
            { programId: 'FSD', weight: 0.30 },
            { programId: 'NIDLP', weight: 0.20 },
            { programId: 'HSG', weight: 0.15 },
            { programId: 'NTP', weight: 0.10 },
            { programId: 'OTHER', weight: 0.25 }
        ]
    },
    'NOI.004': { // Non-oil Exports
        nameAr: 'الصادرات غير النفطية',
        contributors: [
            { programId: 'NIDLP', weight: 0.45 },
            { programId: 'FSD', weight: 0.15 },
            { programId: 'NTP', weight: 0.10 },
            { programId: 'OTHER', weight: 0.30 }
        ]
    },
    'NOI.005': { // Women Employment
        nameAr: 'نسبة النساء في سوق العمل',
        contributors: [
            { programId: 'HCD', weight: 0.40 },
            { programId: 'NTP', weight: 0.20 },
            { programId: 'FSD', weight: 0.15 },
            { programId: 'OTHER', weight: 0.25 }
        ]
    },
    'NOI.006': { // FDI
        nameAr: 'الاستثمار الأجنبي المباشر',
        contributors: [
            { programId: 'FSD', weight: 0.25 },
            { programId: 'NIDLP', weight: 0.20 },
            { programId: 'NTP', weight: 0.10 },
            { programId: 'OTHER', weight: 0.45 }
        ]
    }
};

// ==========================================
// Calculation Functions
// ==========================================

/**
 * Calculate program achievement based on its KPIs
 */
function calculateProgramAchievement(programId) {
    const program = programKPIs[programId];
    if (!program) return 0;
    
    let totalAchievement = 0;
    let totalWeight = 0;
    
    program.kpis.forEach(kpi => {
        const achievement = calculateKPIAchievement(kpi);
        totalAchievement += achievement * kpi.weight;
        totalWeight += kpi.weight;
    });
    
    // Store calculated achievement
    program.overallAchievement = totalWeight > 0 ? totalAchievement / totalWeight : 0;
    return program.overallAchievement;
}

/**
 * Calculate individual KPI achievement
 */
function calculateKPIAchievement(kpi) {
    if (!kpi.currentValue || !kpi.baseline || !kpi.target2030) return 0;
    
    const progress = kpi.currentValue - kpi.baseline;
    const totalRequired = kpi.target2030 - kpi.baseline;
    
    if (totalRequired === 0) return 100;
    
    const achievement = (progress / totalRequired) * 100;
    return Math.max(0, Math.min(100, achievement)); // Cap between 0-100
}

/**
 * Calculate NOI value from program contributions
 */
function calculateNOIFromPrograms(noiId) {
    const noiMapping = contributionMatrix[noiId];
    if (!noiMapping) return null;
    
    let totalContribution = 0;
    
    noiMapping.contributors.forEach(contributor => {
        if (contributor.programId === 'OTHER') {
            // Handle other programs (can be average or fixed value)
            totalContribution += 75 * contributor.weight; // Assume 75% for others
        } else {
            const programAchievement = calculateProgramAchievement(contributor.programId);
            totalContribution += programAchievement * contributor.weight;
        }
    });
    
    return totalContribution;
}

/**
 * Get all programs contributing to a specific NOI
 */
function getNOIContributors(noiId) {
    const noiMapping = contributionMatrix[noiId];
    if (!noiMapping) return [];
    
    return noiMapping.contributors.map(contrib => {
        const program = programKPIs[contrib.programId];
        if (!program) {
            return {
                programId: contrib.programId,
                programName: 'برامج أخرى',
                weight: contrib.weight,
                achievement: 75, // Default for others
                contribution: 75 * contrib.weight
            };
        }
        
        const achievement = calculateProgramAchievement(contrib.programId);
        return {
            programId: contrib.programId,
            programName: program.nameAr,
            programCode: program.code,
            weight: contrib.weight,
            achievement: achievement,
            contribution: achievement * contrib.weight,
            kpis: program.kpis
        };
    });
}

/**
 * Calculate cascade from bottom to top
 */
function calculateFullCascade() {
    const cascade = {};
    
    // Calculate all program achievements first
    Object.keys(programKPIs).forEach(programId => {
        calculateProgramAchievement(programId);
    });
    
    // Then calculate NOI values
    Object.keys(contributionMatrix).forEach(noiId => {
        cascade[noiId] = {
            calculatedValue: calculateNOIFromPrograms(noiId),
            contributors: getNOIContributors(noiId)
        };
    });
    
    return cascade;
}

/**
 * Distribute NOI target to programs
 */
function cascadeNOITarget(noiId, targetValue) {
    const noiMapping = contributionMatrix[noiId];
    if (!noiMapping) return [];
    
    const programTargets = [];
    
    noiMapping.contributors.forEach(contrib => {
        const program = programKPIs[contrib.programId];
        if (program) {
            programTargets.push({
                programId: contrib.programId,
                programName: program.nameAr,
                allocatedTarget: targetValue * contrib.weight,
                weight: contrib.weight
            });
        }
    });
    
    return programTargets;
}

// ==========================================
// Export Functions
// ==========================================

if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        programKPIs,
        contributionMatrix,
        calculateProgramAchievement,
        calculateNOIFromPrograms,
        getNOIContributors,
        calculateFullCascade,
        cascadeNOITarget
    };
}

// Export to window for browser use
if (typeof window !== 'undefined') {
    window.ProgramIndicators = {
        programKPIs,
        contributionMatrix,
        calculateProgramAchievement,
        calculateNOIFromPrograms,
        getNOIContributors,
        calculateFullCascade,
        cascadeNOITarget
    };
}
