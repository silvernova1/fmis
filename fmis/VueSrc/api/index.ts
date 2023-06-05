import req from "./req";
const urlObject = new URL(window.location.href)
const domain = urlObject.hostname
const domainExtension = domain === "192.168.110.45" ? "/dummy" : "" 
export function userDetails(params: {} = {}) {
    return req.get(domainExtension+"/Handsontable/Details", params);
}

export function obligationData(params: {} = {}) {
    return req.get(domainExtension + "/Handsontable/Obligation", params);
}

export function fundSub(params: {} = {}) {
    return req.get(domainExtension + "/Handsontable/fundSub", params);
}

export function expCode(params: {} = {}) {
    return req.get(domainExtension + "/Handsontable/expCode", params);
}







