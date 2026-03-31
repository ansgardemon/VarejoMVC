window.downloadFileFromBytes = function (fileName, bytes) {
    // Cria um arquivo (Blob) a partir dos bytes
    var blob = new Blob([bytes], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;

    // Clica e limpa a memória
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(link.href);
};