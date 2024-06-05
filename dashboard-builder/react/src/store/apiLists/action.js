import {
    GET_APILIST,
    GET_APILISTS_REACT_TABLE,
    API_RESPONSE_SUCCESS,
    API_RESPONSE_ERROR,
    DELETE_APILIST,
    DELETE_APILIST_SUCCESS,
    DELETE_APILIST_FAIL,
    ADD_APILIST,
    ADD_APILIST_SUCCESS,
    ADD_APILIST_FAIL,
    UPDATE_APILIST,
    UPDATE_APILIST_SUCCESS,
    UPDATE_APILIST_FAIL,
    SET_APILIST_LOADING_TRUE
} from "./actionType";

// common success
export const apiListApiResponseSuccess = (actionType, data) => ({
    type: API_RESPONSE_SUCCESS,
    payload: { actionType, data },
});
// common error
export const apiListApiResponseError = (actionType, error) => ({
    type: API_RESPONSE_ERROR,
    payload: { actionType, error },
});

export const getApiList = () => ({
    type: GET_APILIST,
});

export const deleteApiList = data => ({
    type: DELETE_APILIST,
    payload: data,
});

export const deleteApiListSuccess = data => ({
    type: DELETE_APILIST_SUCCESS,
    payload: data,
});

export const deleteApiListFail = error => ({
    type: DELETE_APILIST_FAIL,
    payload: error,
});

export const addApiList = (data, params) => ({
    type: ADD_APILIST,
    payload: { data, params },
});

export const addApiListSuccess = apiList => ({
    type: ADD_APILIST_SUCCESS,
    payload: apiList,
});

export const addApiListFail = error => ({
    type: ADD_APILIST_FAIL,
    payload: error,
});

export const updateApiList = apiList => ({
    type: UPDATE_APILIST,
    payload: apiList,
});

export const updateApiListSuccess = apiList => ({
    type: UPDATE_APILIST_SUCCESS,
    payload: apiList,
});

export const updateApiListFail = error => ({
    type: UPDATE_APILIST_FAIL,
    payload: error,
});

export const getApiListsReactTable = (params) => ({
    type: GET_APILISTS_REACT_TABLE,
    payload: { params }
});
  
export const setApiListLoadingTrue = () => ({
    type: SET_APILIST_LOADING_TRUE
});