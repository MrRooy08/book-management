// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('submit', function (event) {
    const form = event.target;
    if (!form.classList.contains('auth-form')) return;
    event.preventDefault();

    const modalSelector =  '#loginModal .modal-content';
    const modalContent = document.querySelector(modalSelector);
    console.log("Modal content", modalContent);
    console.log("Form", form);
    const formData = new FormData(form);
    const xhr = new XMLHttpRequest();
    xhr.open('POST', form.action, true);
    xhr.onload = function () {
        console.log("Status:", xhr.status);
        console.log("Content-Type:", xhr.getResponseHeader("Content-Type"));
        if (xhr.status ===200) {
            try {
                const result = JSON.parse(xhr.responseText);
                console.log("Result", result);
                if (result.success) {
                    // Login/Register thành công -> redirect
                    window.location.href = result.redirectUrl;
                } else if (result.isLocked) {
                    // Tài khoản bị khóa
                    showAccountLockedAlert(result.message);
                } else if (result.html) {
                    // Server trả về form mới (có thể có validation errors)
                    modalContent.innerHTML = result.html;

                    // ⚠️ Gắn lại event cho nút Register / BackToLogin
                    attachModalEvents();
                }
            } catch (e) {
                // Nếu server trả về HTML trực tiếp, chèn luôn
                modalContent.innerHTML = xhr.responseText;
                console.log("Raw response:", xhr.responseText);
                attachModalEvents();
            }
        }
        else {
            alert("Lỗi server: " + xhr.status);
            console.log("Result", (xhr.responseText));
        }
    }

    console.log(formData);
    xhr.send(formData);
});

// Hiển thị thông báo tài khoản bị khóa
function showAccountLockedAlert(message) {
    const modalContent = document.querySelector('#loginModal .modal-content');
    if (modalContent) {
        modalContent.innerHTML = `
            <div class="modal-header bg-danger text-white">
                <h5 class="modal-title">
                    <i class="bi bi-lock-fill me-2"></i>Tài Khoản Bị Khóa
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body text-center py-4">
                <div class="mb-4">
                    <i class="bi bi-exclamation-triangle-fill text-warning" style="font-size: 4rem;"></i>
                </div>
                <h5 class="mb-3 text-danger">Không thể đăng nhập</h5>
                <p class="text-muted mb-4">${message}</p>
                <div class="alert alert-info">
                    <i class="bi bi-info-circle me-2"></i>
                    <strong>Cần hỗ trợ?</strong><br>
                    <span>Email: support@thanhlongbook.com</span><br>
                    <span>Hotline: 0767 375 429</span>
                </div>
            </div>
            <div class="modal-footer justify-content-center">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="bi bi-x-lg me-1"></i>Đóng
                </button>
                <button type="button" class="btn btn-primary" onclick="loadPartial('/Account/LoginPartial')">
                    <i class="bi bi-arrow-left me-1"></i>Thử lại
                </button>
            </div>
        `;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('loginModal');

    if (modal) {
        // When modal shows, clear previous content and load login partial if empty
        modal.addEventListener('show.bs.modal', function () {
            const form = modal.querySelector('form');
            if (form) {
                form.reset();
            }

            const validationSpans = modal.querySelectorAll('.text-danger');
            validationSpans.forEach(span => span.textContent = '');

            const modalBody = document.getElementById('modalBody');
            // If modal body is empty, load the login partial automatically
            if (modalBody && modalBody.innerHTML.trim() === '') {
                loadPartial('/Account/LoginPartial');
            }
        });

        // Fix: Đảm bảo modal content có thể click được khi modal đã hiển thị
        modal.addEventListener('shown.bs.modal', function () {
            // Đảm bảo modal dialog có pointer-events
            const modalDialog = modal.querySelector('.modal-dialog');
            const modalContent = modal.querySelector('.modal-content');
            if (modalDialog) {
                modalDialog.style.pointerEvents = 'auto';
            }
            if (modalContent) {
                modalContent.style.pointerEvents = 'auto';
            }
            
            // Focus vào input đầu tiên
            const firstInput = modal.querySelector('input:not([type="hidden"])');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
});

// ==================== User Dropdown Toggle ====================
document.addEventListener('DOMContentLoaded', function () {
    const userDropdownWrapper = document.querySelector('.user-dropdown-wrapper');
    const userDropdownToggle = document.getElementById('userDropdownToggle');

    if (userDropdownToggle && userDropdownWrapper) {
        // Toggle dropdown on click
        userDropdownToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            userDropdownWrapper.classList.toggle('active');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!userDropdownWrapper.contains(e.target)) {
                userDropdownWrapper.classList.remove('active');
            }
        });

        // Close dropdown when pressing Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                userDropdownWrapper.classList.remove('active');
            }
        });

        // Close dropdown when clicking on a menu item (except items with submenus)
        const menuItems = userDropdownWrapper.querySelectorAll('.dropdown-item');
        menuItems.forEach(function (item) {
            item.addEventListener('click', function () {
                // Small delay to allow navigation
                setTimeout(function () {
                    userDropdownWrapper.classList.remove('active');
                }, 100);
            });
        });
    }
});

