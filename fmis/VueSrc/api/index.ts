import req from "./req";

export function userDetails(params: {} = {}) {
    return req.get("/Handsontable/Details", params);
}

