/**
 * Complete National Output Indicators (NOI) Data Structure
 * 60 indicators with Entity-Pillar-Indicator relationships
 */

const NOI_PILLARS = {
    'monetary': {
        nameAr: 'الاستقرار النقدي والاستدامة المالية',
        nameEn: 'Monetary Stability & Fiscal Sustainability',
        color: '#1F6046'
    },
    'economic': {
        nameAr: 'النمو الاقتصادي',
        nameEn: 'Economic Growth',
        color: '#2E7D32'
    },
    'quality': {
        nameAr: 'جودة الحياة',
        nameEn: 'Quality of Life',
        color: '#1976D2'
    }
};

const NOI_ENTITIES = {
    'MOF': { nameAr: 'وزارة المالية', nameEn: 'Ministry of Finance' },
    'FC': { nameAr: 'اللجنة المالية', nameEn: 'Finance Committee' },
    'MOJ': { nameAr: 'وزارة العدل', nameEn: 'Ministry of Justice' },
    'BPED': { nameAr: 'لجنة ميزان المدفوعات والتنويع الاقتصادي', nameEn: 'Balance of Payments and Economic Diversification Committee' },
    'LCGPA': { nameAr: 'هيئة المحتوى المحلي والمشتريات الحكومية', nameEn: 'Local Content & Government Procurement Authority' },
    'MISA': { nameAr: 'وزارة الاستثمار', nameEn: 'Ministry of Investments' },
    'MOI': { nameAr: 'وزارة الداخلية', nameEn: 'Ministry of Interior' },
    'PIF': { nameAr: 'صندوق الاستثمارات العامة', nameEn: 'Public Investment Fund' },
    'PPC': { nameAr: 'لجنة السياسة السكانية', nameEn: 'Population Policy Committee' },
    'RCRC': { nameAr: 'الشركة الملكية لمدينة الرياض', nameEn: 'Royal Commission for Riyadh City' },
    'RDIA': { nameAr: 'هيئة البحث والتطوير والابتكار', nameEn: 'RDI Authority' },
    'MEP': { nameAr: 'وزارة الاقتصاد والتخطيط', nameEn: 'Ministry of Economy and Planning' },
    'HVRP': { nameAr: 'برنامج الإسكان', nameEn: 'Housing Vision Realization Program' },
    'MOMRAH': { nameAr: 'وزارة الشؤون البلدية والقروية والإسكان', nameEn: 'Ministry of Municipal and Rural Affairs' },
    'MEWA': { nameAr: 'وزارة البيئة والمياه والزراعة', nameEn: 'Ministry of Environment, Water, and Agriculture' },
    'CUA': { nameAr: 'مجلس شؤون الجامعات', nameEn: 'Council of Universities Affairs' },
    'HCDP': { nameAr: 'برنامج تنمية القدرات البشرية', nameEn: 'Human Capital Development Program' },
    'MHRSD': { nameAr: 'وزارة الموارد البشرية والتنمية الاجتماعية', nameEn: 'Ministry of Human Resources and Social Development' },
    'ETEC': { nameAr: 'هيئة تقويم التعليم والتدريب', nameEn: 'Education & Training Evaluation Commission' },
    'MOE': { nameAr: 'وزارة التعليم', nameEn: 'Ministry of Education' },
    'MCIT': { nameAr: 'وزارة الاتصالات وتقنية المعلومات', nameEn: 'Ministry of Communication and Information Technology' },
    'HSTP': { nameAr: 'برنامج تحول القطاع الصحي', nameEn: 'Health Sector Transformation Program' },
    'MOH': { nameAr: 'وزارة الصحة', nameEn: 'Ministry of Health' },
    'MOT': { nameAr: 'وزارة السياحة', nameEn: 'Ministry of Tourism' },
    'MOTLS': { nameAr: 'وزارة النقل والخدمات اللوجستية', nameEn: 'Ministry of Transport and Logistic Services' },
    'NTP': { nameAr: 'برنامج التحول الوطني', nameEn: 'National Transformation Program' },
    'SFDA': { nameAr: 'الهيئة العامة للغذاء والدواء', nameEn: 'Saudi Food and Drug Authority' },
    'GASTAT': { nameAr: 'الهيئة العامة للإحصاء', nameEn: 'General Authority for Statistics' }
};

