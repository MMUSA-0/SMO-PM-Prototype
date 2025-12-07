/**
 * CRUD Operations Utility
 * Provides common add, edit, delete, and view functions for all pages
 * Integrates with the API service for backend communication
 */

// Global CRUD Operations Manager
const CRUDOperations = {
    /**
     * Generic function to handle add/create operations
     * @param {string} endpoint - API endpoint
     * @param {object} data - Data to send
     * @param {string} successMessage - Success message to display
     * @param {function} onSuccess - Callback on success
     */
    async add(endpoint, data, successMessage = 'تمت الإضافة بنجاح', onSuccess = null) {
        try {
            showLoading('content');
            const response = await window.apiService.fetchWithAuth(endpoint, {
                method: 'POST',
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const result = await response.json();
                showSuccess(successMessage);
                if (onSuccess) onSuccess(result);
                return result;
            } else {
                const error = await response.json();
                throw new Error(error.message || 'فشلت عملية الإضافة');
            }
        } catch (error) {
            console.error('Add operation failed:', error);
            showError('content', error.message || 'حدث خطأ أثناء الإضافة');
            throw error;
        }
    },

    /**
     * Generic function to handle edit/update operations
     * @param {string} endpoint - API endpoint
     * @param {string|number} id - Item ID
     * @param {object} data - Data to update
     * @param {string} successMessage - Success message to display
     * @param {function} onSuccess - Callback on success
     */
    async edit(endpoint, id, data, successMessage = 'تم التعديل بنجاح', onSuccess = null) {
        try {
            showLoading('content');
            const url = `${endpoint}/${id}`;
            const response = await window.apiService.fetchWithAuth(url, {
                method: 'PUT',
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const result = await response.json();
                showSuccess(successMessage);
                if (onSuccess) onSuccess(result);
                return result;
            } else {
                const error = await response.json();
                throw new Error(error.message || 'فشلت عملية التعديل');
            }
        } catch (error) {
            console.error('Edit operation failed:', error);
            showError('content', error.message || 'حدث خطأ أثناء التعديل');
            throw error;
        }
    },

    /**
     * Generic function to handle delete operations
     * @param {string} endpoint - API endpoint
     * @param {string|number} id - Item ID
     * @param {string} confirmMessage - Confirmation message
     * @param {string} successMessage - Success message to display
     * @param {function} onSuccess - Callback on success
     */
    async delete(endpoint, id, confirmMessage = 'هل أنت متأكد من الحذف؟', successMessage = 'تم الحذف بنجاح', onSuccess = null) {
        if (!confirm(confirmMessage)) {
            return false;
        }

        try {
            showLoading('content');
            const url = `${endpoint}/${id}`;
            const response = await window.apiService.fetchWithAuth(url, {
                method: 'DELETE'
            });

            if (response.ok) {
                showSuccess(successMessage);
                if (onSuccess) onSuccess();
                return true;
            } else {
                const error = await response.json();
                throw new Error(error.message || 'فشلت عملية الحذف');
            }
        } catch (error) {
            console.error('Delete operation failed:', error);
            showError('content', error.message || 'حدث خطأ أثناء الحذف');
            throw error;
        }
    },

    /**
     * Generic function to handle view/get operations
     * @param {string} endpoint - API endpoint
     * @param {string|number} id - Item ID (optional)
     * @param {function} onSuccess - Callback on success
     */
    async view(endpoint, id = null, onSuccess = null) {
        try {
            showLoading('content');
            const url = id ? `${endpoint}/${id}` : endpoint;
            const response = await window.apiService.fetchWithAuth(url);

            if (response.ok) {
                const result = await response.json();
                if (onSuccess) onSuccess(result);
                return result;
            } else {
                const error = await response.json();
                throw new Error(error.message || 'فشلت عملية جلب البيانات');
            }
        } catch (error) {
            console.error('View operation failed:', error);
            showError('content', error.message || 'حدث خطأ أثناء جلب البيانات');
            throw error;
        }
    },

    /**
     * Load data into a form for editing
     * @param {string} endpoint - API endpoint
     * @param {string|number} id - Item ID
     * @param {object} fieldMapping - Map API fields to form fields
     * @param {string} modalId - Modal ID to show
     */
    async loadForEdit(endpoint, id, fieldMapping = null, modalId = null) {
        try {
            const data = await this.view(endpoint, id);
            
            // If field mapping provided, map the data
            if (fieldMapping) {
                Object.keys(fieldMapping).forEach(apiField => {
                    const formField = fieldMapping[apiField];
                    const element = document.getElementById(formField);
                    if (element) {
                        if (element.type === 'checkbox') {
                            element.checked = data[apiField];
                        } else {
                            element.value = data[apiField] || '';
                        }
                    }
                });
            } else {
                // Auto-map: try to find form fields with same names
                Object.keys(data).forEach(key => {
                    const element = document.getElementById(key) || document.querySelector(`[name="${key}"]`);
                    if (element) {
                        if (element.type === 'checkbox') {
                            element.checked = data[key];
                        } else {
                            element.value = data[key] || '';
                        }
                    }
                });
            }

            // Show modal if provided
            if (modalId) {
                const modal = new bootstrap.Modal(document.getElementById(modalId));
                modal.show();
            }

            return data;
        } catch (error) {
            console.error('Load for edit failed:', error);
            throw error;
        }
    }
};

// Make it globally available
window.CRUDOperations = CRUDOperations;

/**
 * Helper function to show loading state
 */
function showLoading(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.style.opacity = '0.6';
        element.style.pointerEvents = 'none';
    }
}

/**
 * Helper function to hide loading state
 */
function hideLoading(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    }
}

/**
 * Helper function to show error message
 */
function showError(elementId, message) {
    const element = document.getElementById(elementId);
    if (element) {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger alert-dismissible fade show';
        alertDiv.innerHTML = `
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        element.insertBefore(alertDiv, element.firstChild);
        
        // Auto-remove after 5 seconds
        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }
}

/**
 * Helper function to show success message (using existing function from api-service.js)
 */
if (typeof showSuccess === 'undefined') {
    window.showSuccess = function(message) {
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
    };
}
