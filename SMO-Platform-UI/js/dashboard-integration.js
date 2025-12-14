/**
 * SMO Platform - Dashboard Integration
 * Connects all dashboard pages to the backend API
 * Ready to use - just include this script in your HTML pages
 */

// API Configuration
const API_CONFIG = {
    baseUrl: window.location.hostname === 'localhost' 
        ? 'https://localhost:7001/api' 
        : '/api',
    timeout: 30000
};

// Dashboard Data Loader
class DashboardIntegration {
    constructor() {
        this.apiService = window.apiService || this.createApiService();
    }

    // Create a simple API service if not available
    createApiService() {
        return {
            fetchWithAuth: async (endpoint, options = {}) => {
                const token = localStorage.getItem('authToken');
                const headers = {
                    'Content-Type': 'application/json',
                    ...options.headers
                };
                
                if (token) {
                    headers['Authorization'] = `Bearer ${token}`;
                }

                try {
                    const response = await fetch(`${API_CONFIG.baseUrl}${endpoint}`, {
                        ...options,
                        headers
                    });

                    if (!response.ok) {
                        throw new Error(`API Error: ${response.status}`);
                    }

                    return await response.json();
                } catch (error) {
                    console.error('API Error:', error);
                    throw error;
                }
            }
        };
    }

    // Load main dashboard overview
    async loadOverviewDashboard() {
        try {
            const data = await this.apiService.fetchWithAuth('/dashboard/overview');
            
            // Update Vision 2030 stats
            this.updateElement('vision-total-programs', data.Vision2030?.TotalPrograms);
            this.updateElement('vision-active-initiatives', data.Vision2030?.ActiveInitiatives);
            this.updateElement('vision-total-kpis', data.Vision2030?.TotalKPIs);
            this.updateElement('vision-average-progress', `${data.Vision2030?.AverageProgress?.toFixed(1)}%`);
            
            // Update Performance Management stats
            this.updateElement('perf-total-employees', data.PerformanceManagement?.TotalEmployees);
            this.updateElement('perf-active-goals', data.PerformanceManagement?.ActiveGoals);
            this.updateElement('perf-completed-goals', data.PerformanceManagement?.CompletedGoals);
            this.updateElement('perf-average-progress', `${data.PerformanceManagement?.AverageGoalProgress?.toFixed(1)}%`);
            
            // Update Shared Resources
            this.updateElement('total-risks', data.SharedResources?.TotalRisks);
            this.updateElement('critical-risks', data.SharedResources?.CriticalRisks);
            this.updateElement('high-risks', data.SharedResources?.HighRisks);
            
            // Update Quick Stats
            this.updateElement('overall-progress', `${data.QuickStats?.OnTrackPercentage?.toFixed(1)}%`);
            this.updateProgressBar('overall-progress-bar', data.QuickStats?.OnTrackPercentage);
            
            // Update Recent Activity
            this.updateRecentActivity(data.RecentActivity);
            
            return data;
        } catch (error) {
            console.error('Failed to load overview dashboard:', error);
            this.showError('Failed to load dashboard data');
        }
    }

    // Load Vision 2030 dashboard
    async loadVision2030Dashboard() {
        try {
            const data = await this.apiService.fetchWithAuth('/dashboard/vision2030');
            
            // Update summary cards
            this.updateElement('total-programs', data.Summary?.TotalPrograms);
            this.updateElement('active-programs', data.Summary?.ActivePrograms);
            this.updateElement('completed-programs', data.Summary?.CompletedPrograms);
            
            // Update KPI overview
            this.updateElement('kpis-total', data.KPIOverview?.Total);
            this.updateElement('kpis-on-target', data.KPIOverview?.OnTarget);
            this.updateElement('kpis-at-risk', data.KPIOverview?.AtRisk);
            this.updateElement('kpis-behind', data.KPIOverview?.Behind);
            
            // Update Initiative status
            this.updateElement('initiatives-total', data.InitiativeStatus?.Total);
            this.updateElement('initiatives-in-progress', data.InitiativeStatus?.InProgress);
            this.updateElement('initiatives-completed', data.InitiativeStatus?.Completed);
            
            // Render programs table
            this.renderProgramsTable(data.ProgramsBreakdown);
            
            // Render upcoming milestones
            this.renderUpcomingMilestones(data.UpcomingMilestones);
            
            return data;
        } catch (error) {
            console.error('Failed to load Vision 2030 dashboard:', error);
            this.showError('Failed to load Vision 2030 data');
        }
    }

