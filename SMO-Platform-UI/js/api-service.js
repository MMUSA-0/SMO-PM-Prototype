/**
 * API Service for SMO Platform
 * This file handles all backend API connections
 */

// API Configuration
const API_CONFIG = {
    // Change this to your actual backend URL in production
    BASE_URL: window.location.hostname === 'localhost' 
        ? 'https://localhost:7001/api' // Development API URL
        : '/api', // Production API URL (relative path)
    
    // API endpoints - UPDATED to match actual backend controllers
    ENDPOINTS: {
        // Authentication
        LOGIN: '/auth/login',
        LOGOUT: '/auth/logout',
        REFRESH_TOKEN: '/auth/refresh',
        
        // Core Entities - Using EXISTING controllers
        VISION: '/vision',                    // VisionController - but using VisionProgram entity
        PROGRAMS: '/program',                  // ProgramController - VisionProgram entity
        PROGRAM_DETAIL: '/program/{id}',
        PROGRAM_INITIATIVES: '/program/{id}/initiatives',
        PROGRAM_KPIS: '/program/{id}/kpis',
        
        INITIATIVES: '/initiative',            // InitiativeController (to be created)
        INITIATIVE_DETAIL: '/initiative/{id}',
        INITIATIVE_MILESTONES: '/initiative/{id}/milestones',
        INITIATIVE_KPIS: '/initiative/{id}/kpis',
        
        KPIS: '/kpi',                         // KPIController (to be created)
        KPI_DETAIL: '/kpi/{id}',
        KPI_VALUES: '/kpi/{id}/values',
        KPI_TARGETS: '/kpi/{id}/targets',
        
        // Performance Management
        PERFORMANCE_DASHBOARD: '/dashboard',   // To be created
        APPROVALS: '/workflow/approvals',      // WorkflowController
        REQUESTS: '/workflow/requests',
        THRESHOLDS: '/threshold',
        
        // Reports
        REPORTS: '/reports/generate',
        QUARTERLY_REPORT: '/performance/programperformance/{programId}/report',
        
        // Data Management
        DATA_SYNC: '/data/sync-status',
        
        // Risks
        RISKS: '/risks',
        RISK_ESCALATION: '/risks/escalation',
        
        // Achievements
        ACHIEVEMENTS: '/achievements',
        
        // Indicators
        INDICATORS: '/indicators',
        INDICATOR_DETAILS: '/indicators/{id}',
        MACROECONOMIC: '/performance/macroeconomicindicators',
        
        // Thresholds
        THRESHOLDS: '/performance/thresholds',
        THRESHOLD_DETAILS: '/performance/thresholds/{id}',
        
        // Reports
        REPORT_DETAILS: '/reports/{id}',
        REPORT_DOWNLOAD: '/reports/{id}/download',
        REPORT_COMMENTS: '/reports/{id}/comments',
        
        // Request History
        REQUEST_HISTORY: '/performance/requests/{id}/history',
        REQUEST_RESUBMIT: '/performance/requests/{id}/resubmit',
        
        // Approval Decisions
        APPROVAL_DECIDE: '/performance/approvals/decide',
        
        // Milestones
        MILESTONES: '/milestones',
        
        // Audit
        AUDIT_LOGS: '/audit/logs'
    }
};

// Environment detection
const Environment = {
    isDevelopment: () => {
        return window.location.hostname === 'localhost' || 
               window.location.hostname === '127.0.0.1';
    },
    isProduction: () => {
        return !Environment.isDevelopment();
    }
};

// Token management
const TokenManager = {
    // Check environment and handle token appropriately
    getToken: () => {
        // Only bypass in development mode
        if (Environment.isDevelopment() && !localStorage.getItem('access_token')) {
            console.warn('Development mode: Using bypass token');
            return 'dummy-development-token-bypass-login';
        }
        return localStorage.getItem('access_token');
    },
    setToken: (token) => localStorage.setItem('access_token', token),
    removeToken: () => localStorage.removeItem('access_token'),
    getRefreshToken: () => localStorage.getItem('refresh_token') || 'dummy-refresh-token',
    setRefreshToken: (token) => localStorage.setItem('refresh_token', token),
    removeRefreshToken: () => localStorage.removeItem('refresh_token')
};

