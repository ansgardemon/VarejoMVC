window.biometria = {
    isSupported: async () => {
        return window.PublicKeyCredential !== undefined;
    },
    // Solicita a digital/rosto (Retorna true se o usuário validou)
    autenticar: async () => {
        try {
            // Cria um "desafio" falso apenas para acionar o prompt nativo do celular/PC
            const challenge = new Uint8Array(32);
            window.crypto.getRandomValues(challenge);

            const cred = await navigator.credentials.get({
                publicKey: {
                    challenge: challenge,
                    rpId: window.location.hostname,
                    userVerification: "required" // Isso força pedir a biometria
                }
            });
            return cred !== null;
        } catch (err) {
            console.error("Erro na biometria:", err);
            return false;
        }
    }
};