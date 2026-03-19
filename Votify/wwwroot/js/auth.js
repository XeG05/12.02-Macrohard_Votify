window.authStorage = {
    saveToken: (token) => localStorage.setItem("jwt_token", token),
    getToken: () => localStorage.getItem("jwt_token"),
    removeToken: () => localStorage.removeItem("jwt_token")
};