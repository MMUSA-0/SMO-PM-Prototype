/**
 * NOI Tabs Navigation Fix
 * Enables hidden tabs and connects sub-navigation
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('🔧 NOI Tabs Fix initialized');
    
    // Step 1: Remove display:none from all hidden tabs
    const hiddenTabIds = [
        'entities', 'composite', 'rollup', 
        'integration', 'standards', 'analysis', 
        'contributions', 'categories'
    ];
    
    hiddenTabIds.forEach(tabId => {
        const tab = document.getElementById(tabId);
        if (tab) {
            // Remove inline style that hides it
            tab.style.removeProperty('display');
            console.log(`✓ Enabled tab: ${tabId}`);
        } else {
            console.warn(`⚠ Tab not found: ${tabId}`);
        }
    });
    
    // Step 2: Fix sub-navigation buttons to properly target tabs
    // These buttons were pointing to hidden content
    const buttonMappings = {
        // Note: eor-tree-sub-tab already points to correct #tree in HTML, no need to remap
        'eor-cards-sub-tab': 'entities',
        'impact-composite-sub-tab': 'composite',
        'impact-rollup-sub-tab': 'rollup',
        'impact-integration-sub-tab': 'integration',
        'bench-standards-sub-tab': 'standards',
        'monitoring-analysis-sub-tab': 'analysis',
        'settings-contrib-sub-tab': 'contributions',
        'settings-categories-sub-tab': 'categories'
    };
    
    Object.entries(buttonMappings).forEach(([buttonId, tabId]) => {
        const button = document.getElementById(buttonId);
        if (button) {
            // Update the data-bs-target to point to correct tab
            button.setAttribute('data-bs-target', `#${tabId}`);
            console.log(`✓ Connected button ${buttonId} → #${tabId}`);
        }
    });
    
    // Step 3: Add event listeners for debugging
    document.querySelectorAll('[data-bs-toggle="pill"]').forEach(button => {
        button.addEventListener('shown.bs.tab', function (e) {
            console.log('📍 Tab activated:', e.target.getAttribute('data-bs-target'));
        });
    });
    
    console.log('✅ NOI Tabs Fix completed successfully');
});

