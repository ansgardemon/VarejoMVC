window.dashboardChart = null;

window.renderizarGrafico = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Se o Auto-Refresh rodar, precisamos destruir o gráfico velho antes de desenhar o novo
    if (window.dashboardChart) {
        window.dashboardChart.destroy();
    }

    window.dashboardChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Volume de Produtos',
                data: data,
                backgroundColor: "rgba(22, 56, 137, 0.7)", // O seu Azul Glassmorphism
                borderColor: "rgba(22, 56, 137, 1)",
                borderWidth: 1,
                borderRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                x: { grid: { display: false } },
                y: { beginAtZero: true, ticks: { stepSize: 1 } }
            }
        }
    });
};