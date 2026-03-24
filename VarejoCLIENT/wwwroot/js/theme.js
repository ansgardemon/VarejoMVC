window.toggleTheme = (isDark) => {
    const root = document.documentElement;

    if (isDark) {
        root.setAttribute("data-theme", "dark");
    } else {
        root.removeAttribute("data-theme");
    }
};