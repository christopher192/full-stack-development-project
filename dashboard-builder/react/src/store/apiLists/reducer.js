import {
    API_RESPONSE_SUCCESS,
    API_RESPONSE_ERROR,
    GET_APILIST,
    GET_APILISTS_REACT_TABLE,
    SET_APILIST_LOADING_TRUE,
    ADD_APILIST_SUCCESS,
    ADD_APILIST_FAIL,
    UPDATE_APILIST_SUCCESS,
    UPDATE_APILIST_FAIL,
    DELETE_APILIST_SUCCESS,
    DELETE_APILIST_FAIL
} from "./actionType";

const INIT_STATE = {
    apiLists: [],
    apiList: {},
    isLoading: true,
    total: 0,
    error: {},
};

const ApiLists = (state = INIT_STATE, action) => {
    switch (action.type) {
        case API_RESPONSE_SUCCESS:
            switch (action.payload.actionType) {
                case GET_APILISTS_REACT_TABLE:
                    return {
                        ...state,
                        apiLists: action.payload.data.data,
                        total: action.payload.data.total,
                        isLoading: false
                    };
                default:
                    return { ...state };
            }
        case API_RESPONSE_ERROR:
            switch (action.payload.actionType) {
                case GET_APILISTS_REACT_TABLE:
                    return {
                        ...state,
                        error: action.payload.error,
                        isLoading: true,
                    };          
                default:
                    return { ...state };
            }
        case ADD_APILIST_SUCCESS:
            return {
                ...state,
                apiLists: action.payload.data,
                total: action.payload.total,
                isLoading: false
            };
        case ADD_APILIST_FAIL:
            return {
                ...state,
                error: action.payload,
                isLoading: true
            };
        case UPDATE_APILIST_SUCCESS:
            return {
                ...state,
                apiLists: state.apiLists.map(apiList =>
                    apiList.id.toString() === action.payload.id.toString()
                        ? { ...apiList, ...action.payload }
                        : apiList
                ),
            };
        case UPDATE_APILIST_FAIL:
            return {
                ...state,
                error: action.payload,
            };
        case DELETE_APILIST_SUCCESS:
            return {
                ...state,
                apiLists: state.apiLists.filter(apiList => apiList.id.toString() !== action.payload.id.toString()),
                total: state.total - 1
            };
        case DELETE_APILIST_FAIL:
            return {
                ...state,
                error: action.payload
            };                                  
        case SET_APILIST_LOADING_TRUE:
            return {
                ...state,
                isLoading: true
            };                   
        default:
            return { ...state };
    }
};

export default ApiLists;