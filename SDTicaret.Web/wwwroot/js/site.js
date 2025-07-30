// SDTicaret Web Application JavaScript
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global variables
let currentPage = 1;
let pageSize = 10;

// Utility functions
const SDTicaret = {
    // Show loading spinner
    showLoading: function(element) {
        if (element) {
            element.innerHTML = '<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';
        }
    },

    // Hide loading spinner
    hideLoading: function(element) {
        if (element) {
            element.innerHTML = '';
        }
    },

    // Show alert message
    showAlert: function(message, type = 'info', duration = 5000) {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        const container = document.querySelector('.container');
        if (container) {
            container.insertBefore(alertDiv, container.firstChild);
            
            // Auto dismiss after duration
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.remove();
                }
            }, duration);
        }
    },

    // Format currency
    formatCurrency: function(amount) {
        return new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY'
        }).format(amount);
    },

    // Format date
    formatDate: function(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR');
    },

    // Format datetime
    formatDateTime: function(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString('tr-TR');
    },

    // Validate form
    validateForm: function(formElement) {
        const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
            }
        });

        return isValid;
    },

    // Clear form
    clearForm: function(formElement) {
        formElement.reset();
        formElement.querySelectorAll('.is-invalid').forEach(input => {
            input.classList.remove('is-invalid');
        });
    },

    // Confirm delete
    confirmDelete: function(message = 'Bu öğeyi silmek istediğinizden emin misiniz?') {
        return confirm(message);
    },

    // AJAX helper
    ajax: {
        get: function(url, successCallback, errorCallback) {
            fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (successCallback) successCallback(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (errorCallback) errorCallback(error);
                else SDTicaret.showAlert('Bir hata oluştu: ' + error.message, 'danger');
            });
        },

        post: function(url, data, successCallback, errorCallback) {
            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (successCallback) successCallback(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (errorCallback) errorCallback(error);
                else SDTicaret.showAlert('Bir hata oluştu: ' + error.message, 'danger');
            });
        },

        put: function(url, data, successCallback, errorCallback) {
            fetch(url, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (successCallback) successCallback(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (errorCallback) errorCallback(error);
                else SDTicaret.showAlert('Bir hata oluştu: ' + error.message, 'danger');
            });
        },

        delete: function(url, successCallback, errorCallback) {
            fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (successCallback) successCallback(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (errorCallback) errorCallback(error);
                else SDTicaret.showAlert('Bir hata oluştu: ' + error.message, 'danger');
            });
        }
    }
};

// Table functionality
const TableManager = {
    // Initialize table with sorting and filtering
    init: function(tableId) {
        const table = document.getElementById(tableId);
        if (!table) return;

        // Add sorting functionality
        const headers = table.querySelectorAll('th[data-sort]');
        headers.forEach(header => {
            header.style.cursor = 'pointer';
            header.addEventListener('click', () => {
                this.sortTable(table, header.dataset.sort);
            });
        });

        // Add search functionality
        const searchInput = document.querySelector(`#${tableId}-search`);
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                this.filterTable(table, e.target.value);
            });
        }
    },

    // Sort table by column
    sortTable: function(table, columnIndex) {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));
        const isAscending = table.dataset.sortDirection !== 'asc';

        rows.sort((a, b) => {
            const aValue = a.cells[columnIndex].textContent.trim();
            const bValue = b.cells[columnIndex].textContent.trim();
            
            if (isAscending) {
                return aValue.localeCompare(bValue, 'tr');
            } else {
                return bValue.localeCompare(aValue, 'tr');
            }
        });

        rows.forEach(row => tbody.appendChild(row));
        table.dataset.sortDirection = isAscending ? 'asc' : 'desc';
    },

    // Filter table by search term
    filterTable: function(table, searchTerm) {
        const tbody = table.querySelector('tbody');
        const rows = tbody.querySelectorAll('tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const matches = text.includes(searchTerm.toLowerCase());
            row.style.display = matches ? '' : 'none';
        });
    }
};