const COMPLETE_NOI_DATA = [
    // Ministry of Finance (5 indicators)
    {
        id: 'NOI.001',
        nameAr: 'الإيرادات الحكومية (الإجمالي)',
        nameEn: 'Govt Revenues (Total)',
        entity: 'MOF',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.025
    },
    {
        id: 'NOI.002',
        nameAr: 'الإيرادات الحكومية (غير النفطية)',
        nameEn: 'Govt Revenues (Non-Oil)',
        entity: 'MOF',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.020
    },
    {
        id: 'NOI.003',
        nameAr: 'المصروفات الحكومية',
        nameEn: 'Government Expenditures',
        entity: 'MOF',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.018
    },
    {
        id: 'NOI.004',
        nameAr: 'الدين الحكومي كنسبة من الناتج المحلي',
        nameEn: 'Govt. Debt as % of GDP',
        entity: 'MOF',
        pillar: 'monetary',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.015
    },
    {
        id: 'NOI.005',
        nameAr: 'الاحتياطيات (الحكومية)',
        nameEn: 'Reserves (Government)',
        entity: 'MOF',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.012
    },

    // Finance Committee (2 indicators)
    {
        id: 'NOI.006',
        nameAr: 'إجمالي الإنفاق العام',
        nameEn: 'Total Public Expenditure',
        entity: 'FC',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.015
    },
    {
        id: 'NOI.007',
        nameAr: 'إجمالي الدين العام كنسبة من الناتج المحلي',
        nameEn: 'Total Public Debt as % of GDP',
        entity: 'FC',
        pillar: 'monetary',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.012
    },

    // Ministry of Justice
    {
        id: 'NOI.008',
        nameAr: 'البيئة القضائية',
        nameEn: 'Judicial Environment',
        entity: 'MOJ',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.010
    },

    // BPED Committee
    {
        id: 'NOI.009',
        nameAr: 'رصيد الحساب الجاري غير النفطي',
        nameEn: 'Non-Oil Current Account Balance',
        entity: 'BPED',
        pillar: 'monetary',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.018
    },

    // Local Content & Government Procurement Authority
    {
        id: 'NOI.010',
        nameAr: 'حصة المحتوى المحلي',
        nameEn: 'Local Content Share',
        entity: 'LCGPA',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.015
    },

    // Ministry of Investments (2 indicators)
    {
        id: 'NOI.011',
        nameAr: 'مؤشر ثقة المستثمر',
        nameEn: 'Investor Confidence Index',
        entity: 'MISA',
        pillar: 'economic',
        unit: 'نقطة',
        unitEn: 'Points',
        frequency: 'quarterly',
        weight: 0.020
    },
    {
        id: 'NOI.012',
        nameAr: 'البيئة التشريعية',
        nameEn: 'Legislative Environment',
        entity: 'MISA',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.008
    },

    // Ministry of Interior
    {
        id: 'NOI.013',
        nameAr: 'مؤشر السلامة العامة',
        nameEn: 'Public Safety Index',
        entity: 'MOI',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.012
    },

    // Public Investment Fund
    {
        id: 'NOI.014',
        nameAr: 'أصول صندوق الاستثمارات العامة',
        nameEn: 'PIF Assets',
        entity: 'PIF',
        pillar: 'monetary',
        unit: 'تريليون ريال',
        unitEn: 'Trillion SAR',
        frequency: 'annual',
        weight: 0.025
    },

    // Population Policy Committee
    {
        id: 'NOI.015',
        nameAr: 'السكان',
        nameEn: 'Population',
        entity: 'PPC',
        pillar: 'economic',
        unit: 'مليون',
        unitEn: 'Million',
        frequency: 'annual',
        weight: 0.010
    },

    // RCRC and Development Authorities
    {
        id: 'NOI.016',
        nameAr: 'قابلية العيش في المدن السعودية',
        nameEn: 'Livability of Saudi Cities',
        entity: 'RCRC',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.015
    },

    // RDI Authority
    {
        id: 'NOI.017',
        nameAr: 'الإنفاق على البحث والتطوير والابتكار',
        nameEn: 'Research, Development, & Innovation (RDI) Spending',
        entity: 'RDIA',
        pillar: 'economic',
        unit: '% من الناتج المحلي',
        unitEn: '% of GDP',
        frequency: 'annual',
        weight: 0.018
    },

    // Ministry of Economy and Planning (9 indicators)
    {
        id: 'NOI.018',
        nameAr: 'التضخم',
        nameEn: 'Inflation',
        entity: 'MEP',
        pillar: 'monetary',
        unit: '%',
        unitEn: '%',
        frequency: 'monthly',
        weight: 0.012
    },
    {
        id: 'NOI.019',
        nameAr: 'إنتاجية العمل',
        nameEn: 'Labor Productivity',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'ريال/ساعة',
        unitEn: 'SAR/hour',
        frequency: 'annual',
        weight: 0.015
    },
    {
        id: 'NOI.020',
        nameAr: 'إنتاجية رأس المال',
        nameEn: 'Capital Productivity',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'نسبة',
        unitEn: 'Ratio',
        frequency: 'annual',
        weight: 0.012
    },
    {
        id: 'NOI.021',
        nameAr: 'التعقيد الاقتصادي',
        nameEn: 'Economic Complexity',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.010
    },
    {
        id: 'NOI.022',
        nameAr: 'مساهمة القطاع الخاص كنسبة من الناتج المحلي الحقيقي',
        nameEn: 'Private Sector Contribution as % of Real GDP',
        entity: 'MEP',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.022
    },
    {
        id: 'NOI.023',
        nameAr: 'الدخل السعودي',
        nameEn: 'Saudi Income',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'ريال',
        unitEn: 'SAR',
        frequency: 'annual',
        weight: 0.015
    },
    {
        id: 'NOI.024',
        nameAr: 'معدل الفقر',
        nameEn: 'Poverty Rate',
        entity: 'MEP',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.012
    },
    {
        id: 'NOI.025',
        nameAr: 'التحويلات المالية',
        nameEn: 'Remittances',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.008
    },
    {
        id: 'NOI.026',
        nameAr: 'الناتج المحلي غير النفطي للفرد',
        nameEn: 'Non-oil GDP per capita',
        entity: 'MEP',
        pillar: 'economic',
        unit: 'ألف ريال',
        unitEn: 'Thousand SAR',
        frequency: 'annual',
        weight: 0.018
    },

    // Housing Vision Realization Program
    {
        id: 'NOI.027',
        nameAr: 'تملك السعوديين للمساكن',
        nameEn: 'Saudi House Ownership',
        entity: 'HVRP',
        pillar: 'quality',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.015
    },

    // Ministry of Municipal and Rural Affairs (2 indicators)
    {
        id: 'NOI.028',
        nameAr: 'رضا المواطنين عن الخدمات البلدية',
        nameEn: 'Citizen Satisfaction with Municipal Services',
        entity: 'MOMRAH',
        pillar: 'quality',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.010
    },
    {
        id: 'NOI.029',
        nameAr: 'مؤشر التلوث البصري',
        nameEn: 'Visual Pollution Index',
        entity: 'MOMRAH',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.008
    },

    // Ministry of Environment, Water, and Agriculture
    {
        id: 'NOI.030',
        nameAr: 'مؤشر الأداء البيئي',
        nameEn: 'Environmental Performance Index (EPI)',
        entity: 'MEWA',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.012
    },

    // Council of Universities Affairs
    {
        id: 'NOI.031',
        nameAr: 'الجامعات السعودية في أفضل 200',
        nameEn: 'KSA Universities in Top 200',
        entity: 'CUA',
        pillar: 'quality',
        unit: 'عدد',
        unitEn: 'Count',
        frequency: 'annual',
        weight: 0.010
    },

    // Human Capital Development Program (3 indicators)
    {
        id: 'NOI.032',
        nameAr: 'مؤشر التنمية البشرية',
        nameEn: 'Human Development Index',
        entity: 'HCDP',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.015
    },
    {
        id: 'NOI.033',
        nameAr: 'توظيف خريجي التعليم العالي',
        nameEn: 'Higher Education Graduate Employment',
        entity: 'HCDP',
        pillar: 'quality',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.012
    },
    {
        id: 'NOI.034',
        nameAr: 'توظيف خريجي التدريب التقني والمهني',
        nameEn: 'TVET Graduate Employment',
        entity: 'HCDP',
        pillar: 'quality',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.010
    },

    // Ministry of Human Resources and Social Development (4 indicators)
    {
        id: 'NOI.035',
        nameAr: 'حصة القطاع الخاص في التوظيف السعودي',
        nameEn: 'Share of Private Sector in Saudi Employment',
        entity: 'MHRSD',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.018
    },
    {
        id: 'NOI.036',
        nameAr: 'معدل المشاركة في القوى العاملة',
        nameEn: 'Labor Force Participation Rate',
        entity: 'MHRSD',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.015
    },
    {
        id: 'NOI.037',
        nameAr: 'البطالة السعودية',
        nameEn: 'Saudi Unemployment',
        entity: 'MHRSD',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.020
    },
    {
        id: 'NOI.038',
        nameAr: 'إجمالي البطالة',
        nameEn: 'Total Unemployment',
        entity: 'MHRSD',
        pillar: 'economic',
        unit: '%',
        unitEn: '%',
        frequency: 'quarterly',
        weight: 0.015
    },

    // ETEC
    {
        id: 'NOI.039',
        nameAr: 'مؤشر أداء النظام التعليمي',
        nameEn: 'Education Ecosystem Performance Index',
        entity: 'ETEC',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.012
    },

    // Ministry of Education
    {
        id: 'NOI.040',
        nameAr: 'أداء الطلاب في اختبار PISA',
        nameEn: 'Student Performance in PISA',
        entity: 'MOE',
        pillar: 'quality',
        unit: 'نقطة',
        unitEn: 'Points',
        frequency: 'triennial',
        weight: 0.015
    },

    // Ministry of Communication and Information Technology
    {
        id: 'NOI.041',
        nameAr: 'مؤشر الاقتصاد الرقمي - الترتيب',
        nameEn: 'Digital Eco. Compo. Index - Rank',
        entity: 'MCIT',
        pillar: 'quality',
        unit: 'ترتيب',
        unitEn: 'Rank',
        frequency: 'annual',
        weight: 0.012
    },

    // Health Sector Transformation Program
    {
        id: 'NOI.042',
        nameAr: 'معدل وفيات الطرق',
        nameEn: 'Road Mortality',
        entity: 'HSTP',
        pillar: 'quality',
        unit: 'لكل 100 ألف',
        unitEn: 'per 100k',
        frequency: 'annual',
        weight: 0.010
    },

    // Ministry of Health (4 indicators)
    {
        id: 'NOI.043',
        nameAr: 'مؤشر جودة الصحة',
        nameEn: 'Health Quality Index',
        entity: 'MOH',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.015
    },
    {
        id: 'NOI.044',
        nameAr: 'متوسط العمر المتوقع',
        nameEn: 'Average Life Expectancy (ALE)',
        entity: 'MOH',
        pillar: 'quality',
        unit: 'سنة',
        unitEn: 'Years',
        frequency: 'annual',
        weight: 0.012
    },
    {
        id: 'NOI.045',
        nameAr: 'معدل وفيات الأمراض غير المعدية',
        nameEn: 'Non-Communicable Diseases (NCD) Mortality Rate',
        entity: 'MOH',
        pillar: 'quality',
        unit: 'لكل 100 ألف',
        unitEn: 'per 100k',
        frequency: 'annual',
        weight: 0.010
    },
    {
        id: 'NOI.046',
        nameAr: 'نسبة المجتمعات التي لديها خدمات رعاية صحية أولية',
        nameEn: '% of Communities with Primary Health Services',
        entity: 'MOH',
        pillar: 'quality',
        unit: '%',
        unitEn: '%',
        frequency: 'annual',
        weight: 0.008
    },

    // Ministry of Tourism
    {
        id: 'NOI.047',
        nameAr: 'إجمالي الزيارات السياحية',
        nameEn: 'Total Overnight Visits',
        entity: 'MOT',
        pillar: 'quality',
        unit: 'مليون',
        unitEn: 'Million',
        frequency: 'quarterly',
        weight: 0.015
    },

    // Ministry of Transport and Logistic Services
    {
        id: 'NOI.048',
        nameAr: 'مؤشر الأداء اللوجستي',
        nameEn: 'Logistics Performance Index',
        entity: 'MOTLS',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.012
    },

    // National Transformation Program
    {
        id: 'NOI.049',
        nameAr: 'مؤشر فعالية الحكومة',
        nameEn: 'Government Effectiveness Index (GEI)',
        entity: 'NTP',
        pillar: 'quality',
        unit: 'مؤشر',
        unitEn: 'Index',
        frequency: 'annual',
        weight: 0.015
    },

    // Saudi Food and Drug Authority
    {
        id: 'NOI.050',
        nameAr: 'تفشي الأمراض المنقولة بالغذاء',
        nameEn: 'Foodborne Outbreaks',
        entity: 'SFDA',
        pillar: 'quality',
        unit: 'عدد',
        unitEn: 'Count',
        frequency: 'annual',
        weight: 0.005
    },

    // Economic Growth Indicators (10 indicators - likely GASTAT)
    {
        id: 'NOI.051',
        nameAr: 'الناتج المحلي الإجمالي الحقيقي',
        nameEn: 'Real GDP (Total)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'تريليون ريال',
        unitEn: 'Trillion SAR',
        frequency: 'quarterly',
        weight: 0.030
    },
    {
        id: 'NOI.052',
        nameAr: 'الناتج المحلي الإجمالي الحقيقي (غير النفطي)',
        nameEn: 'Real GDP (Non-Oil)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'تريليون ريال',
        unitEn: 'Trillion SAR',
        frequency: 'quarterly',
        weight: 0.025
    },
    {
        id: 'NOI.053',
        nameAr: 'الاستهلاك الخاص',
        nameEn: 'Private Consumption',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.015
    },
    {
        id: 'NOI.054',
        nameAr: 'الاستهلاك الحكومي',
        nameEn: 'Government Consumption',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.012
    },
    {
        id: 'NOI.055',
        nameAr: 'الاستثمارات',
        nameEn: 'Investments',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'quarterly',
        weight: 0.020
    },
    {
        id: 'NOI.056',
        nameAr: 'الميزان التجاري (الإجمالي)',
        nameEn: 'Balance of Trade (Total)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.015
    },
    {
        id: 'NOI.057',
        nameAr: 'الميزان التجاري (غير النفطي)',
        nameEn: 'Balance of Trade (Non-oil)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.012
    },
    {
        id: 'NOI.058',
        nameAr: 'الواردات (الإجمالي)',
        nameEn: 'Imports (Total)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.010
    },
    {
        id: 'NOI.059',
        nameAr: 'الصادرات (الإجمالي)',
        nameEn: 'Exports (Total)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.012
    },
    {
        id: 'NOI.060',
        nameAr: 'الصادرات (غير النفطية)',
        nameEn: 'Exports (Non-oil)',
        entity: 'GASTAT',
        pillar: 'economic',
        unit: 'مليار ريال',
        unitEn: 'Billion SAR',
        frequency: 'monthly',
        weight: 0.018
    }
];

