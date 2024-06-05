import { call, put, takeEvery, all, fork, select } from "redux-saga/effects";

import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import {
    GET_APILISTS_REACT_TABLE,
    SET_APILIST_LOADING_TRUE,
    ADD_APILIST,
    UPDATE_APILIST,
    DELETE_APILIST
} from "./actionType";

import {
    apiListApiResponseSuccess,
    apiListApiResponseError,
    setApiListLoadingTrue,
    addApiListSuccess,
    addApiListFail,
    updateApiListSuccess,
    updateApiListFail,
    deleteApiListSuccess,
    deleteApiListFail
} from "./action";

import {
    getReactApiLists as getReactApiListsApi,
    addNewApiList,
    updateApiList,
    deleteApiList
} from "../../helpers/fakebackend_helper";

function* getApiLists({ payload: data }) {
    try {
        const isLoading = yield select(state => state.ApiLists.isLoading);
        if (!isLoading) {
            yield put(setApiListLoadingTrue());
        }
        const response = yield call(getReactApiListsApi, data.params);
        yield put(apiListApiResponseSuccess(GET_APILISTS_REACT_TABLE, response));
    } catch (error) {
        yield put(apiListApiResponseError(GET_APILISTS_REACT_TABLE, error));
    }
}

function* addApiList({ payload: data }) {
    try {
        const response = yield call(addNewApiList, data.data);

        if (response.id !== 0) {
            const isLoading = yield select(state => state.ApiLists.isLoading);
            
            if (!isLoading) {
                yield put(setApiListLoadingTrue());
            }

            const response = yield call(getReactApiListsApi, data.params);
            yield put(addApiListSuccess(response));

            toast.success("ApiList Added Success", { autoClose: 3000 });
        }
    } catch (error) {
        yield put(addApiListFail(error));

        toast.error("ApiList Added Failed", { autoClose: 3000 });
    }
}

function* updateApiListSaga({ payload: data }) {
    try {
        const response = yield call(updateApiList, data);
        if (response.id !== 0) { 
            yield put(updateApiListSuccess(response));

            toast.success("ApiList Update Success", { autoClose: 3000 });
        }
    } catch (error) {
        yield put(updateApiListFail(error));

        toast.error("ApiList Update Failed", { autoClose: 3000 });
    }
}

function* deleteApiListSaga({ payload: data }) {
    try {
        const response = yield call(deleteApiList, data.id);
        yield put(deleteApiListSuccess(data));

        toast.success("ApiList Delete Success", { autoClose: 3000 });
    } catch (error) {
        yield put(deleteApiListFail(error));

        toast.error("ApiList Delete Failed", { autoClose: 3000 });
    }
}

export function* watchGetApiLists() {
    yield takeEvery(GET_APILISTS_REACT_TABLE, getApiLists);
}

export function* watchAddApiList() {
    yield takeEvery(ADD_APILIST, addApiList);
}

export function* watchUpdateApiList() {
    yield takeEvery(UPDATE_APILIST, updateApiListSaga);
}

export function* watchDeleteApiList() {
    yield takeEvery(DELETE_APILIST, deleteApiListSaga);
}

function* apiListSaga() {
    yield all([
        fork(watchGetApiLists),
        fork(watchAddApiList),
        fork(watchUpdateApiList),
        fork(watchDeleteApiList)
    ]);
}

export default apiListSaga;
