window.pushNotify = {
    show: (title, body) => {
        if (Notification.permission === 'granted') {
            new Notification(title, { body: body, icon: '/icon-192.png' });
        } else if (Notification.permission !== 'denied') {
            Notification.requestPermission().then(permission => {
                if (permission === 'granted') new Notification(title, { body: body });
            });
        }
    }
};