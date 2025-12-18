# Weight & Contribution Logic - Complete System Design

## 🎯 Overview
The Saudi Vision 2030 performance management system uses a **hierarchical cascade model** where lower-level indicators contribute to higher-level indicators through weighted contributions.

## 📊 The Mathematical Logic

### 1. **Two-Dimensional Weight System**

#### A. Horizontal Weights (NOI Level)
- **Purpose**: Determine the relative importance of each NOI in the overall Vision 2030 score
- **Formula**: 
  ```
  Vision_2030_Score = Σ(NOI_Achievement[i] × NOI_Weight[i])
  where Σ(NOI_Weight) = 100%
  ```
- **Example**:
  - NOI.051 (Real GDP) = 3.5% weight
  - NOI.052 (Non-oil GDP) = 2.8% weight
  - NOI.022 (Private Sector) = 2.1% weight
  - ... (57 other NOIs)
  - Total = 100%

#### B. Vertical Contributions (Program → NOI)
- **Purpose**: Show how Program KPIs contribute to NOI achievement
- **Formula**:
  ```
  NOI_Achievement = Σ(Program_Achievement[j] × Program_Contribution[j])
  where Σ(Program_Contribution) = 100% for each NOI
  ```
- **Example for NOI.051 (Real GDP)**:
  - Tourism Program (FSD) → 22% contribution
  - Industry Program (NIDLP) → 18% contribution
  - Transport Program (NTP) → 16% contribution
  - Housing Program (HSG) → 14% contribution
  - Human Capital (HCD) → 12% contribution
  - Other Programs → 18% contribution
  - Total = 100%

### 2. **Cascade Calculation Flow**

```
Step 1: Calculate KPI Achievement (Level 4)
├── KPI_Achievement = (Actual - Baseline) / (Target - Baseline) × 100

Step 2: Calculate Program Achievement (Level 3)
├── Program_Achievement = Σ(KPI_Achievement × KPI_Weight) / Σ(KPI_Weight)

Step 3: Calculate NOI Value (Level 2)
├── NOI_Value = Σ(Program_Achievement × Program_Contribution)

Step 4: Calculate Vision Score (Level 1)
└── Vision_Score = Σ(NOI_Value × NOI_Weight)
```

## 🔄 Real-World Example

### Calculating NOI.051 (Real GDP)

**Input Data:**
```javascript
// Program Achievements (from their KPIs)
Tourism (FSD): 78% achievement
Industry (NIDLP): 82% achievement  
Transport (NTP): 71% achievement
Housing (HSG): 85% achievement
Human Capital (HCD): 69% achievement
Others: 75% (estimated)

// Contribution Weights to NOI.051
Tourism: 22%
Industry: 18%
Transport: 16%
Housing: 14%
Human Capital: 12%
Others: 18%
```

**Calculation:**
```
NOI.051 Achievement = 
  (78% × 0.22) + // Tourism
  (82% × 0.18) + // Industry
  (71% × 0.16) + // Transport
  (85% × 0.14) + // Housing
  (69% × 0.12) + // Human Capital
  (75% × 0.18)   // Others
= 17.16% + 14.76% + 11.36% + 11.90% + 8.28% + 13.50%
= 76.96% achievement
```

**Vision 2030 Impact:**
```
If NOI.051 has 3.5% weight in Vision 2030:
Contribution to Vision = 76.96% × 3.5% = 2.69 points
```

## 🎛️ Weight Management Interface Logic

### 1. **Weight Validation Rules**
```javascript
function validateWeights(weights) {
    const total = sum(weights);
    
    if (total === 100) {
        return { valid: true, status: 'perfect' };
    } else if (total > 100) {
        return { 
            valid: false, 
            status: 'excess',
            message: `Total ${total}% exceeds 100% by ${total - 100}%`
        };
    } else {
        return { 
            valid: false, 
            status: 'deficit',
            message: `Total ${total}% is below 100% by ${100 - total}%`
        };
    }
}
```

