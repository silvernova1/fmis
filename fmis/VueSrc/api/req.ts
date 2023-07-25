import Axios from "axios";

declare module "axios" {
    export interface AxiosResponse<T = any> extends Promise<T> { }
}

Axios.defaults.timeout = 300000;

Axios.defaults.baseURL = process.env.API_BASE_URL;
//Axios.defaults.baseURL = import.meta.env.API_BASE_URL;
Axios.interceptors.request.use((request: any) => {

    //request.headers["Authorization"] = request.headers["Authorization"] ? '' : 'Bearer ' + S.getAuthToken();
    request.headers["Accept"] = "application/json";
    request.headers["Content-Type"] = "application/json;charset=UTF-8";

    return request;
});

Axios.interceptors.response.use(
    (response) => {
        const code = response.status;

        if (code == undefined) {
            let error = {
                message: "network anomaly",
            };
            return Promise.reject(error);
        } else if (code < 200 || code > 300) {
            return Promise.reject("error");
        }
        else {
            return response.data
        }
    },
    (error) => {
        console.log(error)
        let code = 0;
        let customCode = 0;
        try {
            code = error.response.status;
            customCode = error.response.code;
        } catch (e) {
            if (error.toString().indexOf("Error: timeout") !== -1) {
                return Promise.reject(error);
            }
        }

        if (customCode) {
            if (customCode === 403) {
                console.log(error.response.data.message);
            } else if (customCode === 500) {
                console.log(error.response.data.message);
            }
        } else if (code) {
            if (code === 401) {

            } else if (code === 403) {
            } else {
                console.log(error.response.data.message);
            }
        }

        return Promise.reject(error.response);
    }
);

function get(url: string, params: any, headers: {} = {}) {
    return Axios.get(url, { params, headers });
}

function deletes(url: string, params: any, headers: {} = {}) {
    return Axios.delete(url, {
        params,
        headers,
    });
}

async function post(url: string, params: any, headers: {} = {}) {
    return await Axios.post(url, params, headers);
}

function put(url: string, params: any, headers: {} = {}) {
    return Axios.put(url, params, headers);
}

export default {
    get,
    deletes,
    post,
    put,
};