// Category dropdown submenu handling
document.addEventListener('DOMContentLoaded', function () {
    // Handle dropdown submenu for category navigation
    const dropdownSubmenus = document.querySelectorAll('.dropdown-submenu');
    
    dropdownSubmenus.forEach(function(submenu) {
        const toggle = submenu.querySelector('.dropdown-toggle');
        
        if (toggle) {
            toggle.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                
                const parentDropdown = this.nextElementSibling;
                if (parentDropdown) {
                    // Toggle submenu
                    parentDropdown.classList.toggle('show');
                    
                    // Close other submenus
                    const siblings = submenu.parentElement.querySelectorAll('.dropdown-submenu .dropdown-menu');
                    siblings.forEach(function(sibling) {
                        if (sibling !== parentDropdown) {
                            sibling.classList.remove('show');
                        }
                    });
                }
            });
        }
        
        // Show submenu on hover for desktop
        submenu.addEventListener('mouseenter', function() {
            if (window.innerWidth > 768) {
                const dropdownMenu = this.querySelector('.dropdown-menu');
                if (dropdownMenu) {
                    dropdownMenu.classList.add('show');
                }
            }
        });
        
        submenu.addEventListener('mouseleave', function() {
            if (window.innerWidth > 768) {
                const dropdownMenu = this.querySelector('.dropdown-menu');
                if (dropdownMenu) {
                    dropdownMenu.classList.remove('show');
                }
            }
        });
    });
    
    // Close all submenus when main dropdown is hidden
    const mainDropdown = document.getElementById('categoryDropdown');
    if (mainDropdown) {
        mainDropdown.addEventListener('hidden.bs.dropdown', function () {
            const submenus = document.querySelectorAll('.dropdown-submenu .dropdown-menu');
            submenus.forEach(function(submenu) {
                submenu.classList.remove('show');
            });
        });
    }
});

async function loadPartial(url) {
    const modalBody = document.getElementById('modalBody') || document.getElementById('modalLabel');
    if (!modalBody) return;
    modalBody.innerHTML = '<p>Loading...</p>';
    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'accept': 'text/html',
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            throw new Error('Some error occured with partial view load');
        }

        const html = await response.text();
        modalBody.innerHTML = html;
        console.log("Raw response:", modalBody.innerHTML);
        attachModalEvents();
    }
    catch (e) {
        modalBody.innerHTML = '<p>Lỗi: ' + e.message + '</p>';
    }
}

function attachModalEvents() {
    const loginBtn = document.getElementById('loginBtn');
    const registerBtn = document.getElementById('registerBtn');

    if (loginBtn) {
        loginBtn.addEventListener('click', function () {
            loadPartial('Account/LoginPartial');
        });
    }

    if (registerBtn) {
        registerBtn.addEventListener('click', function () {
            loadPartial('Account/RegisterPartial');
        });
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const getStartedBtn = document.getElementById('getStartedBtn');

    if (getStartedBtn) {
        getStartedBtn.addEventListener('click', function () {
            // ensure modal's body is loaded when user clicks
            loadPartial('/Account/LoginPartial');
        });
    }
});