### 2. **Auto-Normalization Algorithm**
```javascript
function normalizeWeights(weights) {
    const currentTotal = sum(weights);
    
    if (currentTotal === 0) {
        // Equal distribution if all zeros
        return weights.map(() => 100 / weights.length);
    }
    
    // Proportional adjustment
    return weights.map(w => (w / currentTotal) * 100);
}
```

### 3. **Template-Based Distribution**
```javascript
const weightTemplates = {
    economic: {
        // Emphasize economic indicators
        pattern: (indicator) => {
            if (indicator.pillar === 'ECON') return 2.5;
            if (indicator.pillar === 'SOC') return 1.0;
            return 1.5;
        }
    },
    social: {
        // Emphasize social indicators
        pattern: (indicator) => {
            if (indicator.pillar === 'SOC') return 2.5;
            if (indicator.pillar === 'ECON') return 1.0;
            return 1.5;
        }
    },
    balanced: {
        // Equal distribution
        pattern: () => 100 / 60 // 1.67% each
    }
};
```

## 🔍 Why This Logic?

### 1. **Transparency**
- Clear line of sight from operational KPIs to Vision 2030
- Every program knows its contribution
- Every entity understands its impact

### 2. **Flexibility**
- Weights can be adjusted based on priorities
- New programs can be added without breaking the system
- Seasonal adjustments possible (e.g., emphasize tourism in summer)

### 3. **Accountability**
- Each level is responsible for its indicators
- Clear cascade shows where improvements are needed
- Performance gaps are traceable to source

### 4. **Mathematical Integrity**
- All weights sum to 100% at each level
- No double counting
- Normalized calculations prevent inflation

## 📈 Impact Analysis

### Scenario 1: Improving a Program KPI
```
If Tourism KPI improves by 10%:
→ Tourism Program achievement increases by 10% × KPI_weight
→ NOI.051 increases by (10% × KPI_weight) × 22% (Tourism contribution)
→ Vision 2030 increases by above × 3.5% (NOI.051 weight)
```

### Scenario 2: Changing NOI Weights
```
If NOI.051 weight increases from 3.5% to 4.0%:
→ Other NOI weights must decrease by 0.5% total
→ Economic focus becomes stronger
→ Programs contributing to NOI.051 become more critical
```

## 🛠️ Implementation in Code

### Current Implementation
```javascript
// In national-output-indicators.js
function calculateNOIFromPrograms(noiId) {
    const mapping = contributionMatrix[noiId];
    let totalValue = 0;
    
    mapping.contributors.forEach(contrib => {
        const achievement = calculateProgramAchievement(contrib.programId);
        totalValue += achievement * contrib.weight;
    });
    
    return totalValue;
}

// Weight management
function initializeWeightsTab() {
    // Load all 60 NOI weights
    Object.keys(noiMockData).forEach(key => {
        originalWeights[key] = noiMockData[key].weight || 1.67;
        tempWeights[key] = noiMockData[key].weight || 1.67;
    });
    
    populateWeightsTable();
}
```

## 🎯 Best Practices

1. **Regular Review**: Weights should be reviewed quarterly
2. **Data-Driven Adjustment**: Use historical performance to guide weight changes
3. **Stakeholder Consensus**: Major weight changes need approval from all entities
4. **Documentation**: Every weight change should be documented with rationale
5. **Testing**: Simulate impact before applying weight changes

## 📊 Current System Status

- **60 NOIs** defined with default weights (1.67% each)
- **12 Major Programs** with contribution mappings
- **300+ Program KPIs** feeding into the system
- **Cascade calculation** implemented and functional
- **Weight management UI** operational with validation

## 🚀 Next Steps

1. **Fine-tune weights** based on actual Vision 2030 priorities
2. **Add more program mappings** for complete coverage
3. **Implement historical tracking** of weight changes
4. **Create simulation tools** for what-if analysis
5. **Build approval workflow** for weight modifications