    // Load Performance Management dashboard
    async loadPerformanceDashboard() {
        try {
            const data = await this.apiService.fetchWithAuth('/dashboard/performance');
            
            // Update employee overview
            this.updateElement('total-employees', data.EmployeeOverview?.Total);
            this.updateElement('active-employees', data.EmployeeOverview?.Active);
            
            // Update goals overview
            this.updateElement('total-goals', data.GoalsOverview?.TotalGoals);
            this.updateElement('active-goals', data.GoalsOverview?.ActiveGoals);
            this.updateElement('completed-goals', data.GoalsOverview?.CompletedGoals);
            this.updateElement('goals-average-progress', `${data.GoalsOverview?.AverageProgress?.toFixed(1)}%`);
            
            // Update reviews status
            this.updateElement('current-quarter', data.ReviewsStatus?.CurrentQuarter);
            this.updateElement('reviews-completed', data.ReviewsStatus?.Completed);
            this.updateElement('reviews-in-progress', data.ReviewsStatus?.InProgress);
            this.updateElement('reviews-not-started', data.ReviewsStatus?.NotStarted);
            this.updateElement('reviews-average-score', `${data.ReviewsStatus?.AverageScore?.toFixed(1)}%`);
            
            // Render top performers
            this.renderTopPerformers(data.TopPerformers);
            
            // Render upcoming reviews
            this.renderUpcomingReviews(data.UpcomingReviews);
            
            return data;
        } catch (error) {
            console.error('Failed to load Performance dashboard:', error);
            this.showError('Failed to load Performance data');
        }
    }

    // Load employee profile data
    async loadEmployeeProfile(employeeId) {
        try {
            const data = await this.apiService.fetchWithAuth(`/performance/performance/employees/${employeeId}`);
            
            // Update employee info
            this.updateElement('employee-name', data.FullName);
            this.updateElement('employee-position', data.Position);
            this.updateElement('employee-department', data.Department);
            this.updateElement('employee-email', data.Email);
            this.updateElement('employee-manager', data.Manager?.FullName);
            
            // Update performance metrics
            this.updateElement('goals-count', data.PerformanceMetrics?.TotalGoals);
            this.updateElement('completed-goals-count', data.PerformanceMetrics?.CompletedGoals);
            this.updateElement('active-goals-count', data.PerformanceMetrics?.ActiveGoals);
            this.updateElement('average-completion', `${data.PerformanceMetrics?.AverageGoalCompletion?.toFixed(1)}%`);
            
            // Render goals list
            this.renderEmployeeGoals(data.PerformanceGoals);
            
            // Render recent reviews
            this.renderEmployeeReviews(data.RecentReviews);
            
            return data;
        } catch (error) {
            console.error('Failed to load employee profile:', error);
            this.showError('Failed to load employee data');
        }
    }

    // Generate performance report
    async generatePerformanceReport(options) {
        try {
            const requestData = {
                type: options.type || 'quarterly',
                period: options.period || `Q${Math.floor((new Date().getMonth() / 3) + 1)}-${new Date().getFullYear()}`,
                format: options.format || 'pptx',
                programId: options.programId,
                year: options.year || new Date().getFullYear()
            };

            const data = await this.apiService.fetchWithAuth('/report/generate/performance', {
                method: 'POST',
                body: JSON.stringify(requestData)
            });

            // Show success message
            this.showSuccess(`Report generated successfully! Download from: ${data.DownloadUrl}`);
            
            // Trigger download
            if (data.DownloadUrl) {
                window.open(`${API_CONFIG.baseUrl}${data.DownloadUrl}`, '_blank');
            }
            
            return data;
        } catch (error) {
            console.error('Failed to generate report:', error);
            this.showError('Failed to generate report');
        }
    }

