import req from "./req";
const urlObject = new URL(window.location.href)
const domain = urlObject.hostname
const domainExtension = domain === "192.168.110.45" ? "/dummy" : "" 
export function userDetails(params: {} = {}) {
    return req.get(domainExtension+"/Handsontable/Details", params);
}