// Form functionality
const FormManager = {
    // Initialize form validation
    init: function(formId) {
        const form = document.getElementById(formId);
        if (!form) return;

        // Add validation on submit
        form.addEventListener('submit', (e) => {
            if (!SDTicaret.validateForm(form)) {
                e.preventDefault();
                SDTicaret.showAlert('Lütfen tüm gerekli alanları doldurun.', 'warning');
            }
        });

        // Add real-time validation
        const inputs = form.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.addEventListener('blur', () => {
                this.validateField(input);
            });
        });
    },

    // Validate individual field
    validateField: function(field) {
        const value = field.value.trim();
        const isRequired = field.hasAttribute('required');
        const minLength = field.getAttribute('minlength');
        const maxLength = field.getAttribute('maxlength');
        const pattern = field.getAttribute('pattern');

        // Clear previous validation
        field.classList.remove('is-valid', 'is-invalid');

        // Required validation
        if (isRequired && !value) {
            field.classList.add('is-invalid');
            return false;
        }

        // Length validation
        if (minLength && value.length < parseInt(minLength)) {
            field.classList.add('is-invalid');
            return false;
        }

        if (maxLength && value.length > parseInt(maxLength)) {
            field.classList.add('is-invalid');
            return false;
        }

        // Pattern validation
        if (pattern && value && !new RegExp(pattern).test(value)) {
            field.classList.add('is-invalid');
            return false;
        }

        // Email validation
        if (field.type === 'email' && value && !this.isValidEmail(value)) {
            field.classList.add('is-invalid');
            return false;
        }

        field.classList.add('is-valid');
        return true;
    },

    // Email validation
    isValidEmail: function(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
};

// Dashboard functionality
const DashboardManager = {
    // Initialize dashboard
    init: function() {
        this.loadStats();
        this.initCharts();
    },

    // Load dashboard statistics
    loadStats: function() {
        const statsContainer = document.getElementById('dashboard-stats');
        if (!statsContainer) return;

        SDTicaret.ajax.get('/api/dashboard/stats', (data) => {
            this.updateStats(statsContainer, data);
        }, (error) => {
            console.error('Failed to load dashboard stats:', error);
        });
    },

    // Update statistics display
    updateStats: function(container, data) {
        // Update each stat card
        Object.keys(data).forEach(key => {
            const element = container.querySelector(`[data-stat="${key}"]`);
            if (element) {
                element.textContent = data[key];
            }
        });
    },

    // Initialize charts (placeholder for future chart implementation)
    initCharts: function() {
        // This would be implemented with a charting library like Chart.js
        console.log('Charts initialization placeholder');
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize table managers
    document.querySelectorAll('table[data-sortable]').forEach(table => {
        TableManager.init(table.id);
    });

    // Initialize form managers
    document.querySelectorAll('form[data-validate]').forEach(form => {
        FormManager.init(form.id);
    });

    // Initialize dashboard if on dashboard page
    if (document.getElementById('dashboard-stats')) {
        DashboardManager.init();
    }

    // Add delete confirmation to all delete buttons
    document.querySelectorAll('.btn-delete').forEach(button => {
        button.addEventListener('click', function(e) {
            if (!SDTicaret.confirmDelete()) {
                e.preventDefault();
            }
        });
    });

    // Add form clearing to reset buttons
    document.querySelectorAll('.btn-reset').forEach(button => {
        button.addEventListener('click', function(e) {
            const form = this.closest('form');
            if (form) {
                SDTicaret.clearForm(form);
            }
        });
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(() => {
        document.querySelectorAll('.alert').forEach(alert => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 5000);
});

// Export for global access
window.SDTicaret = SDTicaret;
window.TableManager = TableManager;
window.FormManager = FormManager;
window.DashboardManager = DashboardManager;
