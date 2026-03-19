window.authStorage = {
    saveToken: (token) => sessionStorage.setItem("jwt_token", token),
    getToken: () => sessionStorage.getItem("jwt_token"),
    removeToken: () => sessionStorage.removeItem("jwt_token")
};