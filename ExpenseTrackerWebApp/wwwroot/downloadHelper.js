    function downloadFileFromText(fileName, textContent) {
        console.log("hello");
        const blob = new Blob([textContent], { type: 'text/csv' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }

    // Dev-only: Remove empty Blazor error overlay on hot reload (every 200ms)
    // if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
    //     setInterval(() => {
    //         const overlay = document.getElementById('dotnet-compile-error');
    //         if (overlay && (!overlay.innerHTML || overlay.innerHTML.trim() === '')) {
    //             overlay.remove();
    //         }
    //     }, 200);
    // }