// Function to get indicators by entity
function getIndicatorsByEntity(entityCode) {
    return COMPLETE_NOI_DATA.filter(noi => noi.entity === entityCode);
}

// Function to get indicators by pillar
function getIndicatorsByPillar(pillarCode) {
    return COMPLETE_NOI_DATA.filter(noi => noi.pillar === pillarCode);
}

// Function to calculate pillar totals
function calculatePillarWeights() {
    const pillarWeights = {
        monetary: 0,
        economic: 0,
        quality: 0
    };
    
    COMPLETE_NOI_DATA.forEach(noi => {
        pillarWeights[noi.pillar] += noi.weight;
    });
    
    return pillarWeights;
}

// Function to validate total weights equal 1
function validateTotalWeights() {
    const total = COMPLETE_NOI_DATA.reduce((sum, noi) => sum + noi.weight, 0);
    return Math.abs(total - 1.0) < 0.001; // Allow small rounding error
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        NOI_PILLARS,
        NOI_ENTITIES,
        COMPLETE_NOI_DATA,
        getIndicatorsByEntity,
        getIndicatorsByPillar,
        calculatePillarWeights,
        validateTotalWeights
    };
}

// Export to window for browser use
if (typeof window !== 'undefined') {
    window.CompleteNOIData = {
        NOI_PILLARS,
        NOI_ENTITIES,
        COMPLETE_NOI_DATA,
        getIndicatorsByEntity,
        getIndicatorsByPillar,
        calculatePillarWeights,
        validateTotalWeights
    };
}

