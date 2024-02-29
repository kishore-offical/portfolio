initNotification();
function initNotification() {
    let modalElement = document.getElementById('modalInfo');
    let modalInfo;
    if (modalElement) {
        modalInfo = new bootstrap.Modal(modalElement, {
            keyboard: false
        });
    }

    const element = document.querySelector('.validation-summary-errors ul li');
    let error = "";
    if (element) {
        error = element.textContent.trim();
        if (error)
            modalInfo.show();
    }
    if (modalElement) {
        modalElement.addEventListener('shown.bs.modal', function (event) {
            document.getElementById('notification').innerHTML = error;
            setTimeout(function () {
                modalInfo.hide();
            }, 10000000);
        }, { once: true });
    }
}

function showErrorNotification(message) {
    toastr.error(message, 'Error', { toastClass: 'toastr-error' });
}