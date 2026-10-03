// Give-AID Public Website Scripts
document.addEventListener("DOMContentLoaded", function () {
    // 1. Donation Amount Preset Buttons
    const presetButtons = document.querySelectorAll(".donation-preset-btn");
    const amountInput = document.getElementById("donationAmountInput");

    if (presetButtons.length > 0 && amountInput) {
        presetButtons.forEach(btn => {
            btn.addEventListener("click", function () {
                presetButtons.forEach(b => b.classList.remove("active"));
                this.classList.add("active");
                const val = this.getAttribute("data-amount");
                if (val && val !== "custom") {
                    amountInput.value = parseFloat(val).toFixed(2);
                } else if (val === "custom") {
                    amountInput.focus();
                }
            });
        });

        amountInput.addEventListener("input", function () {
            presetButtons.forEach(b => {
                if (b.getAttribute("data-amount") !== this.value) {
                    b.classList.remove("active");
                } else {
                    b.classList.add("active");
                }
            });
        });
    }

    // 2. Dummy Card Live Interactive Mirroring
    const cardInput = document.getElementById("CardNumber");
    const nameInput = document.getElementById("CardHolderName");
    const monthInput = document.getElementById("ExpiryMonth");
    const yearInput = document.getElementById("ExpiryYear");

    const previewNumber = document.getElementById("cardPreviewNumber");
    const previewName = document.getElementById("cardPreviewName");
    const previewExpiry = document.getElementById("cardPreviewExpiry");

    if (cardInput && previewNumber) {
        cardInput.addEventListener("input", function (e) {
            let val = e.target.value.replace(/\D/g, "");
            let formatted = val.match(/.{1,4}/g)?.join(" ") || val;
            if (formatted.length > 19) formatted = formatted.substring(0, 19);
            e.target.value = formatted;

            if (val.length >= 4) {
                let masked = "**** **** **** " + val.slice(-4);
                previewNumber.textContent = masked;
            } else {
                previewNumber.textContent = "**** **** **** ****";
            }
        });
    }

    if (nameInput && previewName) {
        nameInput.addEventListener("input", function (e) {
            previewName.textContent = (e.target.value || "CARDHOLDER NAME").toUpperCase();
        });
    }

    function updateExpiryPreview() {
        if (previewExpiry && monthInput && yearInput) {
            const mm = (monthInput.value || "MM").padStart(2, '0');
            const yy = yearInput.value ? yearInput.value.slice(-2) : "YY";
            previewExpiry.textContent = `${mm}/${yy}`;
        }
    }

    if (monthInput) monthInput.addEventListener("input", updateExpiryPreview);
    if (yearInput) yearInput.addEventListener("input", updateExpiryPreview);

    // 3. Auto-dismiss flash alerts
    const alerts = document.querySelectorAll(".alert-dismissible");
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 6000);
    });
});
