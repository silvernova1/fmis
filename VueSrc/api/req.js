import Axios from "axios";
Axios.defaults.timeout = 300000;
Axios.defaults.baseURL = process.env.API_BASE_URL;
//Axios.defaults.baseURL = import.meta.env.API_BASE_URL;
Axios.interceptors.request.use((request) => {
    //request.headers["Authorization"] = request.headers["Authorization"] ? '' : 'Bearer ' + S.getAuthToken();
    request.headers["Accept"] = "application/json";
    request.headers["Content-Type"] = "application/json;charset=UTF-8";
    return request;
});
Axios.interceptors.response.use((response) => {
    const code = response.status;
    if (code == undefined) {
        let error = {
            message: "network anomaly",
        };
        return Promise.reject(error);
    }
    else if (code < 200 || code > 300) {
        return Promise.reject("error");
    }
    else {
        return response.data;
    }
    //return response.data.data;
}, (error) => {
    let code = 0;
    let customCode = 0;
    try {
        code = error.response.status;
        customCode = error.response.code;
    }
    catch (e) {
        if (error.toString().indexOf("Error: timeout") !== -1) {
            return Promise.reject(error);
        }
    }
    if (customCode) {
        if (customCode === 403) {
            console.log(error.response.data.message);
        }
        else if (customCode === 500) {
            console.log(error.response.data.message);
        }
    }
    else if (code) {
        if (code === 401) {
        }
        else if (code === 403) {
        }
        else {
            console.log(error.response.data.message);
        }
    }
    return Promise.reject(error.response);
});
function get(url, params, headers = {}) {
    return Axios.get(url, { params, headers });
}
function deletes(url, params, headers = {}) {
    return Axios.delete(url, {
        params,
        headers,
    });
}
function post(url, params, headers = {}) {
    return Axios.post(url, params, headers);
}
function put(url, params, headers = {}) {
    return Axios.put(url, params, headers);
}
export default {
    get,
    deletes,
    post,
    put,
};
