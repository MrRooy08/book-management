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
        if (xhr.status === 200) {
            try {
                const result = JSON.parse(xhr.responseText);
                console.log("Result", result);
                if (result.success) {
                    // Login/Register thành công -> redirect
                    window.location.href = result.redirectUrl;
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

document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('loginModal');

    modal.addEventListener('show.bs.modal', function () {
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
        }

        const validationSpans = modal.querySelectorAll('.text-danger');
        validationSpans.forEach(span => span.textContent = '');
    });

});

document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('userDropdownToggle');

    if (toggleBtn) {
        toggleBtn.addEventListener('click', function () {
            document.querySelector('.custom_dropdown').classList.toggle('show');
        });
    }
});

async function loadPartial(url) {
    const modalBody = document.getElementById('modalBody') || document.getElementById('modalLabel');
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
            loadPartial('/Account/LoginPartial');
        });
    }
});










