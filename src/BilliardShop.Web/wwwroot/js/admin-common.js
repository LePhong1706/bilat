// Common JavaScript functions for Admin area

// Show toast notification
function showToast(message, type = 'success') {
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(toastContainer);
    }

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = toastContainer.lastElementChild;
    const toast = new bootstrap.Toast(toastElement);
    toast.show();

    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function() {
        toastElement.remove();
    });
}

// Generic delete function with AJAX
async function deleteItem(url, onSuccess) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (!token) {
            showToast('Không tìm thấy token bảo mật', 'error');
            return;
        }

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token.value
            }
        });

        const result = await response.json();

        if (result.success) {
            showToast(result.message, 'success');
            if (onSuccess) {
                setTimeout(onSuccess, 1000);
            }
        } else {
            showToast(result.message, 'error');
        }

        return result;
    } catch (error) {
        console.error('Error:', error);
        showToast('Có lỗi xảy ra khi thực hiện thao tác', 'error');
        return { success: false, message: error.message };
    }
}
