// SDTicaret Modern Web Application JavaScript
// Combined with modern enhancements and legacy compatibility

// Global variables
let currentPage = 1;
let pageSize = 10;

// Modern SDTicaret UI Class
class SDTicaretUI {
    constructor() {
        this.init();
    }

    init() {
        this.setupAnimations();
        this.setupInteractions();
        this.setupNotifications();
        this.setupLoadingStates();
        this.setupSearchFunctionality();
        this.setupResponsiveBehavior();
    }

    // Animasyonlar
    setupAnimations() {
        // Sayfa yüklendiğinde fade-in animasyonu
        document.addEventListener('DOMContentLoaded', () => {
            const elements = document.querySelectorAll('.fade-in-up');
            elements.forEach((element, index) => {
                element.style.animationDelay = `${index * 0.1}s`;
            });
        });

        // Scroll animasyonları
        this.setupScrollAnimations();
    }

    setupScrollAnimations() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, observerOptions);

        // Animasyon için elementleri gözlemle
        document.querySelectorAll('.card, .dashboard-card, .product-item').forEach(el => {
            observer.observe(el);
        });
    }

    // Etkileşimler
    setupInteractions() {
        // Hover efektleri
        this.setupHoverEffects();
        
        // Click efektleri
        this.setupClickEffects();
        
        // Form validasyonları
        this.setupFormValidation();
    }

    setupHoverEffects() {
        // Kart hover efektleri
        document.querySelectorAll('.card, .dashboard-card').forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-4px)';
                this.style.boxShadow = '0 20px 40px rgba(0,0,0,0.1)';
            });

            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = '';
            });
        });

        // Buton hover efektleri
        document.querySelectorAll('.btn').forEach(btn => {
            btn.addEventListener('mouseenter', function() {
                if (!this.classList.contains('btn-loading')) {
                    this.style.transform = 'translateY(-2px)';
                }
            });

            btn.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
            });
        });
    }

    setupClickEffects() {
        // Ripple efekti
        document.addEventListener('click', (e) => {
            if (e.target.classList.contains('btn')) {
                this.createRipple(e);
            }
        });
    }

    createRipple(event) {
        const button = event.currentTarget;
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;

        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = x + 'px';
        ripple.style.top = y + 'px';
        ripple.classList.add('ripple');

        button.appendChild(ripple);

        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    setupFormValidation() {
        // Real-time form validation
        document.querySelectorAll('form').forEach(form => {
            const inputs = form.querySelectorAll('input, select, textarea');
            
            inputs.forEach(input => {
                input.addEventListener('blur', () => {
                    this.validateField(input);
                });

                input.addEventListener('input', () => {
                    if (input.classList.contains('is-invalid')) {
                        this.validateField(input);
                    }
                });
            });
        });
    }

    validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';

        // Temel validasyon kuralları
        switch (fieldName) {
            case 'Username':
                if (value.length < 3) {
                    isValid = false;
                    errorMessage = 'Kullanıcı adı en az 3 karakter olmalıdır.';
                }
                break;
            case 'Email':
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    isValid = false;
                    errorMessage = 'Geçerli bir e-posta adresi giriniz.';
                }
                break;
            case 'Password':
                if (value.length < 6) {
                    isValid = false;
                    errorMessage = 'Şifre en az 6 karakter olmalıdır.';
                }
                break;
        }

        this.showFieldValidation(field, isValid, errorMessage);
    }

    showFieldValidation(field, isValid, message) {
        const feedback = field.parentNode.querySelector('.invalid-feedback') || 
                        field.parentNode.querySelector('.valid-feedback');

        if (feedback) {
            feedback.remove();
        }

        field.classList.remove('is-valid', 'is-invalid');

        if (isValid) {
            field.classList.add('is-valid');
        } else {
            field.classList.add('is-invalid');
            const errorDiv = document.createElement('div');
            errorDiv.className = 'invalid-feedback';
            errorDiv.textContent = message;
            field.parentNode.appendChild(errorDiv);
        }
    }

    // Bildirimler
    setupNotifications() {
        // Toast bildirimleri
        this.createToastContainer();
    }

    createToastContainer() {
        if (!document.getElementById('toast-container')) {
            const container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
    }

    showToast(message, type = 'info', duration = 5000) {
        const toastId = 'toast-' + Date.now();
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type} border-0`;
        toast.id = toastId;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;

        const container = document.getElementById('toast-container');
        container.appendChild(toast);

        const bsToast = new bootstrap.Toast(toast, { delay: duration });
        bsToast.show();

        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    }

    // Loading durumları
    setupLoadingStates() {
        // Form submit loading
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', (e) => {
                const submitBtn = form.querySelector('button[type="submit"]');
                if (submitBtn) {
                    this.setButtonLoading(submitBtn, true);
                }
            });
        });

        // Link loading
        document.querySelectorAll('a[href]').forEach(link => {
            link.addEventListener('click', (e) => {
                if (link.classList.contains('btn') && !link.classList.contains('btn-loading')) {
                    this.setButtonLoading(link, true);
                }
            });
        });
    }

    setButtonLoading(button, isLoading) {
        if (isLoading) {
            const originalText = button.innerHTML;
            button.dataset.originalText = originalText;
            button.innerHTML = '<span class="loading me-2"></span>Yükleniyor...';
            button.disabled = true;
            button.classList.add('btn-loading');
        } else {
            button.innerHTML = button.dataset.originalText || button.innerHTML;
            button.disabled = false;
            button.classList.remove('btn-loading');
        }
    }

    // Arama fonksiyonalitesi
    setupSearchFunctionality() {
        const searchInputs = document.querySelectorAll('input[type="search"], #searchInput');
        
        searchInputs.forEach(input => {
            let debounceTimer;
            
            input.addEventListener('input', (e) => {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(() => {
                    this.performSearch(e.target.value, input.dataset.target);
                }, 300);
            });
        });
    }

    performSearch(query, targetSelector) {
        const targetElements = document.querySelectorAll(targetSelector || '.searchable-item');
        
        targetElements.forEach(element => {
            const text = element.textContent.toLowerCase();
            const matches = text.includes(query.toLowerCase());
            
            if (matches) {
                element.style.display = '';
                element.classList.remove('search-hidden');
            } else {
                element.style.display = 'none';
                element.classList.add('search-hidden');
            }
        });
    }

    // Responsive davranış
    setupResponsiveBehavior() {
        // Sidebar toggle
        const sidebarToggle = document.querySelector('[data-bs-toggle="collapse"]');
        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', () => {
                document.body.classList.toggle('sidebar-collapsed');
            });
        }

        // Mobile menu
        const mobileMenuToggle = document.querySelector('.navbar-toggler');
        if (mobileMenuToggle) {
            mobileMenuToggle.addEventListener('click', () => {
                document.body.classList.toggle('mobile-menu-open');
            });
        }

        // Window resize
        window.addEventListener('resize', () => {
            this.handleResize();
        });
    }

    handleResize() {
        const width = window.innerWidth;
        
        if (width < 768) {
            document.body.classList.add('mobile-view');
        } else {
            document.body.classList.remove('mobile-view');
        }
    }
}

// Legacy Utility functions (for backward compatibility)
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

// Modern Utility Class
class SDTicaretUtils {
    static formatCurrency(amount, currency = 'TRY') {
        return new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: currency
        }).format(amount);
    }

    static formatDate(date, options = {}) {
        const defaultOptions = {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        };
        
        return new Intl.DateTimeFormat('tr-TR', { ...defaultOptions, ...options }).format(new Date(date));
    }

    static debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    static throttle(func, limit) {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    static copyToClipboard(text) {
        if (navigator.clipboard) {
            return navigator.clipboard.writeText(text);
        } else {
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            return Promise.resolve();
        }
    }

    static downloadFile(url, filename) {
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

// Modern API Helper Class
class SDTicaretAPI {
    static async request(url, options = {}) {
        const defaultOptions = {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        };

        const config = { ...defaultOptions, ...options };

        try {
            const response = await fetch(url, config);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    }

    static get(url) {
        return this.request(url, { method: 'GET' });
    }

    static post(url, data) {
        return this.request(url, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    static put(url, data) {
        return this.request(url, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    static delete(url) {
        return this.request(url, { method: 'DELETE' });
    }
}

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
    // Initialize modern UI
    window.sdTicaretUI = new SDTicaretUI();
    
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

// Global error handler - only for critical errors
window.addEventListener('error', (event) => {
    console.error('Global error:', event.error);
    
    // Ignore errors from dropdown interactions
    if (event.target && event.target.closest('.dropdown')) {
        return;
    }
    
    // Only show error for critical errors
    if (event.error && event.error.message && !event.error.message.includes('dropdown')) {
        if (window.sdTicaretUI) {
            window.sdTicaretUI.showToast('Bir hata oluştu. Lütfen sayfayı yenileyin.', 'danger');
        }
    }
});

// Unhandled promise rejection handler - only for API errors
window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled promise rejection:', event.reason);
    
    // Only show error for API-related rejections
    if (event.reason && typeof event.reason === 'object' && event.reason.message) {
        if (window.sdTicaretUI) {
            window.sdTicaretUI.showToast('Bir hata oluştu. Lütfen tekrar deneyin.', 'danger');
        }
    }
});

// Export for global access
window.SDTicaret = SDTicaret;
window.SDTicaretUtils = SDTicaretUtils;
window.SDTicaretAPI = SDTicaretAPI;
window.TableManager = TableManager;
window.FormManager = FormManager;
window.DashboardManager = DashboardManager;
