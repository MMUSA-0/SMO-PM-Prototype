/**
 * Shared Utilities for SMO Platform
 * Common functions used across multiple pages
 */

// Format currency in Saudi Riyal
function formatCurrency(amount) {
    if (!amount) return '0.00 ريال';
    return new Intl.NumberFormat('ar-SA', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(amount) + ' ريال';
}

// Show notification toast
function showNotification(message, type = 'info') {
    const toast = `
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'info' ? 'info' : type === 'danger' ? 'danger' : 'warning'} border-0 show position-fixed top-0 end-0 m-3" role="alert" style="z-index: 9999;">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    
    const toastContainer = document.createElement('div');
    toastContainer.innerHTML = toast;
    document.body.appendChild(toastContainer.firstElementChild);
    
    setTimeout(() => {
        const alert = document.querySelector('.toast.position-fixed');
        if (alert) alert.remove();
    }, 3000);
}

// Initialize Bootstrap tooltips
function initializeTooltips() {
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(el => new bootstrap.Tooltip(el));
}


