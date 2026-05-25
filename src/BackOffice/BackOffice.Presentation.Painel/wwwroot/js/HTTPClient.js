

var HTTPClient = {

    get: (action, showLoading = true) => {

        if (showLoading) {
            mostrarCarregando(true);
        }

        var config = {
            method: "GET",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            }
        };

        let p = fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },

    getAsync: async (action, showLoading = true) => {
        if (showLoading) {
            mostrarCarregando(true);
        }
        var config = {
            method: "GET",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            }
        };

        let p = await fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },    

    delete: (action) => {
        mostrarCarregando(true);

        var config = {
            method: "DELETE",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            }
        };

        let p = fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },

    post: (action, body) => {
        mostrarCarregando(true);

        var config = {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            },
            body: body !== null ? JSON.stringify(body) : null
        };

        let p = fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },


    postAsync: async (action, body, showLoading = true) => {

        if (showLoading)
            mostrarCarregando(true);

        var config = {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            },
            body: body !== null ? JSON.stringify(body) : null
        };

        let p = await fetch(baseURL + action, config);


        if (showLoading) {
            Promise.all([p]).then(r => {
                mostrarCarregando(false);
            });
        }

        return p;
    },

    put: (action, body) => {
        mostrarCarregando(true);

        var config = {
            method: "PUT",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json; charset=utf-8"
            },
            body: body !== null ? JSON.stringify(body) : null
        };

        let p = fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },

    postFormData: (action, formData) => {
        mostrarCarregando(true);

        var config = {
            method: "POST",
            headers: {
                "Accept": "application/json",
            },
            body: formData
        };

        let p = fetch(baseURL + action, config);


        Promise.all([p]).then(r => {
            mostrarCarregando(false);
        });

        return p;
    },


    postFormDataAsync: async (action, formData, showLoading = true) => {

        if (showLoading)
            mostrarCarregando(true);

        var config = {
            method: "POST",
            headers: {
                "Accept": "application/json",
            },
            body: formData
        };

        let p = await fetch(baseURL + action, config);


        if (showLoading) {
            Promise.all([p]).then(r => {
                mostrarCarregando(false);
            });
        }

        return p;
    },


}