// API Service Class
class APIService {
    constructor() {
        this.baseURL = API_CONFIG.BASE_URL;
    }

    /**
     * Generic fetch wrapper with authentication
     */
    async fetchWithAuth(url, options = {}) {
        const token = TokenManager.getToken();
        
        const defaultHeaders = {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Accept-Language': 'ar' // Arabic as default language
        };

        if (token) {
            defaultHeaders['Authorization'] = `Bearer ${token}`;
        }

        const config = {
            ...options,
            headers: {
                ...defaultHeaders,
                ...options.headers
            }
        };

        try {
            const response = await fetch(`${this.baseURL}${url}`, config);
            
            // Handle 401 Unauthorized - try to refresh token
            if (response.status === 401 && TokenManager.getRefreshToken()) {
                const refreshed = await this.refreshToken();
                if (refreshed) {
                    // Retry the original request with new token
                    config.headers['Authorization'] = `Bearer ${TokenManager.getToken()}`;
                    return fetch(`${this.baseURL}${url}`, config);
                } else {
                    // Redirect to login if refresh failed
                    this.redirectToLogin();
                }
            }

            return response;
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    }

    /**
     * Refresh the access token
     */
    async refreshToken() {
        try {
            const response = await fetch(`${this.baseURL}${API_CONFIG.ENDPOINTS.REFRESH_TOKEN}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    refreshToken: TokenManager.getRefreshToken()
                })
            });

            if (response.ok) {
                const data = await response.json();
                TokenManager.setToken(data.accessToken);
                TokenManager.setRefreshToken(data.refreshToken);
                return true;
            }
        } catch (error) {
            console.error('Token refresh failed:', error);
        }
        return false;
    }

    /**
     * Redirect to login page
     */
    redirectToLogin() {
        // BYPASS LOGIN: Skip redirect for development
        console.log('Login redirect bypassed for development');
        // Comment out for now - UNCOMMENT IN PRODUCTION
        // TokenManager.removeToken();
        // TokenManager.removeRefreshToken();
        // window.location.href = '/login.html';
    }

    /**
     * Login user
     */
    async login(username, password) {
        const response = await fetch(`${this.baseURL}${API_CONFIG.ENDPOINTS.LOGIN}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const data = await response.json();
            TokenManager.setToken(data.accessToken);
            TokenManager.setRefreshToken(data.refreshToken);
            return data;
        } else {
            throw new Error('Login failed');
        }
    }

    /**
     * Logout user
     */
    async logout() {
        try {
            await this.fetchWithAuth(API_CONFIG.ENDPOINTS.LOGOUT, {
                method: 'POST'
            });
        } finally {
            TokenManager.removeToken();
            TokenManager.removeRefreshToken();
            window.location.href = '/login.html';
        }
    }

    /**
     * Get programs list
     */
    async getPrograms(year = null, quarter = null) {
        let url = API_CONFIG.ENDPOINTS.PROGRAMS;
        const params = new URLSearchParams();
        if (year) params.append('year', year);
        if (quarter) params.append('quarter', quarter);
        if (params.toString()) url += `?${params.toString()}`;
        
        const response = await this.fetchWithAuth(url);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch programs');
    }

    /**
     * Get program details
     */
    async getProgramDetails(id) {
        const response = await this.fetchWithAuth(`${API_CONFIG.ENDPOINTS.PROGRAMS}/${id}`);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch program details');
    }

    /**
     * Create new program
     */
    async createProgram(programData) {
        const response = await this.fetchWithAuth(API_CONFIG.ENDPOINTS.PROGRAMS, {
            method: 'POST',
            body: JSON.stringify(programData)
        });
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to create program');
    }

    /**
     * Update program
     */
    async updateProgram(id, programData) {
        const response = await this.fetchWithAuth(`${API_CONFIG.ENDPOINTS.PROGRAMS}/${id}`, {
            method: 'PUT',
            body: JSON.stringify(programData)
        });
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to update program');
    }

    /**
     * Submit program for approval
     */
    async submitForApproval(id) {
        const response = await this.fetchWithAuth(`${API_CONFIG.ENDPOINTS.PROGRAMS}/${id}/submit`, {
            method: 'POST'
        });
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to submit for approval');
    }

    /**
     * Get dashboard data
     */
    async getDashboardData() {
        const response = await this.fetchWithAuth(API_CONFIG.ENDPOINTS.PERFORMANCE_DASHBOARD);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch dashboard data');
    }

    /**
     * Get initiatives
     */
    async getInitiatives(programId = null) {
        let url = API_CONFIG.ENDPOINTS.INITIATIVES;
        if (programId) url += `?programId=${programId}`;
        
        const response = await this.fetchWithAuth(url);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch initiatives');
    }

    /**
     * Get approvals list
     */
    async getApprovals() {
        const response = await this.fetchWithAuth(API_CONFIG.ENDPOINTS.APPROVALS);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch approvals');
    }

    /**
     * Get requests list
     */
    async getRequests() {
        const response = await this.fetchWithAuth(API_CONFIG.ENDPOINTS.REQUESTS);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch requests');
    }

    /**
     * Get data sync status
     */
    async getDataSyncStatus() {
        const response = await this.fetchWithAuth(API_CONFIG.ENDPOINTS.DATA_SYNC);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch data sync status');
    }

    /**
     * Generate quarterly report
     */
    async generateQuarterlyReport(programId, year, quarter) {
        const url = API_CONFIG.ENDPOINTS.QUARTERLY_REPORT
            .replace('{programId}', programId) + 
            `?year=${year}&quarter=${quarter}`;
        
        const response = await this.fetchWithAuth(url);
        if (response.ok) {
            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.download = `Program_${programId}_Q${quarter}_${year}_Report.xlsx`;
            document.body.appendChild(link);
            link.click();
            link.remove();
            return true;
        }
        throw new Error('Failed to generate report');
    }

    /**
     * Get audit logs
     */
    async getAuditLogs(filters = {}) {
        const params = new URLSearchParams(filters);
        const url = `${API_CONFIG.ENDPOINTS.AUDIT_LOGS}?${params.toString()}`;
        
        const response = await this.fetchWithAuth(url);
        if (response.ok) {
            return await response.json();
        }
        throw new Error('Failed to fetch audit logs');
    }

    /**
     * Check API health status
     */
    async checkHealth() {
        try {
            const response = await fetch(`${this.baseURL}/health`);
            return response.ok;
        } catch {
            return false;
        }
    }
}

// Create global instance
window.apiService = new APIService();

// Helper function to show loading state
function showLoading(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.innerHTML = `
            <div class="text-center p-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">جاري التحميل...</span>
                </div>
                <p class="mt-2">جاري تحميل البيانات...</p>
            </div>
        `;
    }
}

// Helper function to show error
function showError(elementId, message) {
    const element = document.getElementById(elementId);
    if (element) {
        element.innerHTML = `
            <div class="alert alert-danger" role="alert">
                <i class="bi bi-exclamation-triangle-fill"></i>
                ${message}
            </div>
        `;
    }
}

// Helper function to show success
function showSuccess(message) {
    const toast = document.createElement('div');
    toast.className = 'toast position-fixed top-0 end-0 m-3';
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="toast-header bg-success text-white">
            <strong class="me-auto">نجاح</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;
    document.body.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    setTimeout(() => toast.remove(), 5000);
}

// Initialize API connection check on page load
document.addEventListener('DOMContentLoaded', async function() {
    // BYPASS LOGIN: Skip authentication check for development
    console.log('Authentication bypassed for development mode');
    
    // Check if we're on login page
    if (window.location.pathname.includes('login.html')) {
        // Auto-redirect from login to main page in bypass mode
        if (!window.location.pathname.includes('index.html')) {
            console.log('Auto-redirecting from login page (bypass mode)');
            // Uncomment to auto-redirect from login
            // window.location.href = '/index.html';
        }
        return;
    }

    // BYPASS: Skip authentication check
    // Original code commented out - RESTORE IN PRODUCTION
    /*
    // Check if user is authenticated
    if (!TokenManager.getToken()) {
        window.apiService.redirectToLogin();
        return;
    }
    */

    // Check API health
    const isHealthy = await window.apiService.checkHealth();
    if (!isHealthy) {
        console.warn('API server is not responding. Some features may not work.');
        // You could show a warning banner here
    }
});

// Export for ES6 modules if needed
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { APIService, API_CONFIG };
}