    // Helper: Update element content
    updateElement(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value || '0';
        }
    }

    // Helper: Update progress bar
    updateProgressBar(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.style.width = `${value || 0}%`;
            element.setAttribute('aria-valuenow', value || 0);
        }
    }

    // Helper: Update recent activity list
    updateRecentActivity(activities) {
        const container = document.getElementById('recent-activity');
        if (!container || !activities) return;

        container.innerHTML = activities.map(activity => `
            <div class="activity-item">
                <span class="activity-type badge bg-primary">${activity.Type}</span>
                <span class="activity-action">${activity.Action}</span>
                <span class="activity-item-name">${activity.Item}</span>
                <span class="activity-time text-muted">${activity.Time}</span>
            </div>
        `).join('');
    }

    // Helper: Render programs table
    renderProgramsTable(programs) {
        const container = document.getElementById('programs-table');
        if (!container || !programs) return;

        container.innerHTML = programs.map(program => `
            <tr>
                <td>${program.Code}</td>
                <td>${program.Name}</td>
                <td>
                    <div class="progress">
                        <div class="progress-bar bg-${program.PerformanceColor}" 
                             style="width: ${program.Progress}%">${program.Progress}%</div>
                    </div>
                </td>
                <td>${program.InitiativesCount}</td>
                <td>${program.KPIsCount}</td>
                <td><span class="badge bg-${program.Status === 'Active' ? 'success' : 'warning'}">${program.Status}</span></td>
            </tr>
        `).join('');
    }

    // Helper: Render top performers
    renderTopPerformers(performers) {
        const container = document.getElementById('top-performers');
        if (!container || !performers) return;

        container.innerHTML = performers.map(performer => `
            <div class="performer-card">
                <h5>${performer.FullName}</h5>
                <p class="text-muted">${performer.Department} - ${performer.Position}</p>
                <div class="score-badge">${performer.LatestScore?.toFixed(1)}%</div>
                <small>${performer.GoalsCompleted} goals completed</small>
            </div>
        `).join('');
    }

    // Helper: Show error message
    showError(message) {
        const alertHtml = `
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <strong>Error:</strong> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        this.showAlert(alertHtml);
    }

    // Helper: Show success message
    showSuccess(message) {
        const alertHtml = `
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <strong>Success:</strong> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        this.showAlert(alertHtml);
    }

    // Helper: Show alert
    showAlert(html) {
        const container = document.getElementById('alert-container') || document.body;
        const alertDiv = document.createElement('div');
        alertDiv.innerHTML = html;
        container.insertBefore(alertDiv.firstChild, container.firstChild);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const alert = container.querySelector('.alert');
            if (alert) alert.remove();
        }, 5000);
    }

    // Helper: Render upcoming milestones
    renderUpcomingMilestones(milestones) {
        const container = document.getElementById('upcoming-milestones');
        if (!container || !milestones) return;

        container.innerHTML = milestones.map(milestone => `
            <div class="milestone-item">
                <h6>${milestone.Title}</h6>
                <p class="text-muted mb-1">${milestone.InitiativeName} - ${milestone.ProgramName}</p>
                <small>Due: ${new Date(milestone.TargetDate).toLocaleDateString()} (${milestone.DaysRemaining} days)</small>
            </div>
        `).join('');
    }

    // Helper: Render employee goals
    renderEmployeeGoals(goals) {
        const container = document.getElementById('employee-goals');
        if (!container || !goals) return;

        container.innerHTML = goals.map(goal => `
            <div class="goal-card">
                <h5>${goal.Title}</h5>
                <p class="text-muted">${goal.GoalType} - ${goal.Category}</p>
                <div class="progress mb-2">
                    <div class="progress-bar" style="width: ${goal.Progress}%">${goal.Progress}%</div>
                </div>
                <small>Weight: ${goal.Weight}% | Priority: ${goal.Priority}</small>
            </div>
        `).join('');
    }

    // Helper: Render employee reviews
    renderEmployeeReviews(reviews) {
        const container = document.getElementById('employee-reviews');
        if (!container || !reviews) return;

        container.innerHTML = reviews.map(review => `
            <div class="review-card">
                <h6>${review.ReviewType} - ${review.ReviewPeriod}</h6>
                <p>Overall Score: <strong>${review.OverallScore}%</strong></p>
                <p>Rating: <span class="badge bg-success">${review.OverallRating}</span></p>
                <p>Reviewer: ${review.ReviewerName}</p>
                <small>Date: ${new Date(review.ReviewDate).toLocaleDateString()}</small>
            </div>
        `).join('');
    }

    // Helper: Render upcoming reviews
    renderUpcomingReviews(reviews) {
        const container = document.getElementById('upcoming-reviews');
        if (!container || !reviews) return;

        container.innerHTML = reviews.map(review => `
            <tr>
                <td>${review.FullName}</td>
                <td>${review.Department}</td>
                <td>${review.ManagerName || 'N/A'}</td>
                <td>${review.DaysOverdue > 0 ? `<span class="text-danger">${review.DaysOverdue} days overdue</span>` : 'Pending'}</td>
            </tr>
        `).join('');
    }
}

// Initialize dashboard integration when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.dashboardIntegration = new DashboardIntegration();
    
    // Auto-load based on page
    const currentPage = window.location.pathname.split('/').pop();
    
    if (currentPage === 'performance-dashboard.html') {
        window.dashboardIntegration.loadPerformanceDashboard();
    } else if (currentPage === 'vision.html' || currentPage === 'vision-programs.html') {
        window.dashboardIntegration.loadVision2030Dashboard();
    } else if (currentPage === 'index.html' || currentPage === 'dashboard.html') {
        window.dashboardIntegration.loadOverviewDashboard();
    }
    
    // Auto-refresh every 30 seconds
    setInterval(() => {
        if (currentPage === 'performance-dashboard.html') {
            window.dashboardIntegration.loadPerformanceDashboard();
        } else if (currentPage === 'vision.html') {
            window.dashboardIntegration.loadVision2030Dashboard();
        } else if (currentPage === 'index.html' || currentPage === 'dashboard.html') {
            window.dashboardIntegration.loadOverviewDashboard();
        }
    }, 30000);
});

// Export for use in other scripts
window.DashboardIntegration = DashboardIntegration;
